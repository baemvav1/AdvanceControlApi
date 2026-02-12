# Fiscal Estado de Cuenta (fiscalEdoCta) Endpoint Documentation

## Overview
The `fiscalEdoCta` endpoint provides functionality to manage fiscal stamps (TimbreFiscal) and fiscal complements (ComplementoFiscal) related to account statements (EstadoCuenta).

## Authentication
All endpoints require JWT authentication. Include the JWT token in the Authorization header:
```
Authorization: Bearer {token}
```

## Endpoints

### 1. Create Fiscal Stamp (Timbre Fiscal)
Creates a new fiscal stamp for an account statement.

**Endpoint:** `POST /api/fiscalEdoCta/timbre`

**Parameters:**
- `idEstadoCuenta` (int, required): ID of the account statement
- `uuid` (string, required): UUID of the fiscal stamp
- `fechaTimbrado` (DateTime, required): Timestamp date
- `numeroProveedor` (string, optional): Provider number

**Example Request:**
```
POST /api/fiscalEdoCta/timbre?idEstadoCuenta=1&uuid=A1B2C3D4-E5F6-7890-ABCD-EF1234567890&fechaTimbrado=2024-01-15T10:30:00&numeroProveedor=PROV123
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "idTimbre": 1,
  "message": "Timbre fiscal creado exitosamente"
}
```

**Stored Procedure:** `sp_CrearTimbreFiscal`

---

### 2. Query Fiscal Stamps (Timbres Fiscales)
Retrieves fiscal stamps based on search criteria.

**Endpoint:** `GET /api/fiscalEdoCta/timbres`

**Parameters:**
- `idEstadoCuenta` (int, optional): Filter by account statement ID
- `uuid` (string, optional): Filter by UUID

**Example Requests:**
```
GET /api/fiscalEdoCta/timbres
GET /api/fiscalEdoCta/timbres?idEstadoCuenta=1
GET /api/fiscalEdoCta/timbres?uuid=A1B2C3D4-E5F6-7890-ABCD-EF1234567890
```

**Success Response (200 OK):**
```json
[
  {
    "idTimbre": 1,
    "idEstadoCuenta": 1,
    "uuid": "A1B2C3D4-E5F6-7890-ABCD-EF1234567890",
    "fechaTimbrado": "2024-01-15T10:30:00",
    "numeroProveedor": "PROV123",
    "numeroCuenta": "1234567890",
    "fechaCorte": "2024-01-31T23:59:59"
  }
]
```

**Stored Procedure:** `sp_ConsultarTimbresFiscales`

---

### 3. Create Fiscal Complement (Complemento Fiscal)
Creates a new fiscal complement for an account statement.

**Endpoint:** `POST /api/fiscalEdoCta/complemento`

**Parameters:**
- `idEstadoCuenta` (int, required): ID of the account statement
- `rfc` (string, required): Taxpayer's RFC (Federal Taxpayer Registry)
- `formaPago` (string, optional): Payment method code
- `metodoPago` (string, optional): Payment method type
- `usoCFDI` (string, optional): CFDI usage code
- `claveProducto` (string, optional): Product key
- `codigoPostal` (string, optional): Postal code

**Example Request:**
```
POST /api/fiscalEdoCta/complemento?idEstadoCuenta=1&rfc=ABC123456789&formaPago=01&metodoPago=PUE&usoCFDI=G03&claveProducto=84111506&codigoPostal=06600
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "idComplemento": 1,
  "message": "Complemento fiscal creado exitosamente"
}
```

**Stored Procedure:** `sp_CrearComplementoFiscal`

---

### 4. Query Fiscal Complements (Complementos Fiscales)
Retrieves fiscal complements based on search criteria.

**Endpoint:** `GET /api/fiscalEdoCta/complementos`

**Parameters:**
- `idEstadoCuenta` (int, optional): Filter by account statement ID
- `rfc` (string, optional): Filter by RFC

**Example Requests:**
```
GET /api/fiscalEdoCta/complementos
GET /api/fiscalEdoCta/complementos?idEstadoCuenta=1
GET /api/fiscalEdoCta/complementos?rfc=ABC123456789
```

**Success Response (200 OK):**
```json
[
  {
    "idComplemento": 1,
    "idEstadoCuenta": 1,
    "rfc": "ABC123456789",
    "formaPago": "01",
    "metodoPago": "PUE",
    "usoCFDI": "G03",
    "claveProducto": "84111506",
    "codigoPostal": "06600",
    "numeroCuenta": "1234567890",
    "fechaCorte": "2024-01-31T23:59:59"
  }
]
```

