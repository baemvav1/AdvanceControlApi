# Preguntas Frecuentes sobre Tokens JWT

## ¿El token contiene el nombre de usuario?

**Respuesta: SÍ**, el token JWT (Access Token) **SÍ contiene el nombre de usuario**.

El nombre de usuario se almacena en el claim `sub` (subject) del token JWT.

---

## ¿Qué información contiene el token?

El token JWT contiene los siguientes datos (llamados "claims"):

### Tabla de Claims

| Claim | Nombre Completo | Descripción | Valor de Ejemplo |
|-------|----------------|-------------|------------------|
| **`sub`** | **Subject** | **✅ Nombre de usuario** | `"usuario_ejemplo"` |
| `jti` | JWT ID | Identificador único del token | `"a1b2c3d4-e5f6-..."` |
| `iss` | Issuer | Emisor del token | `"AdvanceApi"` |
| `aud` | Audience | Audiencia del token | `"AdvanceApiUsuarios"` |
| `iat` | Issued At | Fecha de emisión (timestamp) | `1699564800` |
| `exp` | Expiration | Fecha de expiración (timestamp) | `1699568400` |

### Visualización del Token

Un token JWT tiene esta estructura:

```
Header.Payload.Signature
```

Ejemplo de **Payload decodificado**:

```json
{
  "sub": "usuario_ejemplo",           ← AQUÍ ESTÁ EL NOMBRE DE USUARIO
  "jti": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "iss": "AdvanceApi",
  "aud": "AdvanceApiUsuarios",
  "iat": 1699564800,
  "exp": 1699568400
}
```

---

## ¿Cómo puedo ver el contenido del token?

### Opción 1: Usar jwt.io (Online)

