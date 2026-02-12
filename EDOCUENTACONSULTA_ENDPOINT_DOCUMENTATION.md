# Endpoint edoCuentaConsulta - Documentación

## Descripción General

Se ha implementado el endpoint `edoCuentaConsulta` que consume el procedimiento almacenado `sp_ConsultarEstadoCuentaCompleto` para obtener la información completa de un estado de cuenta incluyendo todos sus datos relacionados.

## Endpoint

### GET `/api/estadocuenta/edoCuentaConsulta`

**Descripción:** Consulta el estado de cuenta completo con todos sus movimientos, transferencias SPEI, comisiones e impuestos asociados.

**Autenticación:** Requiere JWT Token (endpoint protegido con `[Authorize]`)

### Parámetros de Query

| Parámetro | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| `idEstadoCuenta` | int | Sí | ID del estado de cuenta a consultar. Debe ser mayor a 0. |

### Ejemplo de Request

```http
GET /api/estadocuenta/edoCuentaConsulta?idEstadoCuenta=123
Authorization: Bearer {token}
```

### Respuestas

#### 200 OK - Éxito

Retorna un objeto `EstadoCuentaCompletoDto` con toda la información del estado de cuenta:

```json
{
  "estadoCuenta": {
    "idEstadoCuenta": 123,
    "numeroCuenta": "1234567890",
    "clabe": "012345678901234567",
    "tipoCuenta": "CHEQUES",
    "tipoMoneda": "MXN",
    "fechaInicio": "2024-01-01T00:00:00",
    "fechaFin": "2024-01-31T23:59:59",
    "fechaCorte": "2024-02-01T00:00:00",
    "saldoInicial": 10000.00,
    "totalCargos": 5000.00,
    "totalAbonos": 8000.00,
    "saldoFinal": 13000.00,
    "totalComisiones": 150.00,
    "totalISR": 50.00,
    "totalIVA": 24.00,
    "fechaCarga": "2024-02-01T10:30:00"
  },
  "movimientos": [
    {
      "idMovimiento": 1,
      "fecha": "2024-01-15T10:00:00",
      "descripcion": "DEPOSITO",
      "referencia": "REF001",
      "cargo": null,
      "abono": 5000.00,
      "saldo": 15000.00,
      "tipoOperacion": "DEPOSITO"
    }
  ],
  "transferenciasSPEI": [
    {
      "idTransferencia": 1,
      "idMovimiento": 1,
      "tipoTransferencia": "RECIBIDA",
      "bancoClave": "012",
      "bancoNombre": "BBVA",
      "cuentaOrigen": "1234567890",
      "cuentaDestino": "0987654321",
      "nombreEmisor": "Juan Pérez",
      "nombreDestinatario": "María López",
      "rfcEmisor": "PEPJ800101XXX",
      "rfcDestinatario": "LOPM850202XXX",
      "claveRastreo": "TR123456789",
      "concepto": "Pago de servicios",
      "monto": 5000.00
    }
  ],
  "comisiones": [
    {
      "idComision": 1,
      "idMovimiento": 2,
      "tipoComision": "TRANSFERENCIA",
      "monto": 150.00,
      "iva": 24.00,
      "referencia": "COM001",
      "fecha": "2024-01-15T10:00:00",
      "descripcion": "Comisión por transferencia"
    }
  ],
  "impuestos": [
    {
      "idImpuesto": 1,
      "idMovimiento": 3,
      "tipoImpuesto": "ISR",
      "rfc": "LOPM850202XXX",
      "monto": 50.00,
      "fecha": "2024-01-31T00:00:00",
      "descripcion": "Retención ISR"
    }
  ]
}
```

**Nota:** Las listas (`movimientos`, `transferenciasSPEI`, `comisiones`, `impuestos`) pueden ser `null` si no hay datos, o listas vacías `[]` si el resultado del stored procedure retorna el conjunto pero sin filas.

#### 400 Bad Request - Parámetro inválido

```json
{
  "message": "El parámetro 'idEstadoCuenta' es obligatorio y debe ser mayor a 0."
}
```

#### 404 Not Found - Estado de cuenta no encontrado

```json
{
  "message": "No se encontró un estado de cuenta con el ID 123."
}
```

#### 500 Internal Server Error - Error del servidor

En modo DEBUG:
```json
{
  "message": "Error al acceder a la base de datos.",
  "innerMessage": "Detalles del error..."
}
```

En modo RELEASE:
```json
{
  "message": "Error al acceder a la base de datos."
}
```

