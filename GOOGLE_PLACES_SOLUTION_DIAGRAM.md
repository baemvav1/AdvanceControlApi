# Diagrama de la Solución - Google Places API Proxy

## Flujo de Autenticación y Búsqueda

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           FLUJO COMPLETO                                  │
└─────────────────────────────────────────────────────────────────────────┘

┌──────────────┐
│   Cliente    │
│   (WinUI3)   │
└──────┬───────┘
       │
       │ 1. POST /api/Auth/login
       │    {username, password}
       ▼
┌──────────────────┐
│   Backend API    │
│  Auth Controller │
└──────┬───────────┘
       │
       │ 2. ◄── JWT Token
       ▼
┌──────────────┐
│   Cliente    │ (guarda token)
└──────┬───────┘
       │
       │ 3. GET /api/GooglePlaces/search?query=restaurante
       │    Authorization: Bearer <token>
       ▼
┌──────────────────────┐
│   Backend API        │
│ GooglePlaces         │
│ Controller           │
└──────┬───────────────┘
       │
       │ 4. Valida JWT
       │ 5. Valida parámetros
       ▼
┌──────────────────────┐
│ GooglePlacesService  │
└──────┬───────────────┘
       │
       │ 6. GET https://maps.googleapis.com/maps/api/place/
       │    textsearch/json?query=restaurante&key=AIza...
       ▼
┌──────────────────────┐
│   Google Places API  │
└──────┬───────────────┘
       │
       │ 7. {results: [...], status: "OK"}
       ▼
┌──────────────────────┐
│ GooglePlacesService  │
└──────┬───────────────┘
       │
       │ 8. Valida respuesta
       │ 9. Log resultado
       ▼
┌──────────────────────┐
│ GooglePlaces         │
│ Controller           │
└──────┬───────────────┘
       │
       │ 10. ─► Retorna JSON a cliente
       ▼
┌──────────────┐
│   Cliente    │ (procesa resultados)
└──────────────┘
```

## Comparación: ANTES vs AHORA

```
╔═══════════════════════════════════════════════════════════════╗
║                         ANTES (❌)                             ║
╚═══════════════════════════════════════════════════════════════╝

┌──────────────┐
│   Cliente    │
│   (WinUI3)   │
└──────┬───────┘
       │
       │ 1. GET /api/GoogleMapsConfig/api-key
       ▼
┌──────────────────┐
│   Backend API    │ ◄── ⚠️ API Key expuesta
└──────┬───────────┘
       │
       │ 2. {apiKey: "AIza..."}
       ▼
┌──────────────┐
│   Cliente    │
└──────┬───────┘
       │
       │ 3. GET https://maps.googleapis.com/...?key=AIza...
       ▼
┌──────────────────────┐
│   Google Places API  │
└──────┬───────────────┘
       │
       │ ❌ REQUEST_DENIED
       │ (Restricciones de API key no aplican
       │  a aplicaciones de escritorio)
       ▼
┌──────────────┐
│   Cliente    │ ◄── ❌ "Error en la busqueda,
└──────────────┘        la solicitud fue denegada"


╔═══════════════════════════════════════════════════════════════╗
║                         AHORA (✅)                             ║
╚═══════════════════════════════════════════════════════════════╝

┌──────────────┐
│   Cliente    │
│   (WinUI3)   │
└──────┬───────┘
       │
       │ 1. Autenticación JWT
       ▼
┌──────────────────┐
│   Backend API    │
└──────┬───────────┘
       │
       │ 2. JWT Token
       ▼
┌──────────────┐
│   Cliente    │
└──────┬───────┘
       │
       │ 3. GET /api/GooglePlaces/search?query=...
       │    Authorization: Bearer <token>
       ▼
┌──────────────────────┐
│   Backend Proxy      │ ◄── ✅ API Key protegida
│  + Validaciones      │     ✅ JWT auth
│  + Logging           │     ✅ Validaciones
└──────┬───────────────┘
       │
       │ 4. GET https://maps.googleapis.com/...?key=<secret>
       ▼
