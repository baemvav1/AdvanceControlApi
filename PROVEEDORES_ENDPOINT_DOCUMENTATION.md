# Documentación del Endpoint `proveedores`

## Descripción General
El endpoint `proveedores` permite realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) sobre los proveedores del sistema. Este endpoint interactúa con el procedimiento almacenado `sp_proveedor_edit` de la base de datos.

## Endpoints

### 1. Obtener Proveedores (Select)

#### URL
```
GET /api/proveedores
```

#### Método HTTP
`GET`

#### Autenticación
**Requerida**: Sí  
**Tipo**: Bearer Token (JWT)

#### Parámetros

##### Headers
| Header | Tipo | Requerido | Descripción |
|--------|------|-----------|-------------|
| Authorization | string | Sí | Token JWT con formato "Bearer {token}" |

##### Query Parameters
| Parámetro | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| razonSocial | string | No | Búsqueda parcial por razón social |
| nombreComercial | string | No | Búsqueda parcial por nombre comercial |
| nota | string | No | Búsqueda parcial en notas |

#### Respuesta Exitosa

##### Código de Estado
`200 OK`

##### Ejemplo de Respuesta
```json
[
  {
    "idProveedor": 1,
    "rfc": "ABC123456789",
    "razonSocial": "Proveedores SA de CV",
    "nombreComercial": "Proveedores",
    "estatus": true,
    "nota": "Proveedor principal"
  },
  {
    "idProveedor": 2,
    "rfc": "XYZ987654321",
    "razonSocial": "Suministros SA de CV",
    "nombreComercial": "Suministros",
    "estatus": true,
    "nota": "Proveedor secundario"
  }
]
```

---

### 2. Crear Proveedor (Create)

#### URL
```
POST /api/proveedores
```

#### Método HTTP
`POST`

#### Autenticación
**Requerida**: Sí  
**Tipo**: Bearer Token (JWT)

#### Parámetros

##### Headers
| Header | Tipo | Requerido | Descripción |
|--------|------|-----------|-------------|
| Authorization | string | Sí | Token JWT con formato "Bearer {token}" |

##### Query Parameters
| Parámetro | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| rfc | string | Sí | RFC del proveedor |
| razonSocial | string | No | Razón social del proveedor |
| nombreComercial | string | No | Nombre comercial del proveedor |
| nota | string | No | Nota o comentario sobre el proveedor |

#### Respuesta Exitosa

##### Código de Estado
`200 OK`

##### Ejemplo de Respuesta
```json
{
  "success": true,
  "message": "Proveedor creado correctamente"
}
```

#### Respuestas de Error

##### Código de Estado: `400 Bad Request`
```json
{
  "message": "El campo 'rfc' es obligatorio."
}
```

##### Código de Estado: `500 Internal Server Error`
```json
{
  "message": "Error al acceder a la base de datos."
}
```

---

### 3. Actualizar Proveedor (Update)

#### URL
```
PUT /api/proveedores/{id}
```

#### Método HTTP
`PUT`

#### Autenticación
**Requerida**: Sí  
**Tipo**: Bearer Token (JWT)

#### Parámetros

##### Headers
| Header | Tipo | Requerido | Descripción |
|--------|------|-----------|-------------|
| Authorization | string | Sí | Token JWT con formato "Bearer {token}" |

##### Path Parameters
| Parámetro | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| id | int | Sí | ID del proveedor a actualizar |

##### Query Parameters
| Parámetro | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| rfc | string | No | Nuevo RFC del proveedor |
| razonSocial | string | No | Nueva razón social |
| nombreComercial | string | No | Nuevo nombre comercial |
| nota | string | No | Nueva nota |

**Nota**: Solo se actualizarán los campos proporcionados. Los campos no especificados mantendrán sus valores actuales.

#### Respuesta Exitosa

##### Código de Estado
`200 OK`

##### Ejemplo de Respuesta
```json
{
  "success": true,
  "message": "Proveedor actualizado correctamente"
}
```

#### Respuestas de Error

##### Código de Estado: `400 Bad Request`
```json
{
  "message": "Id Invalido"
}
```

