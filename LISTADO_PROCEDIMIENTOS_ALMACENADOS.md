# Listado de Procedimientos Almacenados del API

Este documento contiene el listado completo de todos los procedimientos almacenados (stored procedures) utilizados en el AdvanceControlApi.

## Resumen

El API utiliza **16 procedimientos almacenados** distribuidos en las siguientes categorías:

- **Autenticación y Seguridad**: 6 procedimientos
- **Gestión de Clientes**: 1 procedimiento
- **Gestión de Equipos**: 2 procedimientos
- **Gestión de Refacciones**: 1 procedimiento
- **Gestión de Mantenimientos**: 1 procedimiento
- **Gestión de Operaciones**: 1 procedimiento
- **Relaciones Equipo-Cliente**: 1 procedimiento
- **Relaciones Refacción-Equipo**: 1 procedimiento
- **Información de Usuarios**: 1 procedimiento
- **Logging**: 1 procedimiento

---

## 1. Autenticación y Seguridad

### 1.1 login_credencial
- **Archivo**: `AdvanceApi/Controllers/AuthController.cs` (línea 66)
- **Propósito**: Valida las credenciales de un usuario durante el proceso de login
- **Parámetros**:
  - `@usuario` (NVarChar, 150): Nombre de usuario
  - `@contraseña` (NVarChar, 100): Contraseña del usuario
- **Retorna**: Booleano o entero indicando si las credenciales son válidas
- **Usado en**: Método `Login()` del AuthController

### 1.2 InsertRefreshToken
- **Archivo**: `AdvanceApi/Helpers/DbHelper.cs` (línea 65)
- **Propósito**: Inserta un nuevo refresh token en la base de datos
- **Parámetros**:
  - `@Usuario` (NVarChar, 150): Usuario asociado al token
  - `@TokenHash` (NVarChar, 200): Hash del token de refresco
  - `@ExpiresAt` (DateTime2): Fecha de expiración
  - `@IpAddress` (NVarChar, 50): Dirección IP del cliente
  - `@UserAgent` (NVarChar, 1000): User agent del navegador
- **Retorna**: ID del nuevo registro insertado
- **Usado en**: Métodos `Login()` y `Refresh()` del AuthController

### 1.3 GetRefreshTokenByHash
- **Archivo**: `AdvanceApi/Helpers/DbHelper.cs` (línea 97)
- **Propósito**: Obtiene un registro de refresh token por su hash
- **Parámetros**:
  - `@TokenHash` (NVarChar, 200): Hash del token a buscar
- **Retorna**: Registro completo del token incluyendo usuario, fechas, estado de revocación
- **Usado en**: Métodos `Refresh()` y `Logout()` del AuthController

### 1.4 RevokeRefreshTokenById
- **Archivo**: `AdvanceApi/Helpers/DbHelper.cs` (línea 131)
- **Propósito**: Revoca un refresh token específico por su ID
- **Parámetros**:
  - `@Id` (BigInt): ID del token a revocar
  - `@ReplacedByTokenHash` (NVarChar, 200): Hash del token que lo reemplaza (opcional)
- **Retorna**: Ninguno
- **Usado en**: Métodos `Refresh()` y `Logout()` del AuthController

### 1.5 RevokeAllRefreshTokensForUser
- **Archivo**: `AdvanceApi/Helpers/DbHelper.cs` (línea 150)
- **Propósito**: Revoca todos los refresh tokens activos de un usuario
- **Parámetros**:
  - `@Usuario` (NVarChar, 150): Nombre del usuario
- **Retorna**: Ninguno
- **Usado en**: Método `Refresh()` del AuthController (detección de reuso de tokens)

### 1.6 CountActiveRefreshTokensForUser
- **Archivo**: `AdvanceApi/Helpers/DbHelper.cs` (línea 165)
- **Propósito**: Cuenta los refresh tokens activos de un usuario
- **Parámetros**:
  - `@Usuario` (NVarChar, 150): Nombre del usuario
- **Retorna**: Número entero con la cantidad de tokens activos
- **Usado en**: Método `CountActiveRefreshTokensForUserAsync()` del DbHelper

---

## 2. Gestión de Clientes

### 2.1 sp_cliente_select
- **Archivo**: `AdvanceApi/Services/ClienteService.cs` (línea 38)
- **Propósito**: Obtiene clientes con filtros opcionales
- **Parámetros**:
  - `@search`: Texto de búsqueda general (opcional)
  - `@rfc`: RFC del cliente (opcional)
  - `@curp`: CURP del cliente (opcional)
  - `@notas`: Filtro por notas (opcional)
  - `@prioridad`: Filtro por nivel de prioridad (opcional)
- **Retorna**: Lista de clientes con todos sus campos
- **Usado en**: Método `GetClientesAsync()` del ClienteService