┌──────────────────────┐
│   Google Places API  │
└──────┬───────────────┘
       │
       │ ✅ OK (IP del servidor autorizada)
       ▼
┌──────────────────────┐
│   Backend Proxy      │
└──────┬───────────────┘
       │
       │ 5. {results: [...]}
       ▼
┌──────────────┐
│   Cliente    │ ◄── ✅ Búsqueda exitosa
└──────────────┘
```

## Arquitectura de Capas

```
┌─────────────────────────────────────────────────────────────┐
│                    CLIENTE (WinUI3)                          │
│  - UI de búsqueda de mapas                                  │
│  - Gestión de JWT token                                     │
│  - Renderizado de resultados                                │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ HTTPS + JWT Auth
                         │
┌────────────────────────▼────────────────────────────────────┐
│              CAPA DE PRESENTACIÓN (API)                      │
│  ┌─────────────────────────────────────────────────────┐   │
│  │       GooglePlacesController.cs                      │   │
│  │  - [Authorize] JWT                                   │   │
│  │  - Validación de parámetros                          │   │
│  │  - 3 endpoints (search, details, autocomplete)       │   │
│  └─────────────────────┬───────────────────────────────┘   │
└────────────────────────┼────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│               CAPA DE NEGOCIO (Services)                     │
│  ┌─────────────────────────────────────────────────────┐   │
│  │      GooglePlacesService.cs                          │   │
│  │  - Construye URLs de Google Places API              │   │
│  │  - Agrega API key (desde config)                    │   │
│  │  - Valida respuestas de Google                      │   │
│  │  - Manejo de errores REQUEST_DENIED                 │   │
│  │  - Logging estructurado                             │   │
│  └─────────────────────┬───────────────────────────────┘   │
└────────────────────────┼────────────────────────────────────┘
                         │
                         │ HttpClient (30s timeout)
                         │
┌────────────────────────▼────────────────────────────────────┐
│            SERVICIOS EXTERNOS (Google)                       │
│  ┌─────────────────────────────────────────────────────┐   │
│  │      Google Places API                               │   │
│  │  - Text Search                                       │   │
│  │  - Place Details                                     │   │
│  │  - Autocomplete                                      │   │
│  └─────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────┘
```

## Flujo de Validaciones

```
┌──────────────────────────────────────────────────────────────┐
│              Request Validation Pipeline                      │
└──────────────────────────────────────────────────────────────┘

Client Request
     │
     ▼
┌─────────────────────┐
│  [Authorize]        │ ◄── Valida JWT Token
│  Attribute          │
└────────┬────────────┘
         │ ✅ Token válido
         ▼
┌─────────────────────┐
│  Controller         │
│  - Required params? │ ◄── query/input no vacío
└────────┬────────────┘
         │ ✅ Params presentes
         ▼
┌─────────────────────┐
│  ValidateLocation() │ ◄── Formato "lat,lng"
│                     │     Lat: -90 to 90
│                     │     Lng: -180 to 180
└────────┬────────────┘
         │ ✅ Coordenadas válidas
         ▼
┌─────────────────────┐
│  ValidateRadius()   │ ◄── 1 ≤ radius ≤ 50000
└────────┬────────────┘
         │ ✅ Radio válido
         ▼
┌─────────────────────┐
│  Service Layer      │
│  - Build URL        │
│  - Add API key      │
│  - Call Google API  │
└────────┬────────────┘
         │
         ▼
┌─────────────────────┐
│  Google Response    │
│  - Check status     │ ◄── OK, ZERO_RESULTS: ✅
│  - Handle errors    │     REQUEST_DENIED: ❌
└────────┬────────────┘
         │
         ▼
     Response
```

## Seguridad en Capas

```
╔══════════════════════════════════════════════════════════════╗
║                    SEGURIDAD MULTI-CAPA                       ║
╚══════════════════════════════════════════════════════════════╝

