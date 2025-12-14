using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MantenimientoController : ControllerBase
    {
        private readonly IMantenimientoService _mantenimientoService;
        private readonly ILogger<MantenimientoController> _logger;

        public MantenimientoController(IMantenimientoService mantenimientoService, ILogger<MantenimientoController> logger)
        {
            _mantenimientoService = mantenimientoService ?? throw new ArgumentNullException(nameof(mantenimientoService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene mantenimientos según los criterios de búsqueda proporcionados
        /// GET /api/Mantenimiento
        /// </summary>
        /// <param name="identificador">Búsqueda parcial por identificador del equipo</param>
        /// <param name="idCliente">Filtro exacto por ID de cliente (0 para no filtrar)</param>
        /// <returns>Lista de mantenimientos que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetMantenimientos(
            [FromQuery] string? identificador = null,
            [FromQuery] int idCliente = 0)
        {
            try
            {
                var query = new MantenimientoQueryDto
                {
                    Operacion = "select",
                    Identificador = identificador,
                    IdCliente = idCliente
                };

                var mantenimientos = await _mantenimientoService.GetMantenimientosAsync(query);

                return Ok(mantenimientos);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener mantenimientos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener mantenimientos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo mantenimiento
        /// POST /api/Mantenimiento
        /// </summary>
        /// <param name="idTipoMantenimiento">ID del tipo de mantenimiento (obligatorio)</param>
        /// <param name="idCliente">ID del cliente (obligatorio, mayor que 0)</param>
        /// <param name="idEquipo">ID del equipo (obligatorio, mayor que 0)</param>
        /// <param name="nota">Nota asociada al mantenimiento (opcional)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMantenimiento(
            [FromQuery] int idTipoMantenimiento,
            [FromQuery] int idCliente,
            [FromQuery] int idEquipo,
            [FromQuery] string? nota = null)
        {
            try
            {
                if (idTipoMantenimiento <= 0)
                {
                    return BadRequest(new { message = "El campo 'idTipoMantenimiento' debe ser mayor que 0." });
                }

                if (idCliente <= 0)
                {
                    return BadRequest(new { message = "El campo 'idCliente' debe ser mayor que 0." });
                }

                if (idEquipo <= 0)
                {
                    return BadRequest(new { message = "El campo 'idEquipo' debe ser mayor que 0." });
                }

                var query = new MantenimientoQueryDto
                {
                    Operacion = "put",
                    IdTipoMantenimiento = idTipoMantenimiento,
                    IdCliente = idCliente,
                    IdEquipo = idEquipo,
                    Nota = nota
                };

                var result = await _mantenimientoService.CreateMantenimientoAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear mantenimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear mantenimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) un mantenimiento
        /// DELETE /api/Mantenimiento
        /// </summary>
        /// <param name="idMantenimiento">ID del mantenimiento (obligatorio)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteMantenimiento(
            [FromQuery] int idMantenimiento)
        {
            try
            {
                if (idMantenimiento <= 0)
                {
                    return BadRequest(new { message = "El campo 'idMantenimiento' debe ser mayor que 0." });
                }

                var result = await _mantenimientoService.DeleteMantenimientoAsync(idMantenimiento);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar mantenimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar mantenimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza el estado de atendido de un mantenimiento
        /// PATCH /api/Mantenimiento/atendido
        /// </summary>
        /// <param name="idMantenimiento">ID del mantenimiento (obligatorio)</param>
        /// <param name="idAtendio">ID del usuario que atendió (obligatorio)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPatch("atendido")]
        public async Task<IActionResult> UpdateAtendido(
            [FromQuery] int idMantenimiento,
            [FromQuery] int idAtendio)
        {
            try
            {
                if (idMantenimiento <= 0)
                {
                    return BadRequest(new { message = "El campo 'idMantenimiento' debe ser mayor que 0." });
                }

                if (idAtendio <= 0)
                {
                    return BadRequest(new { message = "El campo 'idAtendio' debe ser mayor que 0." });
                }

                var result = await _mantenimientoService.UpdateAtendidoAsync(idMantenimiento, idAtendio);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar estado atendido de mantenimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar estado atendido de mantenimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
