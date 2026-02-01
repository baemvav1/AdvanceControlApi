# Reporte de Correcciones de Seguridad y Mejores Prácticas

**Fecha:** 2026-02-01  
**Revisión de:** AdvanceControlApi

---

## ✅ CORRECCIONES REALIZADAS (No afectan funcionalidad)

### 1. Eliminación de Código Obsoleto
- ✅ **Eliminado:** `WeatherForecastController.cs` - Controlador de muestra no utilizado
- ✅ **Eliminado:** `WeatherForecast.cs` - Clase modelo de muestra no utilizada
- **Razón:** Código de plantilla de Visual Studio que no tiene uso en la aplicación real
- **Impacto:** Reducción de superficie de ataque, código más limpio

### 2. Mejora en Manejo de Excepciones
- ✅ **Corregido:** 34 bloques `catch` vacíos en todos los servicios
- **Archivos afectados:**
  - `ClienteService.cs` (3 instancias)
  - `ProveedorService.cs` (3 instancias)
  - `OperacionService.cs` (1 instancia)
  - `CargoService.cs` (3 instancias)
  - `MantenimientoService.cs` (3 instancias)
  - `RelacionEquipoClienteService.cs` (3 instancias)
  - `RelacionProveedorRefaccionService.cs` (4 instancias)
  - `RelacionOperacionProveedorRefaccionService.cs` (3 instancias)
  - `EquipoService.cs` (2 instancias)
  - `RefaccionService.cs` (3 instancias)
  - `ServicioService.cs` (3 instancias)
  - `RelacionRefaccionEquipoService.cs` (3 instancias)

**Cambio realizado:**
```csharp
// ANTES:
catch
{
    // No es un mensaje de resultado, operación exitosa
}

// DESPUÉS:
catch (Exception ex)
{
    // No es un mensaje de resultado, operación exitosa
    _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
}
```

**Beneficios:**
- ✅ Mejor diagnóstico de problemas en producción
- ✅ Cumplimiento de mejores prácticas de C#
- ✅ Tipo de excepción específico (Exception) en lugar de catch genérico
- ✅ Logging de excepciones para debugging

---

## ⚠️ PROBLEMAS CRÍTICOS DE SEGURIDAD (REQUIEREN ACCIÓN INMEDIATA)

### 1. 🔴 CRÍTICO: Secretos Hardcodeados en appsettings.json

**Archivo:** `/AdvanceApi/appsettings.json`

**Secretos expuestos:**

```json
{
  "Jwt": {
    "Key": "TuClaveSuperSecreta_1234567890AB",  // ← EXPUESTO
  },
  "RefreshToken": {
    "Secret": "TuOtroSecretoParaHMAC_MuyLargo_Y_Secreto",  // ← EXPUESTO
  },
  "ConnectionStrings": {
    "DefaultConnection": "...User ID=AdvUser;Password=ZEda$^6sXdFGDAb9..."  // ← EXPUESTO
  },
  "GoogleMaps": {
    "ApiKey": "AIzaSyACdL7i17rQab0x_vaLoC_F263LOWuJcrQ"  // ← EXPUESTO
  }
}
```

**Riesgo:** Si este archivo está en control de versiones (Git), los secretos son visibles para cualquiera con acceso al repositorio.

**Acciones requeridas:**

#### Opción 1: User Secrets (Desarrollo) + Azure Key Vault (Producción)

**Paso 1 - Desarrollo:**
```bash
cd AdvanceApi
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "TuClaveSuperSecreta_1234567890AB"
dotnet user-secrets set "RefreshToken:Secret" "TuOtroSecretoParaHMAC_MuyLargo_Y_Secreto"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Password=ZEda$^6sXdFGDAb9;..."
dotnet user-secrets set "GoogleMaps:ApiKey" "AIzaSyACdL7i17rQab0x_vaLoC_F263LOWuJcrQ"
```

