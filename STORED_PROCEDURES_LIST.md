# Stored Procedures List for the API

This document contains a complete list of all stored procedures used in the AdvanceControlApi.

## Summary

The API uses **15 stored procedures** distributed across the following categories:

- **Authentication and Security**: 5 procedures
- **Client Management**: 1 procedure
- **Equipment Management**: 2 procedures
- **Parts Management**: 1 procedure
- **Maintenance Management**: 1 procedure
- **Operations Management**: 1 procedure
- **Equipment-Client Relations**: 1 procedure
- **Part-Equipment Relations**: 1 procedure
- **User Information**: 1 procedure
- **Logging**: 1 procedure

---

## 1. Authentication and Security

### 1.1 login_credencial
- **File**: `AdvanceApi/Controllers/AuthController.cs` (line 66)
- **Purpose**: Validates user credentials during login
- **Parameters**:
  - `@usuario` (NVarChar, 150): Username
  - `@contraseña` (NVarChar, 100): User password
- **Returns**: Boolean or integer indicating whether credentials are valid
- **Used in**: `Login()` method of AuthController

### 1.2 InsertRefreshToken
- **File**: `AdvanceApi/Helpers/DbHelper.cs` (line 65)
- **Purpose**: Inserts a new refresh token into the database
- **Parameters**:
  - `@Usuario` (NVarChar, 150): User associated with the token
  - `@TokenHash` (NVarChar, 200): Hash of the refresh token
  - `@ExpiresAt` (DateTime2): Expiration date
  - `@IpAddress` (NVarChar, 50): Client IP address
  - `@UserAgent` (NVarChar, 1000): Browser user agent
- **Returns**: ID of the newly inserted record
- **Used in**: `Login()` and `Refresh()` methods of AuthController

### 1.3 GetRefreshTokenByHash
- **File**: `AdvanceApi/Helpers/DbHelper.cs` (line 97)
- **Purpose**: Retrieves a refresh token record by its hash
- **Parameters**:
  - `@TokenHash` (NVarChar, 200): Hash of the token to search for
- **Returns**: Complete token record including user, dates, revocation status
- **Used in**: `Refresh()` and `Logout()` methods of AuthController

### 1.4 RevokeRefreshTokenById
- **File**: `AdvanceApi/Helpers/DbHelper.cs` (line 131)
- **Purpose**: Revokes a specific refresh token by its ID
- **Parameters**:
  - `@Id` (BigInt): ID of the token to revoke
  - `@ReplacedByTokenHash` (NVarChar, 200): Hash of the token that replaces it (optional)
- **Returns**: None
- **Used in**: `Refresh()` and `Logout()` methods of AuthController

### 1.5 RevokeAllRefreshTokensForUser
- **File**: `AdvanceApi/Helpers/DbHelper.cs` (line 150)
- **Purpose**: Revokes all active refresh tokens for a user
- **Parameters**:
  - `@Usuario` (NVarChar, 150): Username
- **Returns**: None
- **Used in**: `Refresh()` method of AuthController (token reuse detection)

### 1.6 CountActiveRefreshTokensForUser
- **File**: `AdvanceApi/Helpers/DbHelper.cs` (line 165)
- **Purpose**: Counts the active refresh tokens for a user
- **Parameters**:
  - `@Usuario` (NVarChar, 150): Username
- **Returns**: Integer with the number of active tokens
- **Used in**: `CountActiveRefreshTokensForUserAsync()` method of DbHelper

---

## 2. Client Management

### 2.1 sp_cliente_select
- **File**: `AdvanceApi/Services/ClienteService.cs` (line 38)
- **Purpose**: Retrieves clients with optional filters
- **Parameters**:
  - `@search`: General search text (optional)
  - `@rfc`: Client RFC (optional)
  - `@curp`: Client CURP (optional)
  - `@notas`: Notes filter (optional)
  - `@prioridad`: Priority level filter (optional)
- **Returns**: List of clients with all their fields
- **Used in**: `GetClientesAsync()` method of ClienteService

---

## 3. Equipment Management

