# Referencia Rápida de Procedimientos Almacenados / Stored Procedures Quick Reference

## Listado Completo / Complete List

| # | Procedimiento / Procedure | Categoría / Category | Archivo / File | Operaciones / Operations |
|---|---------------------------|----------------------|----------------|--------------------------|
| 1 | `login_credencial` | Autenticación / Authentication | AuthController.cs | Login validation |
| 2 | `InsertRefreshToken` | Autenticación / Authentication | DbHelper.cs | Insert refresh token |
| 3 | `GetRefreshTokenByHash` | Autenticación / Authentication | DbHelper.cs | Get token by hash |
| 4 | `RevokeRefreshTokenById` | Autenticación / Authentication | DbHelper.cs | Revoke token by ID |
| 5 | `RevokeAllRefreshTokensForUser` | Autenticación / Authentication | DbHelper.cs | Revoke all user tokens |
| 6 | `CountActiveRefreshTokensForUser` | Autenticación / Authentication | DbHelper.cs | Count active tokens |
| 7 | `sp_cliente_select` | Clientes / Clients | ClienteService.cs | Select clients |
| 8 | `sp_equipo_edit` | Equipos / Equipment | EquipoService.cs | Select, Update, Delete |
| 9 | `sp_equipo_create` | Equipos / Equipment | EquipoService.cs | Create equipment |
| 10 | `sp_refaccion_edit` | Refacciones / Parts | RefaccionService.cs | Select, Update, Delete, Create |
| 11 | `sp_MatenimientoEdit` * | Mantenimientos / Maintenance | MantenimientoService.cs | Select, Create, Delete |
| 12 | `sp_operacion_select` | Operaciones / Operations | OperacionService.cs | Select operations |
| 13 | `sp_relacionEquipoCliente_edit` | Relaciones / Relations | RelacionEquipoClienteService.cs | Select, Create, Delete, Update Note |
| 14 | `sp_relacionRefaccionEquipo_edit` | Relaciones / Relations | RelacionRefaccionEquipoService.cs | Select Parts/Equipment, Create, Delete, Update Note |
| 15 | `sp_contacto_usuario_select` | Usuarios / Users | ContactoUsuarioService.cs | Select user contact info |
| 16 | `sp_InsertLog` | Logging | LoggingService.cs | Insert log entry |

## Resumen por Categoría / Summary by Category

| Categoría / Category | Cantidad / Count | Procedimientos / Procedures |
|----------------------|------------------|----------------------------|
| Autenticación y Seguridad / Authentication & Security | 6 | login_credencial, InsertRefreshToken, GetRefreshTokenByHash, RevokeRefreshTokenById, RevokeAllRefreshTokensForUser, CountActiveRefreshTokensForUser |
| Gestión de Clientes / Client Management | 1 | sp_cliente_select |
| Gestión de Equipos / Equipment Management | 2 | sp_equipo_edit, sp_equipo_create |
| Gestión de Refacciones / Parts Management | 1 | sp_refaccion_edit |
| Gestión de Mantenimientos / Maintenance Management | 1 | sp_MatenimientoEdit |
| Gestión de Operaciones / Operations Management | 1 | sp_operacion_select |
| Relaciones / Relations | 2 | sp_relacionEquipoCliente_edit, sp_relacionRefaccionEquipo_edit |
| Información de Usuarios / User Information | 1 | sp_contacto_usuario_select |
| Logging | 1 | sp_InsertLog |

## Patrones de Nombres / Naming Patterns

### Patrón 1: `sp_[entidad]_[acción]`
Utilizado para operaciones específicas:
- `sp_cliente_select` - Selecciona clientes
- `sp_operacion_select` - Selecciona operaciones
- `sp_contacto_usuario_select` - Selecciona información de usuario
- `sp_equipo_create` - Crea equipos

### Patrón 2: `sp_[entidad]Edit`
Utilizado para operaciones CRUD múltiples con parámetro `@operacion`:
- `sp_equipo_edit` (operaciones: select, update, delete)
- `sp_refaccion_edit` (operaciones: select, update, delete, put)
- `sp_MatenimientoEdit` (operaciones: select, put, delete)
- `sp_relacionEquipoCliente_edit` (operaciones: select, put, delete, update_nota)
- `sp_relacionRefaccionEquipo_edit` (operaciones: select_refacciones, select_equipos, put, delete, update_nota)

### Patrón 3: Nombres descriptivos sin prefijo `sp_`
Utilizado para operaciones de autenticación y seguridad:
- `login_credencial` - Valida credenciales
- `InsertRefreshToken` - Inserta token de refresco
- `GetRefreshTokenByHash` - Obtiene token por hash
- `RevokeRefreshTokenById` - Revoca token por ID
- `RevokeAllRefreshTokensForUser` - Revoca todos los tokens de un usuario
- `CountActiveRefreshTokensForUser` - Cuenta tokens activos

### Patrón 4: `sp_[Acción][Entidad]`
Utilizado para operaciones específicas:
- `sp_InsertLog` - Inserta entrada de log

## Operaciones CRUD / CRUD Operations

| Operación / Operation | Procedimientos / Procedures |
|----------------------|----------------------------|
| **Create / Crear** | sp_equipo_create, sp_refaccion_edit (@operacion='put'), sp_MatenimientoEdit (@operacion='put'), sp_relacionEquipoCliente_edit (@operacion='put'), sp_relacionRefaccionEquipo_edit (@operacion='put'), InsertRefreshToken, sp_InsertLog |
| **Read / Leer** | sp_cliente_select, sp_equipo_edit (@operacion='select'), sp_refaccion_edit (@operacion='select'), sp_MatenimientoEdit (@operacion='select'), sp_operacion_select, sp_relacionEquipoCliente_edit (@operacion='select'), sp_relacionRefaccionEquipo_edit (@operacion='select_*'), sp_contacto_usuario_select, GetRefreshTokenByHash |
| **Update / Actualizar** | sp_equipo_edit (@operacion='update'), sp_refaccion_edit (@operacion='update'), sp_relacionEquipoCliente_edit (@operacion='update_nota'), sp_relacionRefaccionEquipo_edit (@operacion='update_nota') |
| **Delete / Eliminar** | sp_equipo_edit (@operacion='delete'), sp_refaccion_edit (@operacion='delete'), sp_MatenimientoEdit (@operacion='delete'), sp_relacionEquipoCliente_edit (@operacion='delete'), sp_relacionRefaccionEquipo_edit (@operacion='delete'), RevokeRefreshTokenById, RevokeAllRefreshTokensForUser |

## Documentación Completa / Full Documentation

Para información detallada sobre cada procedimiento almacenado, incluyendo parámetros, tipos de datos, valores de retorno y ejemplos de uso, consulte:

For detailed information about each stored procedure, including parameters, data types, return values, and usage examples, please refer to:

- **Español**: [LISTADO_PROCEDIMIENTOS_ALMACENADOS.md](./LISTADO_PROCEDIMIENTOS_ALMACENADOS.md)
- **English**: [STORED_PROCEDURES_LIST.md](./STORED_PROCEDURES_LIST.md)

---

**Total de Procedimientos Almacenados / Total Stored Procedures**: 16

---

## Notas / Notes

\* `sp_MatenimientoEdit` y `@descricpion`: Estos nombres tienen errores tipográficos en el código actual del API. Se documentan tal como están implementados.

\* `sp_MatenimientoEdit` and `@descricpion`: These names have typos in the actual API code. They are documented as implemented.

---

**Documento generado / Document generated**: 2025-12-14  
**Versión del API / API Version**: AdvanceControlApi
