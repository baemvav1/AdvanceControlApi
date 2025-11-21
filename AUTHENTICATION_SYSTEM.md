# Sistema de Autenticación - AdvanceControlApi

## Descripción General

El sistema de autenticación implementa un flujo completo de JWT (JSON Web Tokens) con refresh tokens para proporcionar autenticación segura y persistente. Utiliza una arquitectura de tokens rotativos para máxima seguridad.

## Arquitectura

### Componentes Principales

1. **AuthController** (`/AdvanceApi/Controllers/AuthController.cs`)
   - Maneja todos los endpoints de autenticación
   - Genera y valida tokens JWT
   - Gestiona el ciclo de vida de refresh tokens

2. **DbHelper** (`/AdvanceApi/Helpers/DbHelper.cs`)
   - Proporciona acceso a la base de datos
   - Gestiona operaciones de refresh tokens en SQL Server

3. **Usuario** (`/AdvanceApi/Clases/usuario.cs`)
   - Modelo de datos para credenciales de login
   - Incluye validaciones integradas

## Estructura del Token JWT (Access Token)

### ¿Qué contiene el token JWT?

**Respuesta corta:** Sí, el token JWT **contiene el nombre de usuario** junto con otra información de autenticación.

El Access Token es un JWT (JSON Web Token) que contiene los siguientes **claims** (datos):

| Claim | Nombre Completo | Descripción | Ejemplo |
|-------|----------------|-------------|---------|
| `sub` | Subject | **Nombre de usuario** del usuario autenticado | `"usuario_ejemplo"` |
| `jti` | JWT ID | Identificador único del token (GUID) | `"a1b2c3d4-e5f6-7890-abcd-ef1234567890"` |
| `iss` | Issuer | Emisor del token | `"AdvanceApi"` |
| `aud` | Audience | Audiencia/destinatario del token | `"AdvanceApiUsuarios"` |
| `iat` | Issued At | Fecha/hora de emisión (timestamp UTC) | `1699564800` |
| `exp` | Expiration | Fecha/hora de expiración (timestamp UTC) | `1699568400` |

### Estructura Visual del Token

Un token JWT tiene tres partes separadas por puntos:

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c3VhcmlvX2VqZW1wbG8iLCJqdGkiOiJhMWIyYzNkNC1lNWY2LTc4OTAtYWJjZC1lZjEyMzQ1Njc4OTAiLCJpc3MiOiJBZHZhbmNlQXBpIiwiYXVkIjoiQWR2YW5jZUFwaVVzdWFyaW9zIiwiaWF0IjoxNjk5NTY0ODAwLCJleHAiOjE2OTk1Njg0MDB9.firma_hmac_sha256
│                                  │                                                                                                                                                                              │
│         Header (Base64)          │                                                          Payload (Base64)                                                                                                     │     Signature
```

1. **Header**: Algoritmo y tipo de token
   ```json
   {
     "alg": "HS256",
     "typ": "JWT"
   }
   ```

2. **Payload**: Los claims mencionados arriba
   ```json
   {
     "sub": "usuario_ejemplo",
     "jti": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
     "iss": "AdvanceApi",
     "aud": "AdvanceApiUsuarios",
     "iat": 1699564800,
     "exp": 1699568400
   }
   ```

3. **Signature**: Firma HMAC-SHA256 para verificar autenticidad

### ¿Cómo extraer el nombre de usuario del token?

#### Desde el Cliente (JavaScript)

```javascript
// Decodificar el token (sin verificar - solo lectura)
function decodeJwt(token) {
  const base64Url = token.split('.')[1];
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
    return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
  }).join(''));
  
  return JSON.parse(jsonPayload);
}