### 3.1 sp_equipo_edit
- **File**: `AdvanceApi/Services/EquipoService.cs` (lines 38, 91, 145)
- **Purpose**: Executes CRUD operations (select, update, delete) on equipment
- **Parameters**:
  - `@operacion`: Operation type ('select', 'update', 'delete')
  - `@idEquipo`: Equipment ID
  - `@marca`: Equipment brand (optional)
  - `@creado`: Creator user ID (optional)
  - `@descricpion`: Equipment description (optional)
  - `@identificador`: Unique equipment identifier (optional)
  - `@estatus`: Equipment status (active/inactive)
- **Returns**: Depends on operation - list of equipment or result message
- **Used in**: `ExecuteEquipoOperationAsync()`, `DeleteEquipoAsync()`, `UpdateEquipoAsync()` methods of EquipoService

### 3.2 sp_equipo_create
- **File**: `AdvanceApi/Services/EquipoService.cs` (line 199)
- **Purpose**: Creates new equipment in the database
- **Parameters**:
  - `@marca`: Equipment brand (optional)
  - `@creado`: Creator user ID (optional)
  - `@descripcion`: Equipment description (optional)
  - `@identificador`: Unique equipment identifier (optional)
  - `@estatus`: Equipment status (active/inactive)
- **Returns**: ID of created equipment and complete equipment data
- **Used in**: `CreateEquipoAsync()` method of EquipoService

---

## 4. Parts Management

### 4.1 sp_refaccion_edit
- **File**: `AdvanceApi/Services/RefaccionService.cs` (lines 38, 91, 145, 199)
- **Purpose**: Executes CRUD operations on parts
- **Parameters**:
  - `@operacion`: Operation type ('select', 'update', 'delete', 'put')
  - `@idRefaccion`: Part ID
  - `@marca`: Part brand (optional)
  - `@serie`: Serial number (optional)
  - `@costo`: Part cost (optional)
  - `@descripcion`: Part description (optional)
  - `@estatus`: Part status (active/inactive)
- **Returns**: Depends on operation - list of parts or result message
- **Used in**: `ExecuteRefaccionOperationAsync()`, `DeleteRefaccionAsync()`, `UpdateRefaccionAsync()`, `CreateRefaccionAsync()` methods of RefaccionService

---

## 5. Maintenance Management

### 5.1 sp_MatenimientoEdit
- **File**: `AdvanceApi/Services/MantenimientoService.cs` (lines 38, 97, 149)
- **Purpose**: Executes CRUD operations on maintenance records
- **Parameters**:
  - `@operacion`: Operation type ('select', 'put', 'delete')
  - `@identificador`: Equipment identifier (optional)
  - `@idCliente`: Client ID
  - `@nota`: Maintenance notes (optional)
  - `@idMantenimiento`: Maintenance ID (optional)
  - `@idEquipo`: Equipment ID (optional)
  - `@costo`: Maintenance cost (optional)
  - `@idTipoMantenimiento`: Maintenance type (optional)
- **Returns**: Depends on operation - list of maintenance records or result message
- **Used in**: `GetMantenimientosAsync()`, `CreateMantenimientoAsync()`, `DeleteMantenimientoAsync()` methods of MantenimientoService

---

## 6. Operations Management

### 6.1 sp_operacion_select
- **File**: `AdvanceApi/Services/OperacionService.cs` (line 38)
- **Purpose**: Retrieves operations with optional filters
- **Parameters**:
  - `@idtipo`: Operation type (optional)
  - `@idcliente`: Client ID (optional)
  - `@estatus`: Operation status (optional)
- **Returns**: List of operations with complete information
- **Used in**: `GetOperacionesAsync()` method of OperacionService

---

## 7. Equipment-Client Relations

### 7.1 sp_relacionEquipoCliente_edit
- **File**: `AdvanceApi/Services/RelacionEquipoClienteService.cs` (lines 38, 90, 138, 189)
- **Purpose**: Manages relations between equipment and clients
- **Parameters**:
  - `@operacion`: Operation type ('select', 'put', 'delete', 'update_nota')
  - `@identificador`: Equipment identifier (optional)
  - `@idCliente`: Client ID
  - `@nota`: Relation notes (optional)
