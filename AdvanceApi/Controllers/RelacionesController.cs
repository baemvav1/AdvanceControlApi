using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RelacionesController : ControllerBase
    {
        private readonly IRelacionEquipoClienteService _relacionService;
        private readonly ILogger<RelacionesController> _logger;

        public RelacionesController(IRelacionEquipoClienteService relacionService, ILogger<RelacionesController> logger)
        {
            _relacionService = relacionService ?? throw new ArgumentNullException(nameof(relacionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene relaciones equipo-cliente según los criterios de búsqueda proporcionados
        /// GET /api/Relaciones
        /// </summary>
        /// <param name="identificador">Búsqueda parcial por identificador del equipo</param>
        /// <param name="idCliente">Filtro exacto por ID de cliente (0 para no filtrar)</param>
        /// <returns>Lista de relaciones que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetRelaciones(
            [FromQuery] string? identificador = null,
            [FromQuery] int idCliente = 0)
        {
            try
            {
                var query = new RelacionEquipoClienteQueryDto
                {
                    Operacion = "select",
                    Identificador = identificador,
                    IdCliente = idCliente
                };

                var relaciones = await _relacionService.GetRelacionesAsync(query);

                return Ok(relaciones);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener relaciones equipo-cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener relaciones equipo-cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea una nueva relación equipo-cliente
        /// POST /api/Relaciones
        /// </summary>
        /// <param name="identificador">Identificador del equipo (obligatorio)</param>
        /// <param name="idCliente">ID del cliente (obligatorio, mayor que 0)</param>
        /// <param name="nota">Nota asociada a la relación (opcional)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateRelacion(
            [FromQuery] string identificador,
            [FromQuery] int idCliente,
            [FromQuery] string? nota = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identificador))
                {
                    return BadRequest(new { message = "El campo 'identificador' es obligatorio." });
                }

                if (idCliente <= 0)
                {
                    return BadRequest(new { message = "El campo 'idCliente' debe ser mayor que 0." });
                }

                var query = new RelacionEquipoClienteQueryDto
                {
                    Operacion = "put",
                    Identificador = identificador,
                    IdCliente = idCliente,
                    Nota = nota
                };

                var result = await _relacionService.CreateRelacionAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear relación equipo-cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear relación equipo-cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) una relación equipo-cliente
        /// DELETE /api/Relaciones
        /// </summary>
        /// <param name="identificador">Identificador del equipo (obligatorio)</param>
        /// <param name="idCliente">ID del cliente (obligatorio, mayor que 0)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteRelacion(
            [FromQuery] string identificador,
            [FromQuery] int idCliente)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identificador))
                {
                    return BadRequest(new { message = "El campo 'identificador' es obligatorio." });
                }

                if (idCliente <= 0)
                {
                    return BadRequest(new { message = "El campo 'idCliente' debe ser mayor que 0." });
                }

                var result = await _relacionService.DeleteRelacionAsync(identificador, idCliente);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar relación equipo-cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar relación equipo-cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza la nota de una relación equipo-cliente
        /// PUT /api/Relaciones/nota
        /// </summary>
        /// <param name="identificador">Identificador del equipo (obligatorio)</param>
        /// <param name="idCliente">ID del cliente (obligatorio, mayor que 0)</param>
        /// <param name="nota">Nueva nota (opcional, puede ser null para limpiar)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("nota")]
        public async Task<IActionResult> UpdateNota(
            [FromQuery] string identificador,
            [FromQuery] int idCliente,
            [FromQuery] string? nota = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identificador))
                {
                    return BadRequest(new { message = "El campo 'identificador' es obligatorio." });
                }

                if (idCliente <= 0)
                {
                    return BadRequest(new { message = "El campo 'idCliente' debe ser mayor que 0." });
                }

                var query = new RelacionEquipoClienteQueryDto
                {
                    Operacion = "update_nota",
                    Identificador = identificador,
                    IdCliente = idCliente,
                    Nota = nota
                };

                var result = await _relacionService.UpdateNotaAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar nota de relación equipo-cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar nota de relación equipo-cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