**Stored Procedure:** `sp_ConsultarComplementosFiscales`

---

## Error Responses

### 400 Bad Request
Returned when required parameters are missing or invalid.

**Example:**
```json
{
  "message": "El campo 'idEstadoCuenta' debe ser mayor a 0."
}
```

### 401 Unauthorized
Returned when JWT token is missing or invalid.

### 500 Internal Server Error
Returned when there's a database or server error.

**Development Mode:**
```json
{
  "message": "Error al acceder a la base de datos.",
  "innerMessage": "Detailed error message"
}
```

**Production Mode:**
```json
{
  "message": "Error al acceder a la base de datos."
}
```

---

## Database Schema

### TimbreFiscal Table
- `idTimbre` (int, PK): Fiscal stamp ID
- `idEstadoCuenta` (int, FK): Account statement ID
- `uuid` (nvarchar(50)): UUID of the fiscal stamp
- `fechaTimbrado` (datetime): Timestamp date
- `numeroProveedor` (nvarchar(50)): Provider number

### ComplementoFiscal Table
- `idComplemento` (int, PK): Fiscal complement ID
- `idEstadoCuenta` (int, FK): Account statement ID
- `rfc` (nvarchar(13)): Taxpayer's RFC
- `formaPago` (nvarchar(10)): Payment method code
- `metodoPago` (nvarchar(10)): Payment method type
- `usoCFDI` (nvarchar(10)): CFDI usage code
- `claveProducto` (nvarchar(20)): Product key
- `codigoPostal` (nvarchar(10)): Postal code

---

## Implementation Details

### Models
- **TimbreFiscal** (`Clases/TimbreFiscal.cs`): Model class for fiscal stamps
- **ComplementoFiscal** (`Clases/ComplementoFiscal.cs`): Model class for fiscal complements

### DTOs
- **TimbreFiscalCreateDto**: DTO for creating fiscal stamps
- **TimbreFiscalQueryDto**: DTO for querying fiscal stamps
- **ComplementoFiscalCreateDto**: DTO for creating fiscal complements
- **ComplementoFiscalQueryDto**: DTO for querying fiscal complements

### Services
- **IFiscalEdoCtaService**: Service interface
- **FiscalEdoCtaService**: Service implementation that interacts with stored procedures

### Controller
- **FiscalEdoCtaController**: Controller that exposes the API endpoints

### Service Registration
The service is registered in `Program.cs`:
```csharp
builder.Services.AddScoped<AdvanceApi.Services.IFiscalEdoCtaService, AdvanceApi.Services.FiscalEdoCtaService>();
```

---

## Security

### Authentication
All endpoints are protected with JWT authentication using the `[Authorize]` attribute.

### SQL Injection Protection
All database operations use parameterized stored procedure calls to prevent SQL injection attacks.

### Input Validation
- Required fields are validated in the controller
- ID fields must be greater than 0
- String fields are checked for null or whitespace

---

## Notes

1. The fiscal stamp UUID should follow the UUID format for proper validation in the database.
2. The RFC must follow the Mexican Federal Taxpayer Registry format (12 or 13 characters).
3. CFDI codes (formaPago, metodoPago, usoCFDI, claveProducto) should follow the Mexican SAT (Tax Administration Service) catalog.
4. All timestamps are in UTC and should be converted to local time on the client side.
5. The endpoint follows the existing patterns in the codebase, using query parameters for POST requests to maintain consistency with other endpoints like EstadoCuentaController.

---

## Testing

To test the endpoints, you can use tools like Postman, curl, or Swagger UI (available in development mode at `/swagger`).

### Example curl commands:

**Create Fiscal Stamp:**
```bash
curl -X POST "https://localhost:5001/api/fiscalEdoCta/timbre?idEstadoCuenta=1&uuid=A1B2C3D4-E5F6-7890-ABCD-EF1234567890&fechaTimbrado=2024-01-15T10:30:00" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Query Fiscal Stamps:**
```bash
curl -X GET "https://localhost:5001/api/fiscalEdoCta/timbres?idEstadoCuenta=1" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Create Fiscal Complement:**
```bash
curl -X POST "https://localhost:5001/api/fiscalEdoCta/complemento?idEstadoCuenta=1&rfc=ABC123456789&formaPago=01" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Query Fiscal Complements:**
```bash
curl -X GET "https://localhost:5001/api/fiscalEdoCta/complementos?rfc=ABC123456789" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```
