using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContactoController : ControllerBase
    {
        private readonly IContactoService _contactoService;
        private readonly ILogger<ContactoController> _logger;

        public ContactoController(IContactoService contactoService, ILogger<ContactoController> logger)
        {
            _contactoService = contactoService ?? throw new ArgumentNullException(nameof(contactoService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene contactos según los criterios de búsqueda proporcionados
        /// GET /api/Contacto
        /// </summary>
        /// <param name="contactoId">ID del contacto específico</param>
        /// <param name="credencialId">Búsqueda por credencial ID</param>
        /// <param name="nombre">Búsqueda parcial por nombre</param>
        /// <param name="apellido">Búsqueda parcial por apellido</param>
        /// <param name="correo">Búsqueda parcial por correo</param>
        /// <param name="telefono">Búsqueda parcial por teléfono</param>
        /// <param name="departamento">Búsqueda parcial por departamento</param>
        /// <param name="codigoInterno">Búsqueda parcial por código interno</param>
        /// <param name="idProveedor">Búsqueda por ID de proveedor</param>
        /// <param name="cargo">Búsqueda parcial por cargo</param>
        /// <param name="idCliente">Búsqueda por ID de cliente</param>
        /// <returns>Lista de contactos que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetContactos(
            [FromQuery] long? contactoId = null,
            [FromQuery] long? credencialId = null,
            [FromQuery] string? nombre = null,
            [FromQuery] string? apellido = null,
            [FromQuery] string? correo = null,
            [FromQuery] string? telefono = null,
            [FromQuery] string? departamento = null,
            [FromQuery] string? codigoInterno = null,
            [FromQuery] int? idProveedor = null,
            [FromQuery] string? cargo = null,
            [FromQuery] int? idCliente = null)
        {
            try
            {
                var query = new ContactoEditDto
                {
                    ContactoId = contactoId ?? 0,
                    CredencialId = credencialId,
                    Nombre = nombre,
                    Apellido = apellido,
                    Correo = correo,
                    Telefono = telefono,
                    Departamento = departamento,
                    CodigoInterno = codigoInterno,
                    IdProveedor = idProveedor,
                    Cargo = cargo,
                    IdCliente = idCliente
                };

                var contactos = await _contactoService.GetContactosAsync(query);

                return Ok(contactos);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener contactos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener contactos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo contacto
        /// POST /api/Contacto
        /// </summary>
        /// <param name="nombre">Nombre del contacto (obligatorio)</param>
        /// <param name="credencialId">ID de credencial asociada</param>
        /// <param name="apellido">Apellido del contacto</param>
        /// <param name="correo">Correo electrónico</param>
        /// <param name="telefono">Teléfono</param>
        /// <param name="departamento">Departamento</param>
        /// <param name="codigoInterno">Código interno</param>
        /// <param name="activo">Estado activo (por defecto true)</param>
        /// <param name="notas">Notas adicionales</param>
        /// <param name="idProveedor">ID del proveedor asociado</param>
        /// <param name="cargo">Cargo del contacto</param>
        /// <param name="idCliente">ID del cliente asociado</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateContacto(
            [FromQuery] string nombre,
            [FromQuery] long? credencialId = null,
            [FromQuery] string? apellido = null,
            [FromQuery] string? correo = null,
            [FromQuery] string? telefono = null,
            [FromQuery] string? departamento = null,
            [FromQuery] string? codigoInterno = null,
            [FromQuery] bool? activo = true,
            [FromQuery] string? notas = null,
            [FromQuery] int? idProveedor = null,
            [FromQuery] string? cargo = null,
            [FromQuery] int? idCliente = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return BadRequest(new { message = "El campo 'nombre' es obligatorio." });
                }

                var query = new ContactoEditDto
                {
                    CredencialId = credencialId,
                    Nombre = nombre,
                    Apellido = apellido,
                    Correo = correo,
                    Telefono = telefono,
                    Departamento = departamento,
                    CodigoInterno = codigoInterno,
                    Activo = activo,
                    Notas = notas,
                    IdProveedor = idProveedor,
                    Cargo = cargo,
                    IdCliente = idCliente
                };

                var result = await _contactoService.CreateContactoAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear contacto");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear contacto");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza un contacto existente
        /// PUT /api/Contacto/{id}
        /// </summary>
        /// <param name="id">ID del contacto a actualizar</param>
        /// <param name="credencialId">Nueva credencial ID</param>
        /// <param name="nombre">Nuevo nombre</param>
        /// <param name="apellido">Nuevo apellido</param>
        /// <param name="correo">Nuevo correo</param>
        /// <param name="telefono">Nuevo teléfono</param>
        /// <param name="departamento">Nuevo departamento</param>
        /// <param name="codigoInterno">Nuevo código interno</param>
        /// <param name="activo">Nuevo estado activo</param>
        /// <param name="notas">Nuevas notas</param>
        /// <param name="idProveedor">Nuevo ID de proveedor</param>
        /// <param name="cargo">Nuevo cargo</param>
        /// <param name="idCliente">Nuevo ID de cliente</param>
        /// <param name="estatus">Nuevo estatus</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContacto(
            long id,
            [FromQuery] long? credencialId = null,
            [FromQuery] string? nombre = null,
            [FromQuery] string? apellido = null,
            [FromQuery] string? correo = null,
            [FromQuery] string? telefono = null,
            [FromQuery] string? departamento = null,
            [FromQuery] string? codigoInterno = null,
            [FromQuery] bool? activo = null,
            [FromQuery] string? notas = null,
            [FromQuery] int? idProveedor = null,
            [FromQuery] string? cargo = null,
            [FromQuery] int? idCliente = null,
            [FromQuery] bool? estatus = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var query = new ContactoEditDto
                {
                    ContactoId = id,
                    CredencialId = credencialId,
                    Nombre = nombre,
                    Apellido = apellido,
                    Correo = correo,
                    Telefono = telefono,
                    Departamento = departamento,
                    CodigoInterno = codigoInterno,
                    Activo = activo,
                    Notas = notas,
                    IdProveedor = idProveedor,
                    Cargo = cargo,
                    IdCliente = idCliente,
                    Estatus = estatus
                };

                var result = await _contactoService.UpdateContactoAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar contacto");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar contacto");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) un contacto por su ID
        /// DELETE /api/Contacto/{id}
        /// </summary>
        /// <param name="id">ID del contacto a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContacto(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var result = await _contactoService.DeleteContactoAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar contacto");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar contacto");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
