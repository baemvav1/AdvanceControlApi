using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OperacionesController : ControllerBase
    {
        private readonly IOperacionService _operacionService;
        private readonly ILogger<OperacionesController> _logger;

        public OperacionesController(IOperacionService operacionService, ILogger<OperacionesController> logger)
        {
            _operacionService = operacionService ?? throw new ArgumentNullException(nameof(operacionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene operaciones según los criterios de búsqueda proporcionados
        /// GET /api/Operaciones
        /// </summary>
        /// <param name="idTipo">Filtro exacto por idTipo (0 para no filtrar)</param>
        /// <param name="idCliente">Filtro exacto por idCliente (0 para no filtrar)</param>
        /// <param name="idEquipo">Filtro exacto por idEquipo (0 para no filtrar)</param>
        /// <param name="idAtiende">Filtro exacto por idAtiende (0 para no filtrar)</param>
        /// <param name="nota">Búsqueda parcial en nota</param>
        /// <returns>Lista de operaciones que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetOperaciones(
            [FromQuery] int idTipo = 0,
            [FromQuery] int idCliente = 0,
            [FromQuery] int idEquipo = 0,
            [FromQuery] int idAtiende = 0,
            [FromQuery] string? nota = null)
        {
            try
            {
                var query = new OperacionQueryDto
                {
                    IdTipo = idTipo,
                    IdCliente = idCliente,
                    IdEquipo = idEquipo,
                    IdAtiende = idAtiende,
                    Nota = nota
                };

                var operaciones = await _operacionService.GetOperacionesAsync(query);

                return Ok(operaciones);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener operaciones");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener operaciones");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) una operación
        /// DELETE /api/Operaciones
        /// </summary>
        /// <param name="idOperacion">ID de la operación (obligatorio)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteOperacion(
            [FromQuery] int idOperacion)
        {
            try
            {
                if (idOperacion <= 0)
                {
                    return BadRequest(new { message = "El campo 'idOperacion' debe ser mayor que 0." });
                }

                var result = await _operacionService.DeleteOperacionAsync(idOperacion);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar operación");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar operación");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
