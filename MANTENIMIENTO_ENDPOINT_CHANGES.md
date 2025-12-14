# Cambios en los Endpoints de Mantenimiento

## Resumen
Se actualizaron los endpoints de mantenimiento para reflejar los cambios en el procedimiento almacenado `sp_MantenimientoEdit`, que ya no incluye referencias a `costo` y `costo total`.

## Endpoints Modificados

### 1. POST /api/Mantenimiento
**Archivo:** `AdvanceApi/Controllers/MantenimientoController.cs`

**Cambios:**
- Se eliminó el parámetro `costo` (tipo `double`) del endpoint
- Se eliminó la validación que verificaba que `costo > 0`
- El endpoint ahora solo requiere: `idTipoMantenimiento`, `idCliente`, `idEquipo` y opcionalmente `nota`

**Antes:**
```csharp
POST /api/Mantenimiento?idTipoMantenimiento=1&idCliente=10&idEquipo=5&costo=150.50&nota=ejemplo
```

**Ahora:**
```csharp
POST /api/Mantenimiento?idTipoMantenimiento=1&idCliente=10&idEquipo=5&nota=ejemplo
```

### 2. GET /api/Mantenimiento
**Archivo:** `AdvanceApi/Controllers/MantenimientoController.cs`

**Cambios:**
- El endpoint sigue funcionando igual (parámetros `identificador` e `idCliente`)
- La respuesta ya NO incluye los campos `costo` y `costoTotal` en el modelo

**Respuesta Antes:**
```json
[
  {
    "idMantenimiento": 1,
    "tipoMantenimiento": "Preventivo",
    "nombreComercial": "Empresa ABC",
    "razonSocial": "ABC SA de CV",
    "nota": "Mantenimiento programado",
    "identificador": "EQ-001",
    "costo": 150.50,
    "costoTotal": 200.00
  }
]
```

**Respuesta Ahora:**
```json
[
  {
    "idMantenimiento": 1,
    "tipoMantenimiento": "Preventivo",
    "nombreComercial": "Empresa ABC",
    "razonSocial": "ABC SA de CV",
    "nota": "Mantenimiento programado",
    "identificador": "EQ-001"
  }
]
```

### 3. DELETE /api/Mantenimiento
**Archivo:** `AdvanceApi/Controllers/MantenimientoController.cs`

**Cambios:**
- No se realizaron cambios en este endpoint
- Sigue funcionando con el parámetro `idMantenimiento`

## Archivos Modificados

1. **AdvanceApi/Controllers/MantenimientoController.cs**
   - Eliminado parámetro `costo` del método `CreateMantenimiento`
   - Eliminada validación de `costo > 0`

2. **AdvanceApi/Services/MantenimientoService.cs**
   - Eliminado parámetro `@costo` en las llamadas al procedimiento almacenado
   - Actualizado el método `GetMantenimientosAsync` para no leer las columnas 6 y 7 (Costo y CostoTotal)

3. **AdvanceApi/DTOs/MantenimientoQueryDto.cs**
   - Eliminada propiedad `Costo`

4. **AdvanceApi/Clases/Mantenimiento.cs**
   - Eliminadas propiedades `Costo` y `CostoTotal`

## Impacto en Clientes

Los clientes que consumen la API de Mantenimiento deben actualizar su código:

1. **Al crear mantenimientos (POST):** Ya no enviar el parámetro `costo`
2. **Al obtener mantenimientos (GET):** Ya no esperar los campos `costo` y `costoTotal` en la respuesta

## Compatibilidad con Stored Procedure

Estos cambios son compatibles con el nuevo procedimiento almacenado `sp_MantenimientoEdit` que:
- En operación `select`: Retorna 6 columnas (sin costo ni costoTotal)
- En operación `put`: No requiere el parámetro `@costo`
- En operación `delete`: No se vio afectada