## Implementación Técnica

### Estructura de Archivos

1. **DTO**: `/AdvanceApi/DTOs/EstadoCuentaCompletoDto.cs`
   - Define la estructura de respuesta completa
   - Contiene propiedades para EstadoCuenta, Movimientos, TransferenciasSPEI, Comisiones e Impuestos

2. **Interfaz de Servicio**: `/AdvanceApi/Services/IEstadoCuentaService.cs`
   - Método `ConsultarEstadoCuentaCompletoAsync(int idEstadoCuenta)`

3. **Implementación de Servicio**: `/AdvanceApi/Services/EstadoCuentaService.cs`
   - Implementa la lógica para llamar al stored procedure
   - Lee múltiples conjuntos de resultados
   - Mapea cada conjunto a su modelo correspondiente

4. **Controlador**: `/AdvanceApi/Controllers/EstadoCuentaController.cs`
   - Endpoint `[HttpGet("edoCuentaConsulta")]`
   - Validación de parámetros
   - Manejo de errores

### Procedimiento Almacenado

El endpoint consume el procedimiento `sp_ConsultarEstadoCuentaCompleto` que retorna 5 conjuntos de resultados:

1. **Estado de Cuenta**: Un registro con la información general
2. **Movimientos**: Lista de movimientos ordenados por fecha
3. **Transferencias SPEI**: Transferencias asociadas a los movimientos
4. **Comisiones**: Comisiones bancarias asociadas a los movimientos
5. **Impuestos**: Impuestos asociados a los movimientos

### Manejo de Errores

- **Validación de entrada**: Verifica que `idEstadoCuenta` sea mayor a 0
- **Registro de logs**: Registra errores SQL y excepciones inesperadas
- **Respuestas apropiadas**: Retorna códigos HTTP correctos según el tipo de error
- **Información de depuración**: En modo DEBUG incluye detalles adicionales del error

## Características Implementadas

✅ Validación de parámetros de entrada  
✅ Autenticación requerida (JWT)  
✅ Manejo robusto de valores NULL  
✅ Logging de operaciones y errores  
✅ Lectura de múltiples conjuntos de resultados  
✅ Mapeo completo de todos los modelos relacionados  
✅ Documentación XML en el código  
✅ Respuestas HTTP apropiadas (200, 400, 404, 500)  
✅ Seguridad: Sin vulnerabilidades detectadas por CodeQL  

## Consideraciones de Uso

1. **Performance**: El endpoint retorna toda la información relacionada en una sola llamada, lo cual es eficiente pero puede retornar grandes volúmenes de datos si el estado de cuenta tiene muchos movimientos.

2. **NULL vs Lista vacía**: 
   - Si no hay registros en un conjunto de resultados, la propiedad correspondiente será `null` o una lista vacía `[]`
   - El cliente debe manejar ambos casos

3. **Autenticación**: Siempre incluir el token JWT en el header `Authorization: Bearer {token}`

4. **Estado de cuenta inexistente**: Si el `idEstadoCuenta` no existe, se retorna 404 Not Found

## Ejemplo de Uso con cURL

```bash
curl -X GET "https://api.example.com/api/estadocuenta/edoCuentaConsulta?idEstadoCuenta=123" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json"
```

## Ejemplo de Uso con JavaScript/Fetch

```javascript
const idEstadoCuenta = 123;
const token = 'your-jwt-token';

fetch(`/api/estadocuenta/edoCuentaConsulta?idEstadoCuenta=${idEstadoCuenta}`, {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
})
.then(response => {
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  return response.json();
})
.then(data => {
  console.log('Estado de Cuenta:', data.estadoCuenta);
  console.log('Movimientos:', data.movimientos);
  console.log('Transferencias SPEI:', data.transferenciasSPEI);
  console.log('Comisiones:', data.comisiones);
  console.log('Impuestos:', data.impuestos);
})
.catch(error => {
  console.error('Error:', error);
});
```

## Historial de Cambios

### Versión 1.0.0 (2026-02-12)
- ✅ Implementación inicial del endpoint edoCuentaConsulta
- ✅ Creación de DTO EstadoCuentaCompletoDto
- ✅ Implementación de servicio ConsultarEstadoCuentaCompletoAsync
- ✅ Optimización de inicialización de colecciones
- ✅ Validación de estado de cuenta existente
- ✅ Revisión de código y corrección de issues
- ✅ Verificación de seguridad con CodeQL (sin vulnerabilidades)