┌──────────────────────────────────────────────────────────────┐
│ Capa 1: Autenticación                                         │
│ ✅ JWT Bearer Token requerido en todos los endpoints         │
│ ✅ Token valida identidad del usuario                        │
└──────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│ Capa 2: Validación de Entrada                                │
│ ✅ Parámetros requeridos presentes                           │
│ ✅ Formato de coordenadas correcto                           │
│ ✅ Rangos de valores dentro de límites                       │
└──────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│ Capa 3: Protección de API Key                                │
│ ✅ API key nunca expuesta al cliente                         │
│ ✅ API key solo en servidor                                  │
│ ✅ Restricciones de IP en Google Cloud Console               │
└──────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│ Capa 4: Validación de Respuestas                             │
│ ✅ Verifica status de Google API                             │
│ ✅ Maneja REQUEST_DENIED con mensaje descriptivo             │
│ ✅ Logging de todas las operaciones                          │
└──────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│ Capa 5: Manejo de Errores                                    │
│ ✅ Mensajes diferentes para DEBUG/RELEASE                    │
│ ✅ No expone detalles internos en producción                 │
│ ✅ Timeout de 30s previene hanging requests                  │
└──────────────────────────────────────────────────────────────┘
```

## Casos de Uso

```
┌─────────────────────────────────────────────────────────────┐
│                    CASO 1: Búsqueda Simple                   │
└─────────────────────────────────────────────────────────────┘

Usuario escribe: "restaurante"
                 │
                 ▼
GET /api/GooglePlaces/search?query=restaurante
    Authorization: Bearer eyJ...
                 │
                 ▼
Backend valida y llama a Google
                 │
                 ▼
Retorna lista de restaurantes cercanos


┌─────────────────────────────────────────────────────────────┐
│               CASO 2: Búsqueda con Ubicación                 │
└─────────────────────────────────────────────────────────────┘

Usuario busca: "OXXO" cerca de ubicación actual
                 │
                 ▼
GET /api/GooglePlaces/search?query=OXXO&location=19.4326,-99.1332&radius=2000
    Authorization: Bearer eyJ...
                 │
                 ▼
Backend valida coordenadas y radio
                 │
                 ▼
Google retorna OXXOs en radio de 2km


┌─────────────────────────────────────────────────────────────┐
│                 CASO 3: Autocompletado                       │
└─────────────────────────────────────────────────────────────┘

Usuario escribe: "rest"
                 │
                 ▼
GET /api/GooglePlaces/autocomplete?input=rest
    Authorization: Bearer eyJ...
                 │
                 ▼
Muestra sugerencias:
  - Restaurante Los Arcos
  - Restaurant Bar El Patio
  - Rest & Relax Café
```

## Beneficios de la Solución

```
╔══════════════════════════════════════════════════════════════╗
║                          BENEFICIOS                           ║
╚══════════════════════════════════════════════════════════════╝

┌──────────────────┐     ┌──────────────────┐
│   SEGURIDAD      │     │   FUNCIONALIDAD  │
├──────────────────┤     ├──────────────────┤
│ ✅ API key       │     │ ✅ Búsquedas     │
│    protegida     │     │    funcionan     │
│ ✅ JWT auth      │     │ ✅ Autocompletado│
│ ✅ Validaciones  │     │    funciona      │
│ ✅ CodeQL: 0     │     │ ✅ Detalles de   │
│    alertas       │     │    lugar         │
└──────────────────┘     └──────────────────┘

┌──────────────────┐     ┌──────────────────┐
│   MANTENIMIENTO  │     │   ESCALABILIDAD  │
├──────────────────┤     ├──────────────────┤
│ ✅ Código limpio │     │ ✅ Centralizado  │
│ ✅ Sin duplicac. │     │ ✅ Fácil agregar │
│ ✅ Documentado   │     │    endpoints     │
│ ✅ Logging       │     │ ✅ Rate limiting │
│                  │     │    futuro        │
└──────────────────┘     └──────────────────┘
```