**Paso 2 - Actualizar appsettings.json:**
```json
{
  "Jwt": {
    "Key": "",  // ← Vacío, se carga de user secrets
    "Issuer": "AdvanceApi",
    "Audience": "AdvanceApiUsuarios",
    "AccessTokenMinutes": "60"
  },
  "RefreshToken": {
    "Secret": "",  // ← Vacío
    "Days": "30",
    "MaxPerUser": "10"
  },
  "ConnectionStrings": {
    "DefaultConnection": ""  // ← Vacío
  },
  "GoogleMaps": {
    "ApiKey": "",  // ← Vacío
    "DefaultCenter": "19.4326,-99.1332",
    "DefaultZoom": "15"
  }
}
```

**Paso 3 - Producción (Azure App Service):**
Agregar las siguientes variables de entorno en Azure Portal:
- `Jwt__Key`
- `RefreshToken__Secret`
- `ConnectionStrings__DefaultConnection`
- `GoogleMaps__ApiKey`

O usar Azure Key Vault:
```csharp
// En Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential()
);
```

#### Opción 2: Variables de Entorno

**Paso 1 - Configurar variables:**
```bash
export Jwt__Key="TuClaveSuperSecreta_1234567890AB"
export RefreshToken__Secret="TuOtroSecretoParaHMAC_MuyLargo_Y_Secreto"
export ConnectionStrings__DefaultConnection="Server=...;Password=..."
export GoogleMaps__ApiKey="AIzaSyACdL7i17rQab0x_vaLoC_F263LOWuJcrQ"
```

**Paso 2 - Vaciar appsettings.json** (igual que Opción 1)

---

### 2. 🔴 ALTO: Endpoint Expone Google Maps API Key

**Archivo:** `/AdvanceApi/Controllers/GoogleMapsConfigController.cs`  
**Líneas:** 24-48

**Código actual:**
```csharp
[HttpGet("api-key")]
[Authorize]
public IActionResult GetApiKey()
{
    var apiKey = _configuration["GoogleMaps:ApiKey"];
    return Ok(new { apiKey }); // ← EXPONE LA API KEY
}
```

**Problema:** Cualquier usuario autenticado puede obtener la API key completa.

**Acciones requeridas:**

#### Opción 1: Backend Proxy (RECOMENDADO)
Crear un endpoint que haga las llamadas a Google Maps desde el servidor:

```csharp
[HttpGet("geocode")]
[Authorize]
public async Task<IActionResult> Geocode([FromQuery] string address)
{
    var apiKey = _configuration["GoogleMaps:ApiKey"];
    var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";
    
    using var client = new HttpClient();
    var response = await client.GetAsync(url);
    var content = await response.Content.ReadAsStringAsync();
    
    return Content(content, "application/json");
}
```

#### Opción 2: Restricciones de API Key
Si el cliente necesita la API key:
1. En Google Cloud Console → API Key → Restricciones
2. Configurar "Restricciones de HTTP referrer"
3. Agregar dominios permitidos (ej: `https://tudominio.com/*`)
4. Limitar APIs permitidas solo a las necesarias

#### Opción 3: Eliminar el Endpoint
Si no se usa, eliminar completamente el método `GetApiKey()`.

---

### 3. 🟡 MEDIO: Falta Configuración CORS Explícita

**Archivo:** `/AdvanceApi/Program.cs`

**Problema actual:** No hay configuración CORS explícita. Aunque `AllowedHosts: "*"` es permisivo, CORS no está configurado.

**Acción requerida:**
```csharp
// En Program.cs, después de AddControllers()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy =>
    {
        policy.WithOrigins(
            "https://tudominio.com",
            "https://app.tudominio.com"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Después de app.UseAuthorization()
app.UseCors("AllowedOrigins");
```

