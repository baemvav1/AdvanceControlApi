# Implementación del Procedimiento sp_cliente_edit

## Resumen de Cambios

Este documento describe los cambios realizados para implementar el nuevo procedimiento almacenado `sp_cliente_edit` que reemplaza a `cliente_select` y añade funcionalidades completas de CRUD para el endpoint de clientes.

## Archivos Modificados

### 1. DTOs (Data Transfer Objects)

#### Nuevo: `AdvanceApi/DTOs/ClienteEditDto.cs`
- **Propósito**: DTO para manejar todas las operaciones del procedimiento `sp_cliente_edit`
- **Características**:
  - Soporta las 4 operaciones: 'create', 'select', 'update', 'delete'
  - Incluye todos los campos del modelo Cliente
  - Permite filtrado flexible para operaciones de búsqueda

#### Eliminado: `AdvanceApi/DTOs/ClienteQueryDto.cs`
- Este DTO antiguo solo soportaba la operación de consulta con el procedimiento `sp_cliente_select`
- Fue reemplazado por `ClienteEditDto` que es más completo

### 2. Services (Servicios)

#### Modificado: `AdvanceApi/Services/IClienteService.cs`
**Cambios realizados**:
- Actualizado el método `GetClientesAsync` para usar `ClienteEditDto` en lugar de `ClienteQueryDto`
- Agregados nuevos métodos:
  - `CreateClienteAsync`: Crea un nuevo cliente
  - `UpdateClienteAsync`: Actualiza un cliente existente
  - `DeleteClienteAsync`: Elimina (soft delete) un cliente

#### Modificado: `AdvanceApi/Services/ClienteService.cs`
**Cambios realizados**:
- **GetClientesAsync**: 
  - Actualizado para llamar a `sp_cliente_edit` con operación 'select'
  - Ahora soporta filtrado por todos los campos disponibles (rfc, razon_social, nombre_comercial, regimen_fiscal, uso_cfdi, notas, prioridad, credencial_id)
  - Reemplazó la llamada a `sp_cliente_select`

- **CreateClienteAsync** (nuevo):
  - Llama a `sp_cliente_edit` con operación 'create'
  - Maneja la respuesta del procedimiento que incluye el ID del cliente creado
  - Valida y retorna el resultado de la operación

- **UpdateClienteAsync** (nuevo):
  - Llama a `sp_cliente_edit` con operación 'update'
  - Actualiza solo los campos proporcionados (los valores NULL no se actualizan en la BD)
  - Valida que el ID del cliente sea válido

- **DeleteClienteAsync** (nuevo):
  - Llama a `sp_cliente_edit` con operación 'delete'
  - Realiza un soft delete (establece estatus = 0)
  - Registra el usuario que realizó la eliminación

### 3. Controllers (Controladores)

#### Modificado: `AdvanceApi/Controllers/ClientesController.cs`
**Cambios realizados**:

##### GET /api/Clientes
- **Antes**: Solo soportaba filtros básicos (search, rfc, notas, prioridad)
- **Ahora**: Soporta filtrado por:
  - rfc (búsqueda parcial)
  - razonSocial (búsqueda parcial)
  - nombreComercial (búsqueda parcial)
  - regimenFiscal (búsqueda parcial)
  - usoCfdi (búsqueda parcial)
  - notas (búsqueda parcial)
  - prioridad (coincidencia exacta)
  - credencialId (coincidencia exacta)

##### POST /api/Clientes (nuevo)
- **Propósito**: Crear un nuevo cliente
- **Parámetros requeridos**:
  - rfc (obligatorio)
- **Parámetros opcionales**:
  - razonSocial
  - nombreComercial
  - regimenFiscal
  - usoCfdi
  - diasCredito
  - limiteCredito
  - prioridad (por defecto: 1)
  - credencialId
  - notas
  - idUsuario
- **Respuesta**: Objeto con success, message y id_cliente

##### PUT /api/Clientes/{id} (nuevo)
- **Propósito**: Actualizar un cliente existente
- **Parámetros**:
  - id (en la ruta, requerido)
  - Todos los demás campos son opcionales
  - Solo los campos proporcionados se actualizan
- **Validación**: Verifica que el ID sea válido (> 0)
- **Respuesta**: Objeto con success y message