##### Código de Estado: `500 Internal Server Error`
```json
{
  "message": "Error al acceder a la base de datos."
}
```

---

### 4. Eliminar Proveedor (Delete - Soft Delete)

#### URL
```
DELETE /api/proveedores/{id}
```

#### Método HTTP
`DELETE`

#### Autenticación
**Requerida**: Sí  
**Tipo**: Bearer Token (JWT)

#### Parámetros

##### Headers
| Header | Tipo | Requerido | Descripción |
|--------|------|-----------|-------------|
| Authorization | string | Sí | Token JWT con formato "Bearer {token}" |

##### Path Parameters
| Parámetro | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| id | int | Sí | ID del proveedor a eliminar |

**Nota**: Esta operación realiza un "soft delete", es decir, no elimina físicamente el registro sino que marca el campo `estatus` como 0 (inactivo).

#### Respuesta Exitosa

##### Código de Estado
`200 OK`

##### Ejemplo de Respuesta
```json
{
  "success": true,
  "message": "Proveedor eliminado correctamente"
}
```

#### Respuestas de Error

##### Código de Estado: `400 Bad Request`
```json
{
  "message": "Id Invalido"
}
```

##### Código de Estado: `500 Internal Server Error`
```json
{
  "message": "Error al acceder a la base de datos."
}
```

---

## Ejemplos de Uso

### Ejemplo 1: Obtener todos los proveedores activos
```bash
curl -X GET "https://api.example.com/api/proveedores" \
  -H "Authorization: Bearer {tu-token-jwt}"
```

### Ejemplo 2: Buscar proveedores por razón social
```bash
curl -X GET "https://api.example.com/api/proveedores?razonSocial=Proveedores" \
  -H "Authorization: Bearer {tu-token-jwt}"
```

### Ejemplo 3: Crear un nuevo proveedor
```bash
curl -X POST "https://api.example.com/api/proveedores?rfc=ABC123456789&razonSocial=Proveedores%20SA&nombreComercial=Proveedores&nota=Proveedor%20principal" \
  -H "Authorization: Bearer {tu-token-jwt}"
```

### Ejemplo 4: Actualizar un proveedor
```bash
curl -X PUT "https://api.example.com/api/proveedores/1?razonSocial=Nueva%20Razon%20Social" \
  -H "Authorization: Bearer {tu-token-jwt}"
```

### Ejemplo 5: Eliminar un proveedor
```bash
curl -X DELETE "https://api.example.com/api/proveedores/1" \
  -H "Authorization: Bearer {tu-token-jwt}"
```

---

## Modelo de Datos

### Proveedor
| Campo | Tipo | Descripción |
|-------|------|-------------|
| idProveedor | int | Identificador único del proveedor |
| rfc | string | RFC del proveedor |
| razonSocial | string | Razón social del proveedor |
| nombreComercial | string | Nombre comercial del proveedor |
| estatus | bool | Estado del proveedor (true = activo, false = inactivo) |
| nota | string | Notas o comentarios sobre el proveedor |

---

## Procedimiento Almacenado Relacionado

Este endpoint utiliza el procedimiento almacenado `sp_proveedor_edit` con los siguientes parámetros:

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| @operacion | nvarchar(100) | Tipo de operación: 'create', 'select', 'update', 'delete' |
| @idProveedor | int | ID del proveedor (requerido para update y delete) |
| @rfc | nvarchar(100) | RFC del proveedor |
| @razon_social | nvarchar(max) | Razón social del proveedor |
| @nombreComercial | int | Nombre comercial del proveedor |
| @estatus | bit | Estado del proveedor |
| @nota | nvarchar(max) | Notas del proveedor |

---

## Notas Importantes

1. Todos los endpoints requieren autenticación mediante Bearer Token (JWT).
2. Las operaciones de eliminación son "soft delete", no borran físicamente los registros.
3. Las búsquedas (select) son parciales y case-insensitive (utilizan LIKE '%valor%').
4. Los campos opcionales que no se proporcionen mantendrán sus valores actuales en operaciones de actualización.
5. Solo los proveedores con `estatus = 1` (activos) son retornados en las búsquedas.