1. Ve a [https://jwt.io](https://jwt.io)
2. Pega tu token en la sección "Encoded"
3. El sitio automáticamente decodifica y muestra el contenido
4. Podrás ver el `sub` (username) y todos los demás claims

### Opción 2: Decodificar con JavaScript

```javascript
function decodificarToken(token) {
  // Separar el token en sus tres partes
  const partes = token.split('.');
  
  // Decodificar la parte del payload (segunda parte)
  const payloadBase64 = partes[1];
  const payloadJson = atob(payloadBase64.replace(/-/g, '+').replace(/_/g, '/'));
  const payload = JSON.parse(payloadJson);
  
  console.log("Username:", payload.sub);  // ← El nombre de usuario
  console.log("Expira:", new Date(payload.exp * 1000));
  console.log("Claims completos:", payload);
  
  return payload;
}

// Uso:
const miToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
const datos = decodificarToken(miToken);
console.log("El usuario es:", datos.sub);
```

### Opción 3: Usar el endpoint de validación

Puedes llamar al endpoint `/api/Auth/validate` que retorna todos los claims:

```bash
curl -X POST http://localhost:5000/api/Auth/validate \
  -H "Content-Type: application/json" \
  -d '{"token":"tu_token_aqui"}'
```

**Respuesta:**
```json
{
  "valid": true,
  "claims": {
    "sub": "usuario_ejemplo",     ← Aquí está el username
    "jti": "guid...",
    "iss": "AdvanceApi",
    "aud": "AdvanceApiUsuarios",
    "iat": "timestamp",
    "exp": "timestamp"
  }
}
```

---

## ¿Por qué el username está en el token?

El nombre de usuario se incluye en el token por varias razones:

1. **Autenticación Stateless**: El servidor puede identificar al usuario sin consultar la base de datos en cada petición
2. **Performance**: Validar el token es mucho más rápido que buscar en la base de datos
3. **Escalabilidad**: Varios servidores pueden validar el mismo token sin compartir estado
4. **Auditoría**: Cada petición puede registrarse con el username sin consultas adicionales

---

## ¿Es seguro que el username esté en el token?

✅ **SÍ, es seguro** siempre que:

1. **Uses HTTPS**: El token viaja encriptado por la red
2. **No expongas tokens**: No los guardes en lugares accesibles públicamente
3. **No incluyas información sensible**: El token NO debe contener:
   - ❌ Contraseñas
   - ❌ Números de tarjeta de crédito
   - ❌ Información personal sensible
   - ✅ Solo el username está bien (es información de identificación)

⚠️ **IMPORTANTE**: El token JWT **NO está encriptado**, solo está **firmado** digitalmente:
- ✅ **Firmado**: Nadie puede modificar el token sin invalidar la firma
- ❌ **No encriptado**: Cualquiera puede leer su contenido (por eso no poner datos sensibles)

---

## ¿Qué NO contiene el token?

El token JWT **NO contiene**:

- ❌ La contraseña del usuario
- ❌ El refresh token
- ❌ Información personal sensible
- ❌ Roles o permisos (en esta implementación básica)
- ❌ Información que cambia frecuentemente

Solo contiene lo mínimo necesario para identificar al usuario (`sub`) y validar el token (`jti`, `iss`, `aud`, `exp`).

---

## ¿Cómo usar el username del token en el backend?

Si un endpoint está protegido con `[Authorize]`, puedes extraer el username así:

```csharp
[Authorize]
[HttpGet("mi-perfil")]
public IActionResult GetMiPerfil()
{
    // Opción 1: Desde el claim "sub"
    var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
    
    // Opción 2: Desde User.Identity.Name
    var username2 = User.Identity?.Name;
    
    return Ok(new { 
        username,
        mensaje = $"Hola {username}"
    });
}
```

---

## ¿Puedo agregar más información al token?

**SÍ**, puedes agregar más claims al token modificando el método `GenerateJwtToken` en `AuthController.cs`:

```csharp
var claims = new[]
{
    new Claim(JwtRegisteredClaimNames.Sub, subject),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    new Claim("rol", "Administrador"),           // ← Claim personalizado
    new Claim("email", "usuario@ejemplo.com"),   // ← Otro claim personalizado
    new Claim("permisos", "lectura,escritura")   // ← Más claims
};
```

⚠️ **Recomendación**: No agregues demasiada información al token porque:
- El token viaja en cada petición HTTP (aumenta el tamaño de las peticiones)
- Cualquiera puede leer el contenido (no es seguro para datos sensibles)
- Si la información cambia, el token no se actualiza hasta que expire

---

## Resumen Ejecutivo

### Pregunta: ¿El nombre de usuario está en el token?
**✅ Respuesta: SÍ**, en el claim `sub` (subject)

### Pregunta: ¿Qué contiene el token?
**Respuesta:**
- ✅ `sub`: **Nombre de usuario** ← LO MÁS IMPORTANTE
- `jti`: ID único del token
- `iss`: Emisor ("AdvanceApi")
- `aud`: Audiencia ("AdvanceApiUsuarios")
- `iat`: Timestamp de emisión
- `exp`: Timestamp de expiración

### ¿Cómo verlo?
- **Online**: jwt.io
- **JavaScript**: Decodificar Base64 del payload
- **API**: Endpoint `POST /api/Auth/validate`

### ¿Es seguro?
- ✅ Sí, si usas HTTPS
- ✅ Sí, el token está firmado (no se puede modificar)
- ⚠️ No, el token no está encriptado (se puede leer)
- ❌ No incluyas información sensible

---

## Documentación Relacionada

Para más información, consulta:

- **Sistema completo**: [AUTHENTICATION_SYSTEM.md](./AUTHENTICATION_SYSTEM.md) - Sección "Estructura del Token JWT"
- **Guía de implementación**: [CLIENT_IMPLEMENTATION_GUIDE.md](./CLIENT_IMPLEMENTATION_GUIDE.md)
- **Análisis de seguridad**: [SECURITY_ANALYSIS.md](./SECURITY_ANALYSIS.md)

---

**Última actualización:** 2025-11-21