##### DELETE /api/Clientes/{id} (nuevo)
- **Propósito**: Eliminar (soft delete) un cliente
- **Parámetros**:
  - id (en la ruta, requerido)
  - idUsuario (query, opcional)
- **Validación**: Verifica que el ID sea válido (> 0)
- **Respuesta**: Objeto con success y message

## Compatibilidad con el Procedimiento Almacenado

El procedimiento `sp_cliente_edit` implementado en SQL Server tiene la siguiente estructura:

```sql
CREATE PROCEDURE [dbo].[sp_cliente_edit]
    @operacion              NVARCHAR(100),
    @id_cliente             INT = 0,
    @rfc                    NVARCHAR(15) = NULL,
    @razon_social           NVARCHAR(255) = NULL,
    @nombre_comercial       NVARCHAR(255) = NULL,
    @regimen_fiscal         NVARCHAR(10) = NULL,
    @uso_cfdi               NVARCHAR(10) = NULL,
    @dias_credito           INT = NULL,
    @limite_credito         DECIMAL(18, 2) = NULL,
    @prioridad              INT = 1,
    @estatus                BIT = 1,
    @credencial_id          INT = NULL,
    @notas                  VARCHAR(MAX) = NULL,
    @id_usuario             INT = NULL
```

Todas las llamadas desde la API están correctamente mapeadas a estos parámetros.

## Ejemplos de Uso

### Obtener todos los clientes activos
```http
GET /api/Clientes
Authorization: Bearer {token}
```

### Buscar clientes por RFC
```http
GET /api/Clientes?rfc=RFC123
Authorization: Bearer {token}
```

### Buscar clientes por múltiples criterios
```http
GET /api/Clientes?razonSocial=Empresa&prioridad=1
Authorization: Bearer {token}
```

### Crear un nuevo cliente
```http
POST /api/Clientes?rfc=RFC123456789&razonSocial=Mi%20Empresa%20S.A.&nombreComercial=Mi%20Empresa&regimenFiscal=601&usoCfdi=G01&diasCredito=30&limiteCredito=50000&prioridad=1&idUsuario=1
Authorization: Bearer {token}
```

### Actualizar un cliente
```http
PUT /api/Clientes/1?razonSocial=Nueva%20Razon%20Social&diasCredito=45&idUsuario=1
Authorization: Bearer {token}
```

### Eliminar un cliente
```http
DELETE /api/Clientes/1?idUsuario=1
Authorization: Bearer {token}
```

## Mejoras Implementadas

1. **CRUD Completo**: Ahora el endpoint de clientes soporta todas las operaciones CRUD
2. **Filtrado Mejorado**: La operación GET ahora soporta más filtros para búsquedas más precisas
3. **Consistencia**: El patrón de implementación es consistente con otros endpoints como `Proveedores`
4. **Validación**: Se agregaron validaciones apropiadas en el controlador (ID válido, RFC requerido, etc.)
5. **Manejo de Errores**: Respuestas apropiadas para errores de base de datos y validación
6. **Soft Delete**: Las eliminaciones son soft deletes, manteniendo los datos históricos
7. **Auditoría**: Se registra el usuario que realiza las operaciones de creación, actualización y eliminación

## Notas Técnicas

- El procedimiento almacenado usa `ISNULL` para actualizar solo los campos proporcionados
- Los parámetros NULL en el procedimiento almacenado se manejan correctamente con `DBNull.Value`
- El servicio retorna objetos anónimos con `success` y `message` para operaciones de modificación
- La operación `select` siempre retorna solo clientes con `estatus = 1` (activos)
- El campo `estatus` se establece a 0 en la operación `delete` (soft delete)

## Testing

Para probar los endpoints:

1. Obtener un token JWT válido del endpoint de autenticación
2. Usar Swagger UI en desarrollo (https://localhost:xxxx/swagger)
3. O usar herramientas como Postman/Insomnia con el token Bearer

## Seguridad

- Todos los endpoints requieren autenticación JWT
- El atributo `[Authorize]` está aplicado al controlador
- Los parámetros de entrada se validan antes de pasar al procedimiento almacenado
- Se usan comandos parametrizados SQL para prevenir inyección SQL