const token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
const payload = decodeJwt(token);
console.log(payload.sub); // "usuario_ejemplo" ← Aquí está el nombre de usuario
console.log(payload.jti); // ID único del token
console.log(payload.exp); // Timestamp de expiración
```

#### Desde el Backend (C#)

El sistema puede extraer el username automáticamente de los endpoints protegidos:

```csharp
[Authorize]
[HttpGet("mi-perfil")]
public IActionResult GetMiPerfil()
{
    // El username viene en el claim "sub" del token validado
    // Se intenta primero con ClaimTypes.NameIdentifier, luego con JwtRegisteredClaimNames.Sub
    var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    
    // Alternativa: User.Identity.Name (puede no estar configurado en todos los casos)
    // var username = User.Identity?.Name;
    
    return Ok(new { username });
}
```

### Importante: Seguridad

- ✅ **El token está firmado**: No se puede modificar sin invalidar la firma
- ✅ **El token tiene expiración**: Válido por 60 minutos por defecto
- ⚠️ **El token NO está encriptado**: Cualquiera puede leer su contenido
  - No incluir información sensible (contraseñas, números de tarjeta, etc.)
  - Solo incluir información necesaria para identificar al usuario
- ✅ **Usar siempre HTTPS**: Para evitar que intercepten el token

### ¿Por qué incluir el username en el token?

1. **Stateless Authentication**: El servidor no necesita consultar la base de datos en cada petición para saber quién es el usuario
2. **Performance**: Validar el token es mucho más rápido que consultar la base de datos
3. **Escalabilidad**: Los tokens pueden validarse en múltiples servidores sin compartir estado
4. **Auditoría**: Cada petición puede registrarse con el username del claim sin consultas adicionales

---

## Flujo de Autenticación

### 1. Login (Inicio de Sesión)

**Endpoint:** `POST /api/Auth/login`

**Request Body:**
```json
{
  "username": "usuario_ejemplo",
  "password": "contraseña_segura"
}
```

**Validaciones:**
- Username: obligatorio, 3-150 caracteres
- Password: obligatorio, 4-100 caracteres

**Response Exitoso (200):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64_encoded_random_token...",
  "expiresIn": 3600,
  "tokenType": "Bearer",
  "user": {
    "username": "usuario_ejemplo"
  }
}
```

**Proceso Interno:**
1. Valida que las credenciales no estén vacías
2. Llama al stored procedure `login_credencial` con usuario y contraseña
3. Si las credenciales son válidas:
   - Genera un Access Token JWT válido por 60 minutos (configurable)
   - Genera un Refresh Token aleatorio de 64 bytes
   - Hashea el Refresh Token con HMAC-SHA256
   - Almacena el hash en la base de datos con metadatos (IP, User-Agent, fecha de expiración)
4. Retorna ambos tokens al cliente

**Errores Posibles:**
- `400 Bad Request`: Datos inválidos o faltantes
- `401 Unauthorized`: Credenciales incorrectas
- `500 Internal Server Error`: Error de servidor o base de datos

---

### 2. Refresh (Renovación de Token)

**Endpoint:** `POST /api/Auth/refresh`

**Request Body:**
```json
{
  "refreshToken": "base64_encoded_refresh_token..."
}
```

**Response Exitoso (200):**
```json
{
  "accessToken": "nuevo_jwt_token...",
  "refreshToken": "nuevo_refresh_token...",
  "expiresIn": 3600,
  "tokenType": "Bearer",
  "user": {
    "username": "usuario_ejemplo"
  }
}
```

**Proceso Interno (Rotación de Tokens):**
1. Hashea el refresh token recibido
2. Busca el hash en la base de datos
3. Validaciones:
   - El token existe
   - No ha sido revocado
   - No ha expirado
4. **Rotación:** 
   - Genera un nuevo refresh token
   - Revoca el token antiguo marcando `ReplacedByTokenHash`
   - Inserta el nuevo token en la base de datos
5. Genera un nuevo access token JWT
6. Retorna los nuevos tokens

**Seguridad - Detección de Reuso:**
Si se detecta que un refresh token revocado está siendo reutilizado:
- Se asume que hubo un compromiso de seguridad
- Se revocan TODOS los refresh tokens del usuario
- Se retorna error 401

**Errores Posibles:**
- `400 Bad Request`: Refresh token no proporcionado
- `401 Unauthorized`: Token inválido, expirado, revocado o reusado
- `500 Internal Server Error`: Error de servidor

---

### 3. Validate (Validación de Token)

**Endpoint:** `POST /api/Auth/validate`

**Request Body (Opcional):**
```json
{
  "token": "jwt_token_opcional..."
}
```

**O usar Header:**
```
Authorization: Bearer jwt_token...
```

**Response Exitoso (200):**
```json
{
  "valid": true,
  "claims": {
    "sub": "usuario_ejemplo",
    "jti": "guid_unico",
    "iat": "timestamp",
    "exp": "timestamp",
    "iss": "AdvanceApi",
    "aud": "AdvanceApiUsuarios"
  }
}
```

**Proceso Interno:**
1. Extrae el token del body o del header Authorization
2. Valida el token JWT:
   - Firma válida
   - No expirado
   - Issuer correcto
   - Audience correcta
