# Google Places API Proxy - Documentación

## Resumen

Este documento describe la implementación del proxy para Google Places API que soluciona el error "Error en la busqueda, la solicitud fue denegada" que ocurría cuando las aplicaciones cliente intentaban realizar búsquedas de lugares directamente.

## Problema Original

El error "Error en la busqueda, la solicitud fue denegada" ocurría porque:

1. **Exposición de API Key**: La API key se exponía al cliente a través del endpoint `/api/GoogleMapsConfig/api-key`
2. **Llamadas Directas**: El cliente hacía llamadas directas a Google Places API
3. **Restricciones de API Key**: Las restricciones de Google Maps API Key no funcionan correctamente con aplicaciones de escritorio (WinUI3)
4. **HTTP Referrers**: Las aplicaciones de escritorio no envían HTTP referrers, por lo que no pueden usar ese tipo de restricción

## Solución Implementada

Se creó un **proxy backend** que maneja todas las solicitudes a Google Places API. Esto permite:

- ✅ Mantener la API key segura en el servidor
- ✅ Aplicar restricciones basadas en IP del servidor
- ✅ Centralizar el logging y manejo de errores
- ✅ Validar parámetros antes de enviar a Google
- ✅ Requiere autenticación JWT para todas las solicitudes

## Arquitectura

```
Cliente (WinUI3) → [JWT Auth] → Backend API (/api/GooglePlaces/*) → Google Places API
```

El cliente ya no necesita la API key de Google Maps para búsquedas. Solo necesita:
1. Token JWT válido
2. Llamar a los endpoints del proxy backend

## Endpoints Implementados

Todos los endpoints requieren autenticación JWT en el header:
```
Authorization: Bearer <tu-token-jwt>
```

### 1. Búsqueda de Lugares (Text Search)

Busca lugares por texto usando Google Places API.

**Endpoint:** `GET /api/GooglePlaces/search`

**Parámetros Query:**
- `query` (string, **requerido**): Texto de búsqueda (ej: "restaurante", "OXXO", "gasolinera")
- `location` (string, opcional): Ubicación central en formato `"lat,lng"` (ej: "19.4326,-99.1332")
- `radius` (int, opcional): Radio de búsqueda en metros (1 a 50000)

**Ejemplo de Uso:**
```http
GET /api/GooglePlaces/search?query=restaurante&location=19.4326,-99.1332&radius=5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Respuesta Exitosa (200 OK):**
Retorna el JSON directamente de Google Places API:
```json
{
  "html_attributions": [],
  "results": [
    {
      "formatted_address": "Av. Insurgentes Sur 1605, CDMX",
      "geometry": {
        "location": {
          "lat": 19.3799,
          "lng": -99.1710
        }
      },
      "name": "Restaurante El Ejemplo",
      "place_id": "ChIJN1t_tDeuEmsRUsoyG83frY4",
      "rating": 4.5,
      "types": ["restaurant", "food", "point_of_interest"]
    }
  ],
  "status": "OK"
}
```

**Códigos de Error:**
- `400 Bad Request`: Parámetros inválidos
- `401 Unauthorized`: Token JWT faltante o inválido
- `500 Internal Server Error`: Error al conectar con Google Places API

### 2. Detalles de un Lugar

Obtiene información detallada de un lugar específico usando su Place ID.

**Endpoint:** `GET /api/GooglePlaces/details`

**Parámetros Query:**
- `placeId` (string, **requerido**): ID del lugar en Google Places (ej: "ChIJN1t_tDeuEmsRUsoyG83frY4")

**Ejemplo de Uso:**
```http
GET /api/GooglePlaces/details?placeId=ChIJN1t_tDeuEmsRUsoyG83frY4
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Respuesta Exitosa (200 OK):**
```json
{
  "html_attributions": [],
  "result": {
    "address_components": [...],
    "formatted_address": "Av. Insurgentes Sur 1605, CDMX",
    "formatted_phone_number": "(55) 1234-5678",
    "geometry": {
      "location": {
        "lat": 19.3799,
        "lng": -99.1710
      }
    },
    "name": "Restaurante El Ejemplo",
    "opening_hours": {...},
    "place_id": "ChIJN1t_tDeuEmsRUsoyG83frY4",
    "rating": 4.5,
    "reviews": [...],
    "types": ["restaurant", "food", "point_of_interest"],
    "website": "http://www.ejemplo.com"
  },
  "status": "OK"
}
```

### 3. Autocompletado de Lugares

Obtiene sugerencias de autocompletado para búsquedas de lugares.

**Endpoint:** `GET /api/GooglePlaces/autocomplete`

