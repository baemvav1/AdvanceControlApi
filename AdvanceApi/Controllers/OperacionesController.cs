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
        /// <param name="idTipo">Filtro exacto por idTipo</param>
        /// <param name="idCliente">Filtro exacto por idCliente</param>
        /// <param name="estatus">Filtro exacto por estatus</param>
        /// <returns>Lista de operaciones que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetOperaciones(
            [FromQuery] int? idTipo = null,
            [FromQuery] int? idCliente = null,
            [FromQuery] bool? estatus = null)
        {
            try
            {
                var query = new OperacionQueryDto
                {
                    IdTipo = idTipo,
                    IdCliente = idCliente,
                    Estatus = estatus
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
    }
}