3. Retorna los claims si es válido

**Errores Posibles:**
- `400 Bad Request`: Token no proporcionado
- `401 Unauthorized`: Token inválido o expirado
- `500 Internal Server Error`: Error de procesamiento

---

### 4. Logout (Cerrar Sesión)

**Endpoint:** `POST /api/Auth/logout`

**Request Body:**
```json
{
  "refreshToken": "refresh_token_a_revocar..."
}
```

**Response Exitoso:**
- `204 No Content`

**Proceso Interno:**
1. Hashea el refresh token recibido
2. Busca el token en la base de datos
3. Si existe, lo revoca (marca como `Revoked = true`)
4. Operación idempotente: si el token no existe, también retorna 204

**Nota:** El access token seguirá siendo válido hasta su expiración natural. Para seguridad adicional, implementar una lista negra de tokens o reducir el tiempo de expiración.

---

## Configuración

### appsettings.json

```json
{
  "Jwt": {
    "Key": "TuClaveSuperSecreta_1234567890AB",
    "Issuer": "AdvanceApi",
    "Audience": "AdvanceApiUsuarios",
    "AccessTokenMinutes": "60"
  },
  "RefreshToken": {
    "Secret": "TuOtroSecretoParaHMAC_MuyLargo_Y_Secreto",
    "Days": "30",
    "MaxPerUser": "10"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;..."
  }
}
```

**Parámetros:**
- `Jwt:Key`: Clave secreta para firmar JWT (mínimo 32 caracteres)
- `Jwt:Issuer`: Emisor del token
- `Jwt:Audience`: Audiencia del token
- `Jwt:AccessTokenMinutes`: Duración del access token (default: 60)
- `RefreshToken:Secret`: Clave para HMAC de refresh tokens
- `RefreshToken:Days`: Duración del refresh token (default: 30)
- `RefreshToken:MaxPerUser`: Límite de tokens activos por usuario (no implementado aún)

---

## Base de Datos

### Stored Procedures Requeridos

#### 1. login_credencial
```sql
CREATE PROCEDURE [dbo].[login_credencial]
    @usuario NVARCHAR(150),
    @contraseña NVARCHAR(100)
AS
BEGIN
    -- Debe retornar 1 (o true) si las credenciales son válidas
    -- Debe retornar 0 (o false) si son inválidas
    -- Implementar validación de contraseña (idealmente con hash)
END
```

#### 2. InsertRefreshToken
```sql
CREATE PROCEDURE [dbo].[InsertRefreshToken]
    @Usuario NVARCHAR(150),
    @TokenHash NVARCHAR(200),
    @ExpiresAt DATETIME2,
    @IpAddress NVARCHAR(50) = NULL,
    @UserAgent NVARCHAR(1000) = NULL
AS
BEGIN
    -- Inserta un nuevo refresh token
    -- Debe retornar el ID generado como 'NewId'
END
```

#### 3. GetRefreshTokenByHash
```sql
CREATE PROCEDURE [dbo].[GetRefreshTokenByHash]
    @TokenHash NVARCHAR(200)
AS
BEGIN
    -- Retorna el registro del refresh token que coincide con el hash
    -- Columnas: Id, Usuario, TokenHash, CreatedAt, ExpiresAt, Revoked,
    --           ReplacedByTokenHash, IpAddress, UserAgent
END
```

#### 4. RevokeRefreshTokenById
```sql
CREATE PROCEDURE [dbo].[RevokeRefreshTokenById]
    @Id BIGINT,
    @ReplacedByTokenHash NVARCHAR(200) = NULL
AS
BEGIN
    -- Marca el token como revocado
    -- Opcionalmente registra qué token lo reemplazó
END
```

#### 5. RevokeAllRefreshTokensForUser
```sql
CREATE PROCEDURE [dbo].[RevokeAllRefreshTokensForUser]
    @Usuario NVARCHAR(150)
AS
BEGIN
    -- Revoca todos los refresh tokens activos del usuario
    -- Se usa cuando se detecta reuso de token
END
```

#### 6. CountActiveRefreshTokensForUser
```sql
CREATE PROCEDURE [dbo].[CountActiveRefreshTokensForUser]
    @Usuario NVARCHAR(150)
AS
BEGIN
    -- Retorna la cantidad de tokens activos del usuario
    -- Útil para implementar límite de sesiones
END
```

### Tabla RefreshTokens (Ejemplo)

