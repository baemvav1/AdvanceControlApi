# PagoServicios Endpoint Implementation Summary

## Overview
This document describes the implementation of the `pagoServicios` endpoint that uses two stored procedures for creating and querying service payments.

## Stored Procedures
The endpoint uses the following stored procedures:

1. **sp_CrearPagoServicio** - Creates a new service payment
   - Parameters: `@idMovimiento`, `@tipoServicio`, `@referencia` (optional), `@monto`
   - Output: `@idPago`

2. **sp_ConsultarPagosServicio** - Queries service payments
   - Parameters: `@idMovimiento` (optional), `@tipoServicio` (optional), `@fechaInicio` (optional), `@fechaFin` (optional)
   - Returns: List of service payments with associated movement details

## Implementation Components

### 1. Model Class
**File:** `AdvanceApi/Clases/PagoServicio.cs`
```csharp
public class PagoServicio
{
    public int IdPago { get; set; }
    public int IdMovimiento { get; set; }
    public string TipoServicio { get; set; }
    public string? Referencia { get; set; }
    public decimal Monto { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Descripcion { get; set; }
}
```

### 2. Data Transfer Object (DTO)
**File:** `AdvanceApi/DTOs/PagoServicioQueryDto.cs`
```csharp
public class PagoServicioQueryDto
{
    public int? IdMovimiento { get; set; }
    public string? TipoServicio { get; set; }
    public string? Referencia { get; set; }
    public decimal? Monto { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
```

### 3. Service Interface
**File:** `AdvanceApi/Services/IPagoServicioService.cs`
- `Task<object> CrearPagoServicioAsync(PagoServicioQueryDto query)`
- `Task<List<PagoServicio>> ConsultarPagosServicioAsync(int?, string?, DateTime?, DateTime?)`

### 4. Service Implementation
**File:** `AdvanceApi/Services/PagoServicioService.cs`
- Implements the interface using `DbHelper` for database connectivity
- Uses `SqlCommand` with `CommandType.StoredProcedure`
- Handles output parameters and result reading
- Includes comprehensive error handling and logging

### 5. Controller
**File:** `AdvanceApi/Controllers/PagoServiciosController.cs`
**Route:** `/api/pagoServicios`
**Authentication:** Required (`[Authorize]` attribute)

#### Endpoints:

**GET /api/pagoServicios**
- Query service payments with optional filters
- Parameters:
  - `idMovimiento` (optional) - Filter by movement ID
  - `tipoServicio` (optional) - Filter by service type
  - `fechaInicio` (optional) - Filter by start date
  - `fechaFin` (optional) - Filter by end date
- Returns: List of `PagoServicio` objects

**POST /api/pagoServicios**
- Create a new service payment
- Parameters (all via query string):
  - `idMovimiento` (required) - Movement ID
  - `tipoServicio` (required) - Service type
  - `monto` (required) - Payment amount
  - `referencia` (optional) - Payment reference
- Returns: JSON with `success`, `idPago`, and `message`

### 6. Dependency Injection Registration
**File:** `AdvanceApi/Program.cs`
```csharp
builder.Services.AddScoped<AdvanceApi.Services.IPagoServicioService, AdvanceApi.Services.PagoServicioService>();
```

## Usage Examples

### Create a service payment:
```
POST /api/pagoServicios?idMovimiento=123&tipoServicio=Luz&monto=500.50&referencia=ABC123
Authorization: Bearer {token}
```

Response:
```json
{
  "success": true,
  "idPago": 456,
  "message": "Pago de servicio creado exitosamente"
}
```

### Query service payments:
```
GET /api/pagoServicios?tipoServicio=Agua&fechaInicio=2024-01-01&fechaFin=2024-12-31
Authorization: Bearer {token}
```

Response:
```json
[
  {
    "idPago": 1,
    "idMovimiento": 123,
    "tipoServicio": "Agua",
    "referencia": "REF001",
    "monto": 250.00,
    "fecha": "2024-06-15T00:00:00",
    "descripcion": "Pago de agua mensual"
  }
]
```

## Validation
- Build: ✅ Successful (no warnings or errors)
- Code Review: ✅ Passed (follows existing patterns)
- Security Scan (CodeQL): ✅ No vulnerabilities found

## Design Decisions

### Following Existing Patterns
The implementation follows the exact patterns used in other endpoints in the codebase:

1. **Query Parameters for POST**: Following `MovimientoController` pattern, POST endpoints use `[FromQuery]` instead of `[FromBody]`. While this differs from REST best practices, it maintains consistency with the existing codebase.

2. **Return Type**: Service methods return `Task<object>` for creation operations, matching the pattern used in `MovimientoService` and other services.

3. **Error Handling**: Uses the same error handling pattern with `InvalidOperationException` and conditional compilation (`#if DEBUG`) for detailed error messages.

4. **Authorization**: All endpoints require JWT authentication using the `[Authorize]` attribute.

5. **Logging**: Comprehensive logging at debug and error levels using `ILogger<T>`.

## Security Features
- JWT authentication required for all endpoints
- Input validation (positive values, required fields)
- SQL injection protection through parameterized queries
- Comprehensive error handling with safe error messages in production
- No security vulnerabilities detected by CodeQL scanner

## Testing Recommendations
When testing this endpoint, ensure:
1. The database has the stored procedures `sp_CrearPagoServicio` and `sp_ConsultarPagosServicio` created
2. A valid JWT token is provided in the Authorization header
3. The `Movimiento` table has valid records to reference
4. Test with various filter combinations for the GET endpoint
5. Test error scenarios (invalid movement ID, negative amounts, etc.)
