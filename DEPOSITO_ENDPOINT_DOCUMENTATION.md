# Deposito Endpoint Implementation Summary

## Overview
This document describes the implementation of the `deposito` endpoint that uses two stored procedures for creating and querying deposits.

## Stored Procedures
The endpoint uses the following stored procedures:

1. **sp_CrearDeposito** - Creates a new deposit
   - Parameters: `@idMovimiento`, `@tipoDeposito`, `@referencia` (optional), `@monto`
   - Output: `@idDeposito`

2. **sp_ConsultarDepositos** - Queries deposits
   - Parameters: `@idMovimiento` (optional), `@tipoDeposito` (optional), `@fechaInicio` (optional), `@fechaFin` (optional)
   - Returns: List of deposits with associated movement details

## Implementation Components

### 1. Data Transfer Object (DTO)
**File:** `AdvanceApi/DTOs/DepositoQueryDto.cs`
```csharp
public class DepositoQueryDto
{
    public int? IdMovimiento { get; set; }
    public string? TipoDeposito { get; set; }
    public string? Referencia { get; set; }
    public decimal? Monto { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
```

### 2. Service Interface
**File:** `AdvanceApi/Services/IDepositoService.cs`
- `Task<object> CrearDepositoAsync(DepositoQueryDto query)`
- `Task<List<object>> ConsultarDepositosAsync(int? idMovimiento, string? tipoDeposito, DateTime? fechaInicio, DateTime? fechaFin)`

### 3. Service Implementation
**File:** `AdvanceApi/Services/DepositoService.cs`

The service uses `DbHelper` to interact with the database and execute the stored procedures.

Key features:
- Async/await pattern for database operations
- Error handling with SqlException catching
- Logging for debugging and error tracking
- Parameter validation and proper DBNull handling

### 4. Controller
**File:** `AdvanceApi/Controllers/DepositoController.cs`

**Route:** `api/deposito`

**Endpoints:**

#### GET - Query Deposits
- **URL:** `/api/deposito`
- **Method:** `GET`
- **Auth required:** Yes (JWT Bearer token)
- **Query Parameters:**
  - `idMovimiento` (optional): Filter by movement ID
  - `tipoDeposito` (optional): Filter by deposit type
  - `fechaInicio` (optional): Filter by start date
  - `fechaFin` (optional): Filter by end date

**Success Response:**
- **Code:** 200
- **Content:** 
```json
[
  {
    "idDeposito": 1,
    "idMovimiento": 123,
    "tipoDeposito": "Efectivo",
    "referencia": "REF-001",
    "monto": 1000.00,
    "fecha": "2024-01-15T10:30:00",
    "descripcion": "Depósito en efectivo"
  }
]
```

#### POST - Create Deposit
- **URL:** `/api/deposito`
- **Method:** `POST`
- **Auth required:** Yes (JWT Bearer token)
- **Query Parameters:**
  - `idMovimiento` (required): Movement ID
  - `tipoDeposito` (required): Type of deposit
  - `monto` (required): Deposit amount (must be > 0)
  - `referencia` (optional): Reference number

**Success Response:**
- **Code:** 200
- **Content:** 
```json
{
  "success": true,
  "idDeposito": 1,
  "message": "Depósito creado exitosamente"
}
```

**Error Response:**
- **Code:** 400 Bad Request
- **Content:** `{ "message": "Validation error message" }`

- **Code:** 500 Internal Server Error
- **Content:** `{ "message": "Error message" }`

### 5. Dependency Injection
**File:** `AdvanceApi/Program.cs`

The service is registered in the DI container:
```csharp
builder.Services.AddScoped<IDepositoService, DepositoService>();
```

## Error Handling
- Input validation for required fields
- SQL exception handling with detailed logging
- Debug mode provides more detailed error messages
- Production mode returns generic error messages

## Security
- Endpoint requires JWT Bearer token authentication
- CodeQL security scan passed with no vulnerabilities

## Testing
The implementation was successfully built with no compilation errors or warnings.

## Usage Examples

### Creating a Deposit
```bash
POST /api/deposito?idMovimiento=123&tipoDeposito=Efectivo&monto=1000.00&referencia=REF-001
Authorization: Bearer <token>
```

### Querying Deposits
```bash
GET /api/deposito?tipoDeposito=Efectivo&fechaInicio=2024-01-01&fechaFin=2024-12-31
Authorization: Bearer <token>
```

### Query Deposits by Movement
```bash
GET /api/deposito?idMovimiento=123
Authorization: Bearer <token>
```

## Notes
- The endpoint follows the existing codebase patterns for consistency
- Uses `[FromQuery]` parameters consistent with other POST endpoints in the project
- All database operations are asynchronous
- Proper disposal of database connections using `await using` pattern