- **Returns**: Depends on operation - list of relations or result message
- **Used in**: `GetRelacionesAsync()`, `CreateRelacionAsync()`, `DeleteRelacionAsync()`, `UpdateNotaAsync()` methods of RelacionEquipoClienteService

---

## 8. Part-Equipment Relations

### 8.1 sp_relacionRefaccionEquipo_edit
- **File**: `AdvanceApi/Services/RelacionRefaccionEquipoService.cs` (lines 35, 87, 139, 188, 240)
- **Purpose**: Manages relations between parts and equipment
- **Parameters**:
  - `@operacion`: Operation type ('select_refacciones', 'select_equipos', 'put', 'delete', 'update_nota')
  - `@idRelacionRefaccion`: Relation ID (optional)
  - `@idRefaccion`: Part ID (optional)
  - `@nota`: Relation notes (optional)
  - `@idEquipo`: Equipment ID (optional)
- **Returns**: Depends on operation - list of parts, equipment, or result message
- **Used in**: `GetRefaccionesByEquipoAsync()`, `GetEquiposByRefaccionAsync()`, `CreateRelacionAsync()`, `DeleteRelacionAsync()`, `UpdateNotaAsync()` methods of RelacionRefaccionEquipoService

---

## 9. User Information

### 9.1 sp_contacto_usuario_select
- **File**: `AdvanceApi/Services/ContactoUsuarioService.cs` (line 34)
- **Purpose**: Retrieves contact information for a user
- **Parameters**:
  - `@usuario`: Username
- **Returns**: Complete user information (credencial_id, name, email, phone, level, type)
- **Used in**: `GetContactoUsuarioAsync()` method of ContactoUsuarioService

---

## 10. Logging

### 10.1 sp_InsertLog
- **File**: `AdvanceApi/Services/LoggingService.cs` (line 39)
- **Purpose**: Inserts log entries into the database
- **Parameters**:
  - `@Id`: Unique log ID
  - `@Level`: Log level (Info, Warning, Error, etc.)
  - `@Message`: Log message (optional)
  - `@Exception`: Captured exception (optional)
  - `@StackTrace`: Exception stack trace (optional)
  - `@Source`: Log source (optional)
  - `@Method`: Method that generated the log (optional)
  - `@Username`: Associated user (optional)
  - `@MachineName`: Machine name (optional)
  - `@AppVersion`: Application version (optional)
  - `@Timestamp`: Log date and time
  - `@AdditionalData`: Additional data in JSON format (optional)
- **Returns**: LogId and optionally AlertId
- **Used in**: `LogAsync()` method of LoggingService

---

## Important Notes

1. **Naming Convention**: Stored procedures follow mainly two patterns:
   - `sp_[entity]_[action]`: For specific operations (e.g., sp_cliente_select, sp_operacion_select)
   - `sp_[entity]Edit`: For multiple CRUD operations with @operacion parameter (e.g., sp_equipo_edit, sp_refaccion_edit)

2. **Security**: 
   - All procedures use `CommandType.StoredProcedure`
   - Parameters are added using `AddWithValue` or `Add` with specific types
   - Passwords are passed securely to the `login_credencial` procedure

3. **Null Value Handling**: 
   - Most optional parameters are handled with `DBNull.Value`
   - The `??` operator is used for conversion: `(object?)value ?? DBNull.Value`

4. **Transactions**:
   - Connections are managed with `await using` to ensure their disposal
   - The async/await pattern is used in all database operations

5. **Logging**:
   - All services implement logging via ILogger
   - Both successful operations (Debug) and errors (Error/Warning) are logged

---

## Statistics

- **Total Stored Procedures**: 15
- **Services using SPs**: 9
- **Controllers using SPs**: 1 (AuthController)
- **Helpers using SPs**: 1 (DbHelper)

---

**Document generated**: 2025-12-14  
**API Version**: AdvanceControlApi