**Parámetros Query:**
- `input` (string, **requerido**): Texto de entrada del usuario (ej: "rest", "OXX")
- `location` (string, opcional): Ubicación central en formato `"lat,lng"` para priorizar resultados
- `radius` (int, opcional): Radio de búsqueda en metros (1 a 50000)

**Ejemplo de Uso:**
```http
GET /api/GooglePlaces/autocomplete?input=rest&location=19.4326,-99.1332&radius=5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Respuesta Exitosa (200 OK):**
```json
{
  "predictions": [
    {
      "description": "Restaurante Los Arcos, CDMX",
      "matched_substrings": [
        {
          "length": 4,
          "offset": 0
        }
      ],
      "place_id": "ChIJN1t_tDeuEmsRUsoyG83frY4",
      "reference": "...",
      "structured_formatting": {
        "main_text": "Restaurante Los Arcos",
        "main_text_matched_substrings": [
          {
            "length": 4,
            "offset": 0
          }
        ],
        "secondary_text": "CDMX"
      },
      "terms": [...],
      "types": ["restaurant", "food", "establishment"]
    }
  ],
  "status": "OK"
}
```

## Validaciones Implementadas

Todos los endpoints validan:

1. **Autenticación**: Token JWT válido requerido
2. **Parámetros Requeridos**: Verifica que los parámetros obligatorios estén presentes
3. **Formato de Coordenadas**: Valida que `location` esté en formato `"lat,lng"` correcto
4. **Rango de Coordenadas**: Latitud entre -90 y 90, Longitud entre -180 y 180
5. **Rango de Radio**: Entre 1 y 50,000 metros
6. **Respuestas de Google**: Valida el estado de la respuesta de Google Places API

## Manejo de Errores

### Errores de Validación (400 Bad Request)

```json
{
  "message": "El parámetro 'query' es requerido"
}
```

### Errores de Google Places API (500 Internal Server Error)

En modo DEBUG:
```json
{
  "message": "Error de Google Places API: The provided API key is invalid.",
  "innerMessage": "..."
}
```

En modo RELEASE:
```json
{
  "message": "Error al buscar lugares en Google Places API"
}
```

### Estado REQUEST_DENIED

Si Google Places API retorna `REQUEST_DENIED`, el servicio lanza una excepción con el mensaje de error específico de Google, que puede incluir:

- "This API project is not authorized to use this API"
- "The provided API key is invalid"
- "This API key is not authorized to use this service or API"

## Logging

El servicio registra todas las operaciones importantes:

```
[Information] GooglePlacesService inicializado correctamente
[Information] Buscando lugares: restaurante
[Information] Búsqueda completada con estado: OK
[Warning] Google Places API retornó estado: ZERO_RESULTS
[Error] Error de red al llamar a Google Places API
```

## Configuración

La API key se obtiene de `appsettings.json`:

```json
{
  "GoogleMaps": {
    "ApiKey": "AIzaSy....",
    "DefaultCenter": "19.4326,-99.1332",
    "DefaultZoom": "15"
  }
}
```

**Importante:** En producción, la API key debe almacenarse en:
- Azure Key Vault
- Variables de entorno
- .NET User Secrets (solo desarrollo)

## Configuración de Google Cloud Console

Para que el proxy funcione correctamente, la API key de Google debe:

1. **Tener habilitadas las APIs:**
   - Places API
   - Maps JavaScript API (para visualización en el cliente)
   - Geocoding API (opcional, para conversión de direcciones)

2. **Restricciones recomendadas:**
   - **IP addresses**: Restringir a la IP del servidor (más seguro)
   - **APIs**: Limitar solo a las APIs necesarias

3. **NO usar restricciones de:**
   - HTTP referrers (no aplican para llamadas desde servidor)
   - Android/iOS apps (no aplican para este caso)

## Uso desde el Cliente (WinUI3)

### Ejemplo en C# (WinUI3)

```csharp
public class PlacesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _accessToken;

    public PlacesApiClient(string baseUrl, string accessToken)
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl;
        _accessToken = accessToken;
    }

    public async Task<PlacesSearchResult> SearchPlacesAsync(
        string query, 
        string location = null, 
        int? radius = null)
    {
        var url = $"{_baseUrl}/api/GooglePlaces/search?query={Uri.EscapeDataString(query)}";
        
        if (!string.IsNullOrWhiteSpace(location))
            url += $"&location={location}";
            
        if (radius.HasValue)
            url += $"&radius={radius}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = 
            new AuthenticationHeaderValue("Bearer", _accessToken);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlacesSearchResult>(json);
    }

    public async Task<PlaceDetails> GetPlaceDetailsAsync(string placeId)
    {
        var url = $"{_baseUrl}/api/GooglePlaces/details?placeId={Uri.EscapeDataString(placeId)}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = 
            new AuthenticationHeaderValue("Bearer", _accessToken);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlaceDetails>(json);
    }

    public async Task<AutocompleteResult> AutocompletePlacesAsync(
        string input,
        string location = null,
        int? radius = null)
    {
        var url = $"{_baseUrl}/api/GooglePlaces/autocomplete?input={Uri.EscapeDataString(input)}";
        
        if (!string.IsNullOrWhiteSpace(location))
            url += $"&location={location}";
            
        if (radius.HasValue)
            url += $"&radius={radius}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = 
            new AuthenticationHeaderValue("Bearer", _accessToken);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AutocompleteResult>(json);
    }
}
```

## Migración desde Implementación Anterior

Si anteriormente el cliente hacía llamadas directas a Google Places API:

### Antes:
```csharp
// Cliente obtenía API key y hacía llamada directa
var apiKey = await GetApiKeyFromBackend();
var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={query}&key={apiKey}";
var response = await httpClient.GetAsync(url);
// Error: REQUEST_DENIED
```

### Ahora:
```csharp
// Cliente llama al proxy backend con JWT
var url = $"{backendUrl}/api/GooglePlaces/search?query={query}";
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
var response = await httpClient.SendAsync(request);
// Funciona correctamente
```

## Beneficios de Seguridad

1. **API Key Protegida**: Nunca se expone al cliente
2. **Autenticación Requerida**: Solo usuarios autenticados pueden buscar
3. **Restricciones de IP**: La API key puede restringirse a la IP del servidor
4. **Logging Centralizado**: Todas las búsquedas se registran en el servidor
5. **Rate Limiting**: Se puede implementar límites de tasa por usuario
6. **Validación de Entrada**: Previene consultas maliciosas

## Testing

### Usando Swagger

1. Ir a `/swagger` en el servidor
2. Autenticarse usando el endpoint `/api/Auth/login`
3. Copiar el `accessToken` de la respuesta
4. Hacer clic en "Authorize" en Swagger UI
5. Pegar el token en formato: `Bearer <token>`
6. Probar los endpoints de `GooglePlaces`

### Usando cURL

```bash
# Obtener token
curl -X POST "https://tu-servidor.com/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"usuario","password":"contraseña"}'

