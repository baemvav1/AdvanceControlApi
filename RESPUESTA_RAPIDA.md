# 🎯 Respuesta Rápida

## Tu Pregunta:
> "pregunta, dentro del token se encuentra el nombre de usuario? que contiene el token?"

---

## ✅ Respuesta Directa:

### 1. ¿El token contiene el nombre de usuario?
**SÍ** ✅

El nombre de usuario **SÍ está dentro del token JWT** en el campo `sub` (subject).

### 2. ¿Qué contiene el token?

El token contiene **6 datos principales**:

| # | Campo | Nombre | Contiene | Ejemplo |
|---|-------|--------|----------|---------|
| 1 | **`sub`** | **Subject** | **Tu nombre de usuario** | `"usuario_ejemplo"` |
| 2 | `jti` | JWT ID | ID único del token | `"a1b2c3d4-..."` |
| 3 | `iss` | Issuer | Quién emitió el token | `"AdvanceApi"` |
| 4 | `aud` | Audience | Para quién es el token | `"AdvanceApiUsuarios"` |
| 5 | `iat` | Issued At | Cuándo se creó | `1699564800` |
| 6 | `exp` | Expiration | Cuándo expira | `1699568400` |

---

## 📦 Visualización Simple

Tu token se ve así:

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c3VhcmlvIiwianRpIjoiLi4uIn0.firma
│                                      │                                      │
│        Header (cabecera)             │     Payload (tu información)         │  Signature (firma)
```

Cuando lo decodificas, el **Payload** contiene:

```json
{
  "sub": "tu_usuario_aqui",     ← AQUÍ ESTÁ TU NOMBRE DE USUARIO
  "jti": "id-unico-del-token",
  "iss": "AdvanceApi",
  "aud": "AdvanceApiUsuarios",
  "iat": 1699564800,
  "exp": 1699568400
}
```

---

## 🔍 ¿Cómo Puedo Verlo?

### Método 1: En línea (Más Fácil)
1. Ve a **https://jwt.io**
2. Pega tu token
3. ¡Listo! Verás todo el contenido, incluyendo tu username en `sub`

### Método 2: Con JavaScript
```javascript
// Copiar y pegar este código en la consola del navegador
const token = "pega_tu_token_aqui";
const payload = JSON.parse(atob(token.split('.')[1]));
console.log("Tu usuario es:", payload.sub);  // ← Aquí está tu username
```

### Método 3: Con la API
```bash
curl -X POST https://tu-api.com/api/Auth/validate \
  -H "Content-Type: application/json" \
  -d '{"token":"tu_token_aqui"}'
```

---

## ❓ Preguntas Frecuentes

### ¿Es seguro que mi username esté en el token?
✅ **Sí**, es seguro si usas HTTPS. El username no es información secreta.

### ¿La contraseña también está en el token?
❌ **No**, solo el username. La contraseña NUNCA está en el token.

### ¿Puedo modificar el token?
❌ **No**, está firmado digitalmente. Si lo modificas, deja de funcionar.

### ¿Cuánto dura el token?
⏱️ **60 minutos** por defecto, luego necesitas renovarlo con el refresh token.

---

## 📚 Documentación Completa

Para más detalles, consulta:

- **TOKEN_FAQ.md** → Preguntas y respuestas completas
- **AUTHENTICATION_SYSTEM.md** → Sistema completo de autenticación
- **AuthController.cs** → Código fuente con comentarios

---

## 📝 Resumen de Una Línea

**"Sí, el token contiene tu nombre de usuario en el campo `sub`, junto con un ID único (`jti`), emisor (`iss`), audiencia (`aud`), y fechas de emisión y expiración (`iat`, `exp`)."**

---

✅ **Pregunta respondida completamente**  
📅 **Actualizado:** 2025-11-21