---

## 3. Gestión de Equipos

### 3.1 sp_equipo_edit
- **Archivo**: `AdvanceApi/Services/EquipoService.cs` (líneas 38, 91, 145)
- **Propósito**: Ejecuta operaciones CRUD (select, update, delete) sobre equipos
- **Parámetros**:
  - `@operacion`: Tipo de operación ('select', 'update', 'delete')
  - `@idEquipo`: ID del equipo
  - `@marca`: Marca del equipo (opcional)
  - `@creado`: ID del usuario creador (opcional)
  - `@descricpion`: Descripción del equipo (opcional)
  - `@identificador`: Identificador único del equipo (opcional)
  - `@estatus`: Estado del equipo (activo/inactivo)
- **Retorna**: Depende de la operación - lista de equipos o mensaje de resultado
- **Usado en**: Métodos `ExecuteEquipoOperationAsync()`, `DeleteEquipoAsync()`, `UpdateEquipoAsync()` del EquipoService

### 3.2 sp_equipo_create
- **Archivo**: `AdvanceApi/Services/EquipoService.cs` (línea 199)
- **Propósito**: Crea un nuevo equipo en la base de datos
- **Parámetros**:
  - `@marca`: Marca del equipo (opcional)
  - `@creado`: ID del usuario creador (opcional)
  - `@descripcion`: Descripción del equipo (opcional)
  - `@identificador`: Identificador único del equipo (opcional)
  - `@estatus`: Estado del equipo (activo/inactivo)
- **Retorna**: ID del equipo creado y datos completos del equipo
- **Usado en**: Método `CreateEquipoAsync()` del EquipoService

---

## 4. Gestión de Refacciones

### 4.1 sp_refaccion_edit
- **Archivo**: `AdvanceApi/Services/RefaccionService.cs` (líneas 38, 91, 145, 199)
- **Propósito**: Ejecuta operaciones CRUD sobre refacciones
- **Parámetros**:
  - `@operacion`: Tipo de operación ('select', 'update', 'delete', 'put')
  - `@idRefaccion`: ID de la refacción
  - `@marca`: Marca de la refacción (opcional)
  - `@serie`: Número de serie (opcional)
  - `@costo`: Costo de la refacción (opcional)
  - `@descripcion`: Descripción de la refacción (opcional)
  - `@estatus`: Estado de la refacción (activo/inactivo)
- **Retorna**: Depende de la operación - lista de refacciones o mensaje de resultado
- **Usado en**: Métodos `ExecuteRefaccionOperationAsync()`, `DeleteRefaccionAsync()`, `UpdateRefaccionAsync()`, `CreateRefaccionAsync()` del RefaccionService

---

## 5. Gestión de Mantenimientos

### 5.1 sp_MatenimientoEdit
- **Archivo**: `AdvanceApi/Services/MantenimientoService.cs` (líneas 38, 97, 149)
- **Propósito**: Ejecuta operaciones CRUD sobre mantenimientos
- **Parámetros**:
  - `@operacion`: Tipo de operación ('select', 'put', 'delete')
  - `@identificador`: Identificador del equipo (opcional)
  - `@idCliente`: ID del cliente
  - `@nota`: Notas del mantenimiento (opcional)
  - `@idMantenimiento`: ID del mantenimiento (opcional)
  - `@idEquipo`: ID del equipo (opcional)
  - `@costo`: Costo del mantenimiento (opcional)
  - `@idTipoMantenimiento`: Tipo de mantenimiento (opcional)
- **Retorna**: Depende de la operación - lista de mantenimientos o mensaje de resultado
- **Usado en**: Métodos `GetMantenimientosAsync()`, `CreateMantenimientoAsync()`, `DeleteMantenimientoAsync()` del MantenimientoService

---

## 6. Gestión de Operaciones

### 6.1 sp_operacion_select
- **Archivo**: `AdvanceApi/Services/OperacionService.cs` (línea 38)
- **Propósito**: Obtiene operaciones con filtros opcionales
- **Parámetros**:
  - `@idtipo`: Tipo de operación (opcional)
  - `@idcliente`: ID del cliente (opcional)
  - `@estatus`: Estado de la operación (opcional)
- **Retorna**: Lista de operaciones con información completa
- **Usado en**: Método `GetOperacionesAsync()` del OperacionService

---

## 7. Relaciones Equipo-Cliente

### 7.1 sp_relacionEquipoCliente_edit
- **Archivo**: `AdvanceApi/Services/RelacionEquipoClienteService.cs` (líneas 38, 90, 138, 189)
- **Propósito**: Gestiona las relaciones entre equipos y clientes
- **Parámetros**:
  - `@operacion`: Tipo de operación ('select', 'put', 'delete', 'update_nota')
  - `@identificador`: Identificador del equipo (opcional)
  - `@idCliente`: ID del cliente
  - `@nota`: Notas de la relación (opcional)