```sql
CREATE TABLE [dbo].[RefreshTokens] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [Usuario] NVARCHAR(150) NOT NULL,
    [TokenHash] NVARCHAR(200) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ExpiresAt] DATETIME2 NOT NULL,
    [Revoked] BIT NOT NULL DEFAULT 0,
    [ReplacedByTokenHash] NVARCHAR(200) NULL,
    [IpAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(1000) NULL,
    INDEX IX_TokenHash (TokenHash),
    INDEX IX_Usuario_Revoked (Usuario, Revoked)
)
```

---

## Seguridad

### Características Implementadas

1. **Tokens JWT Firmados:**
   - Usa HMAC-SHA256 para firmar tokens
   - Validación de emisor, audiencia y expiración

2. **Refresh Token Rotation:**
   - Cada vez que se usa un refresh token, se genera uno nuevo
   - El antiguo se revoca automáticamente
   - Previene reuso de tokens robados

3. **HMAC de Refresh Tokens:**
   - Los refresh tokens nunca se almacenan en texto plano
   - Se usa HMAC-SHA256 con una clave secreta

4. **Detección de Reuso:**
   - Si se detecta un token revocado siendo usado
   - Se revocan todas las sesiones del usuario

5. **Metadatos de Sesión:**
   - Se registra IP y User-Agent
   - Permite auditoría de sesiones

6. **Validación de Entrada:**
   - Atributos de validación en modelos
   - Validación de longitud de credenciales

### Mejores Prácticas Recomendadas

1. **Claves Secretas:**
   - Usar variables de entorno en producción
   - Claves de mínimo 32 caracteres
   - Rotar claves periódicamente

2. **HTTPS:**
   - Siempre usar HTTPS en producción
   - Los tokens son sensibles y no deben viajar en texto plano

3. **Almacenamiento en Cliente:**
   - Access Token: memoria (no localStorage)
   - Refresh Token: httpOnly cookie o almacenamiento seguro

4. **Duración de Tokens:**
   - Access Token: corto (15-60 minutos)
   - Refresh Token: largo (7-30 días)

5. **Rate Limiting:**
   - Implementar límites de intentos de login
   - Prevenir ataques de fuerza bruta

---

## Uso con Clientes

### Ejemplo: Login y Uso del Token

```javascript
// 1. Login
const loginResponse = await fetch('/api/Auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'usuario',
    password: 'contraseña'
  })
});

const { accessToken, refreshToken } = await loginResponse.json();

// 2. Usar el access token en peticiones
const dataResponse = await fetch('/api/Clientes', {
  headers: {
    'Authorization': `Bearer ${accessToken}`
  }
});

// 3. Renovar cuando expire (refresh)
const refreshResponse = await fetch('/api/Auth/refresh', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ refreshToken })
});

const { accessToken: newAccessToken, refreshToken: newRefreshToken } = 
  await refreshResponse.json();

// 4. Logout
await fetch('/api/Auth/logout', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ refreshToken })
});
```

### Manejo de Expiración Automática

```javascript
class AuthClient {
  constructor() {
    this.accessToken = null;
    this.refreshToken = null;
  }

  async login(username, password) {
    const response = await fetch('/api/Auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password })
    });
    
    if (response.ok) {
      const data = await response.json();
      this.accessToken = data.accessToken;
      this.refreshToken = data.refreshToken;
      return true;
    }
    return false;
  }

  async fetchWithAuth(url, options = {}) {
    // Intenta con el token actual
    let response = await fetch(url, {
      ...options,
      headers: {
        ...options.headers,
        'Authorization': `Bearer ${this.accessToken}`
      }
    });

    // Si es 401, intenta refresh
    if (response.status === 401) {
      const refreshed = await this.refresh();
      if (refreshed) {
        // Reintenta la petición original
        response = await fetch(url, {
          ...options,
          headers: {
            ...options.headers,
            'Authorization': `Bearer ${this.accessToken}`
          }
        });
      }
    }

    return response;
  }

  async refresh() {
    try {
      const response = await fetch('/api/Auth/refresh', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken: this.refreshToken })
      });

      if (response.ok) {
        const data = await response.json();
        this.accessToken = data.accessToken;
        this.refreshToken = data.refreshToken;
        return true;
      }
    } catch (error) {
      console.error('Error al refrescar token:', error);
    }
    return false;
  }

  async logout() {
    await fetch('/api/Auth/logout', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: this.refreshToken })
    });
    
    this.accessToken = null;
    this.refreshToken = null;
  }
}
```

---

## Testing

