# Servicio Endpoint Implementation

## Overview
This document describes the implementation of the **servicio** endpoint that consumes the stored procedure `sp_servicio_edit`.

## Endpoint Route
Base URL: `/api/servicio`

## Authentication
All endpoints require JWT Bearer token authentication.

## Endpoints

### 1. GET /api/servicio
Retrieves services based on optional search criteria.

**Query Parameters:**
- `concepto` (string, optional): Partial search by concept
- `descripcion` (string, optional): Partial search by description
- `costo` (double, optional): Exact search by cost

**Response:** List of Servicio objects

**Example:**
```
GET /api/servicio?concepto=Mantenimiento
```

### 2. POST /api/servicio
Creates a new service.

**Query Parameters (Required):**
- `concepto` (string): Service concept
- `descripcion` (string): Service description
- `costo` (double): Service cost
- `estatus` (bool, optional, default: true): Service status

**Response:** Success/failure message

**Example:**
```
POST /api/servicio?concepto=Servicio de Prueba&descripcion=Descripcion de prueba&costo=150.00
```

### 3. PUT /api/servicio/{id}
Updates an existing service.

**Path Parameters:**
- `id` (int): Service ID to update

**Query Parameters (Optional):**
- `concepto` (string): New concept
- `descripcion` (string): New description
- `costo` (double): New cost

**Response:** Success/failure message

**Example:**
```
PUT /api/servicio/1?concepto=Nuevo Concepto&costo=200.00
```

### 4. DELETE /api/servicio/{id}
Soft deletes a service (sets estatus to false).

**Path Parameters:**
- `id` (int): Service ID to delete

**Response:** Success/failure message

**Example:**
```
DELETE /api/servicio/1
```

## Stored Procedure
All endpoints use the `sp_servicio_edit` stored procedure with the following operations:
- **select**: Retrieve services
- **put**: Create new service
- **update**: Update existing service
- **delete**: Soft delete service

## Implementation Files
1. **Clases/Servicio.cs** - Data model
2. **DTOs/ServicioQueryDto.cs** - Query parameters DTO
3. **Services/IServicioService.cs** - Service interface
4. **Services/ServicioService.cs** - Service implementation
5. **Controllers/ServicioController.cs** - REST API controller
6. **Program.cs** - DI registration

## Error Handling
The endpoint includes comprehensive error handling:
- SQL exceptions are caught and logged
- Invalid IDs return 400 Bad Request
- Database errors return 500 Internal Server Error
- Debug mode provides detailed error messages

## Validation
- Required fields are validated before creating services
- ID validation for update and delete operations
- Null safety for optional parameters

## Testing Examples

### Using curl:
```bash
# Get all services
curl -H "Authorization: Bearer <token>" https://api.example.com/api/servicio

# Create service
curl -X POST -H "Authorization: Bearer <token>" \
  "https://api.example.com/api/servicio?concepto=Test&descripcion=Test Desc&costo=100.5"

# Update service
curl -X PUT -H "Authorization: Bearer <token>" \
  "https://api.example.com/api/servicio/1?costo=150.0"

# Delete service
curl -X DELETE -H "Authorization: Bearer <token>" \
  https://api.example.com/api/servicio/1
```