- **Retorna**: Depende de la operación - lista de relaciones o mensaje de resultado
- **Usado en**: Métodos `GetRelacionesAsync()`, `CreateRelacionAsync()`, `DeleteRelacionAsync()`, `UpdateNotaAsync()` del RelacionEquipoClienteService

---

## 8. Relaciones Refacción-Equipo

### 8.1 sp_relacionRefaccionEquipo_edit
- **Archivo**: `AdvanceApi/Services/RelacionRefaccionEquipoService.cs` (líneas 35, 87, 139, 188, 240)
- **Propósito**: Gestiona las relaciones entre refacciones y equipos
- **Parámetros**:
  - `@operacion`: Tipo de operación ('select_refacciones', 'select_equipos', 'put', 'delete', 'update_nota')
  - `@idRelacionRefaccion`: ID de la relación (opcional)
  - `@idRefaccion`: ID de la refacción (opcional)
  - `@nota`: Notas de la relación (opcional)
  - `@idEquipo`: ID del equipo (opcional)
- **Retorna**: Depende de la operación - lista de refacciones, equipos o mensaje de resultado
- **Usado en**: Métodos `GetRefaccionesByEquipoAsync()`, `GetEquiposByRefaccionAsync()`, `CreateRelacionAsync()`, `DeleteRelacionAsync()`, `UpdateNotaAsync()` del RelacionRefaccionEquipoService

---

## 9. Información de Usuarios

### 9.1 sp_contacto_usuario_select
- **Archivo**: `AdvanceApi/Services/ContactoUsuarioService.cs` (línea 34)
- **Propósito**: Obtiene información de contacto de un usuario
- **Parámetros**:
  - `@usuario`: Nombre de usuario
- **Retorna**: Información completa del usuario (credencial_id, nombre, correo, teléfono, nivel, tipo)
- **Usado en**: Método `GetContactoUsuarioAsync()` del ContactoUsuarioService

---

## 10. Logging

### 10.1 sp_InsertLog
- **Archivo**: `AdvanceApi/Services/LoggingService.cs` (línea 39)
- **Propósito**: Inserta entradas de log en la base de datos
- **Parámetros**:
  - `@Id`: ID único del log
  - `@Level`: Nivel de log (Info, Warning, Error, etc.)
  - `@Message`: Mensaje del log (opcional)
  - `@Exception`: Excepción capturada (opcional)
  - `@StackTrace`: Stack trace de la excepción (opcional)
  - `@Source`: Origen del log (opcional)
  - `@Method`: Método que generó el log (opcional)
  - `@Username`: Usuario asociado (opcional)
  - `@MachineName`: Nombre de la máquina (opcional)
  - `@AppVersion`: Versión de la aplicación (opcional)
  - `@Timestamp`: Fecha y hora del log
  - `@AdditionalData`: Datos adicionales en formato JSON (opcional)
- **Retorna**: LogId y opcionalmente AlertId
- **Usado en**: Método `LogAsync()` del LoggingService

---

## Notas Importantes

1. **Convención de Nombres**: Los procedimientos almacenados siguen principalmente dos patrones:
   - `sp_[entidad]_[acción]`: Para operaciones específicas (ej: sp_cliente_select, sp_operacion_select)
   - `sp_[entidad]Edit`: Para operaciones CRUD múltiples con parámetro @operacion (ej: sp_equipo_edit, sp_refaccion_edit)

2. **Seguridad**: 
   - Todos los procedimientos utilizan `CommandType.StoredProcedure`
   - Los parámetros se agregan usando `AddWithValue` o `Add` con tipos específicos
   - Las contraseñas se pasan de forma segura al procedimiento `login_credencial`

3. **Manejo de Valores Nulos**: 
   - La mayoría de parámetros opcionales se manejan con `DBNull.Value`
   - Se utiliza el operador `??` para conversión: `(object?)valor ?? DBNull.Value`

4. **Transacciones**:
   - Las conexiones se gestionan con `await using` para asegurar su liberación
   - Se utiliza el patrón async/await en todas las operaciones de base de datos

5. **Logging**:
   - Todos los servicios implementan logging mediante ILogger
   - Se registran tanto operaciones exitosas (Debug) como errores (Error/Warning)

---

## Estadísticas

- **Total de Procedimientos Almacenados**: 16
- **Servicios que utilizan SPs**: 9
- **Controllers que utilizan SPs**: 1 (AuthController)
- **Helpers que utilizan SPs**: 1 (DbHelper)

---

**Documento generado**: 2025-12-14  
**Versión del API**: AdvanceControlApi