### Probar con Swagger

1. Navegar a `/swagger` en desarrollo
2. Usar el endpoint `POST /api/Auth/login` con credenciales válidas
3. Copiar el `accessToken` de la respuesta
4. Hacer clic en el botón "Authorize" en Swagger
5. Introducir: `Bearer {accessToken}`
6. Ahora puedes probar endpoints protegidos como `/api/Clientes`

### Probar con cURL

```bash
# Login
curl -X POST http://localhost:5000/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"usuario","password":"contraseña"}'

# Usar token en petición protegida
curl -X GET http://localhost:5000/api/Clientes \
  -H "Authorization: Bearer {access_token}"

# Refresh
curl -X POST http://localhost:5000/api/Auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"{refresh_token}"}'

# Logout
curl -X POST http://localhost:5000/api/Auth/logout \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"{refresh_token}"}'
```

---

## Troubleshooting

### Error: "Token inválido o expirado"
- Verificar que el token no haya expirado (por defecto 60 minutos)
- Usar el endpoint `/api/Auth/refresh` para obtener un nuevo token
- Verificar que la clave secreta sea la misma en configuración

### Error: "Credenciales inválidas"
- Verificar que el usuario exista en la base de datos
- Verificar que el stored procedure `login_credencial` esté implementado correctamente
- Revisar logs de base de datos para errores

### Error: "Refresh token revocado"
- El token puede haber sido usado múltiples veces
- Por seguridad, todas las sesiones fueron revocadas
- Hacer login nuevamente

### Error: "No se encontró la configuración"
- Verificar que `appsettings.json` tenga todas las claves necesarias
- Verificar que `ConnectionStrings:DefaultConnection` esté configurado

---

## Mantenimiento

### Limpieza de Tokens Expirados

Recomendado: Crear un job que periódicamente elimine tokens expirados:

```sql
-- Eliminar tokens expirados hace más de 7 días
DELETE FROM RefreshTokens 
WHERE ExpiresAt < DATEADD(day, -7, GETUTCDATE())
```

### Monitoreo

Consultas útiles para monitorear el sistema:

```sql
-- Tokens activos por usuario
SELECT Usuario, COUNT(*) as TokensActivos
FROM RefreshTokens
WHERE Revoked = 0 AND ExpiresAt > GETUTCDATE()
GROUP BY Usuario

-- Tokens creados hoy
SELECT COUNT(*) as TokensHoy
FROM RefreshTokens
WHERE CAST(CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE)

-- Sesiones revocadas por reuso (últimos 7 días)
SELECT Usuario, COUNT(*) as RevocacionesPorReuso
FROM RefreshTokens
WHERE Revoked = 1 
  AND ReplacedByTokenHash IS NOT NULL
  AND CreatedAt > DATEADD(day, -7, GETUTCDATE())
GROUP BY Usuario
```

---

## Resumen para Implementadores

### Pasos para Implementar/Confirmar el Sistema

1. **Base de Datos:**
   - Crear tabla `RefreshTokens`
   - Implementar los 6 stored procedures requeridos
   - Verificar que `login_credencial` valide credenciales correctamente

2. **Configuración:**
   - Configurar `appsettings.json` con claves secretas seguras
   - En producción, usar variables de entorno

3. **Testing:**
   - Probar login con credenciales válidas e inválidas
   - Verificar que se generen ambos tokens
   - Probar refresh token rotation
   - Verificar que logout revoque tokens
   - Probar detección de reuso de tokens

4. **Integración Cliente:**
   - Implementar lógica de login
   - Almacenar tokens de forma segura
   - Implementar refresh automático
   - Implementar logout

5. **Seguridad:**
   - Verificar que HTTPS esté activo
   - Implementar rate limiting
   - Configurar CORS apropiadamente
   - Rotar claves secretas periódicamente

### Checklist de Validación

- [ ] Stored procedures creados y funcionando
- [ ] Login retorna ambos tokens correctamente
- [ ] Access token permite acceso a endpoints protegidos
- [ ] Refresh rota tokens correctamente
- [ ] Logout revoca tokens
- [ ] Detección de reuso funciona
- [ ] Validación de entrada funciona
- [ ] Errores retornan mensajes apropiados
- [ ] HTTPS configurado en producción
- [ ] Claves secretas seguras y en variables de entorno

---

## Contacto y Soporte

Para preguntas o reportar problemas con el sistema de autenticación, contactar al equipo de desarrollo.

**Última actualización:** 2025-11-10
