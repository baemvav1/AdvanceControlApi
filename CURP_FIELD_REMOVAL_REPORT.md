# Reporte de Eliminación del Campo CURP

## Resumen
El campo `curp` ha sido eliminado de la base de datos y se han actualizado todos los componentes del API que lo utilizaban.

## Archivos Modificados

### 1. DTOs
- **AdvanceApi/DTOs/ClienteQueryDto.cs**
  - ❌ Eliminada la propiedad `Curp` (línea 18-21)
  - ❌ Eliminado el comentario de documentación XML para CURP

### 2. Controladores
- **AdvanceApi/Controllers/ClientesController.cs**
  - ❌ Eliminado el parámetro `curp` del método `GetClientes` (línea 28, 36)
  - ❌ Eliminado el comentario de documentación XML para el parámetro curp (línea 28)
  - ❌ Eliminada la asignación `Curp = curp` en la creación del DTO (línea 46)

### 3. Servicios
- **AdvanceApi/Services/ClienteService.cs**
  - ❌ Eliminada la línea que agrega el parámetro `@curp` al comando SQL (línea 44)

## Endpoints Afectados

### GET /api/Clientes
**Ruta completa:** `GET /api/Clientes`

**Parámetros eliminados:**
- `curp` (query parameter) - Búsqueda parcial por CURP

**Parámetros actuales (después del cambio):**
- `search` (opcional) - Búsqueda en razon_social o nombre_comercial
- `rfc` (opcional) - Búsqueda parcial por RFC
- `notas` (opcional) - Búsqueda parcial en notas
- `prioridad` (opcional) - Coincidencia exacta de prioridad

**Ejemplo de uso anterior:**
```
GET /api/Clientes?curp=ABCD123456
```

**Ejemplo de uso actual:**
```
GET /api/Clientes?rfc=ABC123&search=ejemplo
```

## Procedimientos Almacenados que Requieren Modificación

### sp_cliente_select
**Ubicación:** Base de datos (no incluido en el repositorio)

**Modificaciones requeridas:**
1. ✅ Eliminar el parámetro `@curp` de la declaración del procedimiento
2. ✅ Eliminar cualquier referencia al parámetro `@curp` en las cláusulas WHERE
3. ✅ Eliminar cualquier lógica que utilice el campo `curp` de la tabla `clientes`

**Ejemplo de modificación necesaria:**

**Antes:**
```sql
CREATE PROCEDURE sp_cliente_select
    @search NVARCHAR(255) = NULL,
    @rfc NVARCHAR(13) = NULL,
    @curp NVARCHAR(18) = NULL,  -- <- ELIMINAR
    @notas NVARCHAR(MAX) = NULL,
    @prioridad INT = NULL
AS
BEGIN
    SELECT * FROM clientes
    WHERE (@search IS NULL OR razon_social LIKE '%' + @search + '%' OR nombre_comercial LIKE '%' + @search + '%')
        AND (@rfc IS NULL OR rfc LIKE '%' + @rfc + '%')
        AND (@curp IS NULL OR curp LIKE '%' + @curp + '%')  -- <- ELIMINAR
        AND (@notas IS NULL OR notas LIKE '%' + @notas + '%')
        AND (@prioridad IS NULL OR prioridad = @prioridad)
END
```

**Después:**
```sql
CREATE PROCEDURE sp_cliente_select
    @search NVARCHAR(255) = NULL,
    @rfc NVARCHAR(13) = NULL,
    @notas NVARCHAR(MAX) = NULL,
    @prioridad INT = NULL
AS
BEGIN
    SELECT * FROM clientes
    WHERE (@search IS NULL OR razon_social LIKE '%' + @search + '%' OR nombre_comercial LIKE '%' + @search + '%')
        AND (@rfc IS NULL OR rfc LIKE '%' + @rfc + '%')
        AND (@notas IS NULL OR notas LIKE '%' + @notas + '%')
        AND (@prioridad IS NULL OR prioridad = @prioridad)
END
```

## Impacto

### Impacto en el API
- ✅ El endpoint `GET /api/Clientes` ya no acepta el parámetro `curp`
- ✅ Las aplicaciones cliente que usaban el parámetro `curp` deberán actualizarse
- ✅ El API continuará funcionando normalmente con los demás parámetros de búsqueda

### Impacto en la Base de Datos
- ⚠️ **IMPORTANTE:** El procedimiento almacenado `sp_cliente_select` debe modificarse en la base de datos para eliminar el parámetro `@curp`
- ⚠️ Si el procedimiento no se actualiza, el API funcionará pero enviará un parámetro que el procedimiento no espera (esto podría causar errores dependiendo de la configuración de SQL Server)

## Estado de Implementación

- ✅ Código del API actualizado
- ✅ DTOs actualizados
- ✅ Controladores actualizados
- ✅ Servicios actualizados
- ✅ Compilación exitosa
- ⚠️ **PENDIENTE:** Actualizar procedimiento almacenado `sp_cliente_select` en la base de datos

## Recomendaciones

1. **Actualizar el procedimiento almacenado** `sp_cliente_select` en la base de datos siguiendo las modificaciones indicadas anteriormente
2. **Notificar a los equipos de frontend** sobre la eliminación del parámetro `curp` del endpoint
3. **Revisar la documentación del API** (Swagger/OpenAPI) para asegurar que refleje los cambios
4. **Verificar que no existan otros procedimientos almacenados** que utilicen el campo `curp` de la tabla `clientes`

## Verificación

Para verificar que los cambios funcionan correctamente:

1. Actualizar el procedimiento almacenado en la base de datos
2. Ejecutar el API
3. Probar el endpoint: `GET /api/Clientes?search=test&rfc=ABC`
4. Verificar que no se produzcan errores relacionados con el parámetro `curp`

## Fecha de Implementación
2025-12-14