**Para desarrollo:**
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseCors(policy => policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
}
else
{
    app.UseCors("AllowedOrigins");
}
```

---

## 📋 MEJORAS RECOMENDADAS (Prioridad Media)

### 4. Warnings del Compilador en AuthController

**Archivo:** `/AdvanceApi/Controllers/AuthController.cs`

**Warnings encontrados:**
```
AuthController.cs(110,33): warning CS0168: Variable 'sqlEx' declarada pero no usada
AuthController.cs(118,30): warning CS0168: Variable 'ex' declarada pero no usada
AuthController.cs(197,33): warning CS0168: Variable 'sqlEx' declarada pero no usada
AuthController.cs(205,30): warning CS0168: Variable 'ex' declarada pero no usada
AuthController.cs(262,30): warning CS0168: Variable 'ex' declarada pero no usada
AuthController.cs(298,33): warning CS0168: Variable 'sqlEx' declarada pero no usada
AuthController.cs(306,30): warning CS0168: Variable 'ex' declarada pero no usada
```

**Acción requerida:** Revisar si estas excepciones deben ser loggeadas o si se pueden suprimir con `_`:

```csharp
// ANTES:
catch (SqlException sqlEx)
{
    _logger.LogError("Error en la base de datos");
    // ...
}

// OPCIÓN 1 - Loggear la excepción:
catch (SqlException sqlEx)
{
    _logger.LogError(sqlEx, "Error en la base de datos");
    // ...
}

// OPCIÓN 2 - Suprimir si no se usa:
catch (SqlException)
{
    _logger.LogError("Error en la base de datos");
    // ...
}
```

---

### 5. OnlineController Minimalista

**Archivo:** `/AdvanceApi/Controllers/OnlineController.cs`

**Código actual:**
```csharp
[ApiController]
[Route("[controller]")]
public class OnlineController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "online" });
    }
}
```

**Recomendación:** Usar Health Checks oficiales de ASP.NET Core:

```csharp
// En Program.cs
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

// Después de UseAuthorization()
app.MapHealthChecks("/health");
```

Luego eliminar `OnlineController.cs`.

**Beneficios:**
- ✅ Checks automáticos de base de datos
- ✅ Formato estándar de health checks
- ✅ Integración con monitoring tools (Azure Monitor, Application Insights, etc.)

---

### 6. Inconsistencias de Nombrado de Rutas

**Archivos:** Varios controladores

**Problema:** Algunas rutas usan `[Route("api/[controller]")]` (PascalCase) y otras `[Route("api/proveedores")]` (lowercase).

**Ejemplos:**
- `ClientesController` → `/api/Clientes` (PascalCase)
- `ProveedoresController` → `/api/proveedores` (lowercase)

**Recomendación:** Estandarizar en kebab-case (lowercase):

```csharp
[Route("api/clientes")]
public class ClientesController : ControllerBase { }

[Route("api/proveedores")]
public class ProveedoresController : ControllerBase { }

[Route("api/mantenimiento")]
public class MantenimientoController : ControllerBase { }
```

**Razón:** 
- Convención REST estándar
- URLs más legibles
- Evita problemas case-sensitive en algunos clientes

---

### 7. Parámetros por Query String en lugar de Body

**Archivo:** `/AdvanceApi/Controllers/ClientesController.cs`  
**Líneas:** 36-44, 102-113, 179-191

**Problema actual:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateCliente(
    [FromQuery] int idUsuarioAct,
    [FromQuery] string? nombre,
    [FromQuery] string? telefono,
    [FromQuery] string? direccion,
    // ... 10+ parámetros más
)
```

**Problemas:**
- URLs muy largas (límite de ~2000 caracteres)
- Difícil de documentar en Swagger
- No es RESTful
- Difícil validar con Data Annotations

**Recomendación:** Usar DTOs con `[FromBody]`:

```csharp
// Crear DTO
public class ClienteCreateRequest
{
    [Required]
    public int IdUsuarioAct { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; }
    
    [Phone]
    public string? Telefono { get; set; }
    
    [StringLength(500)]
    public string? Direccion { get; set; }
    
    // ... resto de propiedades
}

// Usar en controller
[HttpPost]
public async Task<IActionResult> CreateCliente([FromBody] ClienteCreateRequest request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    // ...
}
```