# Buscar lugares
curl -X GET "https://tu-servidor.com/api/GooglePlaces/search?query=restaurante&location=19.4326,-99.1332&radius=5000" \
  -H "Authorization: Bearer <tu-token>"

# Autocompletar
curl -X GET "https://tu-servidor.com/api/GooglePlaces/autocomplete?input=rest&location=19.4326,-99.1332" \
  -H "Authorization: Bearer <tu-token>"

# Detalles de lugar
curl -X GET "https://tu-servidor.com/api/GooglePlaces/details?placeId=ChIJN1t_tDeuEmsRUsoyG83frY4" \
  -H "Authorization: Bearer <tu-token>"
```

## Archivos Implementados

1. **`Services/IGooglePlacesService.cs`**: Interface del servicio
2. **`Services/GooglePlacesService.cs`**: Implementación del servicio con llamadas a Google Places API
3. **`Controllers/GooglePlacesController.cs`**: Endpoints REST del proxy
4. **`Program.cs`**: Registro de dependencias (HttpClient y GooglePlacesService)

## Limitaciones y Consideraciones

1. **Cuotas de Google**: El servidor compartirá las cuotas de Google Places API entre todos los usuarios
2. **Caching**: Considerar implementar caché para reducir llamadas a Google (no implementado)
3. **Rate Limiting**: Considerar implementar límites por usuario para evitar abuso (no implementado)
4. **Costos**: Monitorear costos de Google Places API ya que todas las búsquedas pasan por el servidor

## Próximos Pasos (Opcional)

1. Implementar sistema de caché para resultados frecuentes
2. Implementar rate limiting por usuario
3. Agregar métricas y monitoreo de uso
4. Implementar paginación para resultados grandes
5. Agregar soporte para más APIs de Google (Directions, Distance Matrix, etc.)

## Conclusión

Esta implementación soluciona el error "Error en la busqueda, la solicitud fue denegada" al centralizar las llamadas a Google Places API en el backend, donde la API key puede ser protegida adecuadamente y las restricciones de seguridad pueden aplicarse correctamente.

El cliente ahora solo necesita:
- Autenticarse con JWT
- Llamar a los endpoints del proxy backend
- Procesar las respuestas de Google Places API sin preocuparse por la configuración de la API key