**Beneficios:**
- ✅ Validación automática con Data Annotations
- ✅ Mejor documentación en Swagger
- ✅ Sin límites de URL
- ✅ Más RESTful

---

## 🔍 POSIBLES ERRORES DETECTADOS

### 8. Posible Typo en Nombre de Columna

**Archivo:** `/AdvanceApi/Services/ClienteService.cs`  
**Línea:** 78

**Código:**
```csharp
IdUsuarioAct = reader.IsDBNull(reader.GetOrdinal("id_usuaio_act")) 
    ? (int?)null 
    : reader.GetInt32(reader.GetOrdinal("id_usuaio_act"))
```

**Problema:** `id_usuaio_act` parece ser un typo de `id_usuario_act` (falta 'r' en 'usuario').

**NOTA IMPORTANTE:** Esta columna solo aparece una vez en todo el código. Si la base de datos realmente tiene el typo `id_usuaio_act`, entonces el código es correcto. Si la columna se llama `id_usuario_act`, entonces este es un bug que causa que el campo siempre sea NULL.

**Acción requerida:**
1. Verificar el nombre real de la columna en la base de datos:
   ```sql
   SELECT COLUMN_NAME 
   FROM INFORMATION_SCHEMA.COLUMNS 
   WHERE TABLE_NAME = 'Cliente' 
   AND COLUMN_NAME LIKE '%usuari%'
   ```

2. Si es un typo:
   - Si la columna en BD es `id_usuario_act`, corregir el código
   - Si la columna en BD es `id_usuaio_act`, considerar renombrar la columna en BD

---

## 📊 RESUMEN DE CORRECCIONES

| Categoría | Severidad | Realizadas | Pendientes | Total |
|-----------|-----------|------------|------------|-------|
| Código Obsoleto | 🟡 Baja | 2 | 0 | 2 |
| Manejo de Excepciones | 🟡 Media | 34 | 7 | 41 |
| Seguridad Crítica | 🔴 Alta | 0 | 4 | 4 |
| Mejores Prácticas | 🟡 Media | 0 | 4 | 4 |
| **TOTAL** | - | **36** | **15** | **51** |

---

## ✅ PRÓXIMOS PASOS RECOMENDADOS

### Inmediato (Antes de siguiente deployment):
1. ✅ Mover secretos de appsettings.json a User Secrets / Azure Key Vault
2. ✅ Revisar endpoint de Google Maps API Key (eliminar o proteger)
3. ✅ Agregar configuración CORS explícita
4. ✅ Verificar typo en nombre de columna `id_usuaio_act`

### Corto plazo (Próximas 2 semanas):
5. ✅ Corregir warnings del compilador en AuthController
6. ✅ Implementar Health Checks oficiales
7. ✅ Estandarizar nombrado de rutas
8. ✅ Migrar parámetros query string a DTOs con [FromBody]

### Largo plazo (Próximo mes):
9. ✅ Implementar rate limiting
10. ✅ Agregar middleware de logging de requests/responses
11. ✅ Configurar Application Insights para monitoreo
12. ✅ Agregar unit tests para servicios críticos

---

## 📝 NOTAS ADICIONALES

### Compilación Exitosa
✅ El proyecto compila correctamente después de las correcciones realizadas.
✅ Solo 7 warnings en `AuthController.cs` relacionados con variables no usadas.
✅ No se han introducido breaking changes.

### Testing
⚠️ No se encontraron pruebas unitarias en el proyecto.
📋 Recomendación: Agregar xUnit/NUnit tests para servicios críticos.

### Documentación
✅ El proyecto tiene varios archivos de documentación markdown.
✅ Se ha agregado este reporte de seguridad.

---

**Generado por:** GitHub Copilot Code Review  
**Fecha:** 2026-02-01  
**Estado:** CORRECCIONES SEGURAS APLICADAS - REQUIERE REVISIÓN MANUAL PARA TEMAS DE SEGURIDAD
