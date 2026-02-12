using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/pagoServicios")]
    [Authorize]
    public class PagoServiciosController : ControllerBase
    {
        private readonly IPagoServicioService _pagoServicioService;
        private readonly ILogger<PagoServiciosController> _logger;

        public PagoServiciosController(IPagoServicioService pagoServicioService, ILogger<PagoServiciosController> logger)
        {
            _pagoServicioService = pagoServicioService ?? throw new ArgumentNullException(nameof(pagoServicioService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Consulta pagos de servicio según los criterios de búsqueda proporcionados
        /// GET /api/pagoServicios
        /// </summary>
        /// <param name="idMovimiento">ID del movimiento (opcional)</param>
        /// <param name="tipoServicio">Tipo de servicio (opcional)</param>
        /// <param name="fechaInicio">Fecha inicio del período (opcional)</param>
        /// <param name="fechaFin">Fecha fin del período (opcional)</param>
        /// <returns>Lista de pagos de servicio que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetPagosServicio(
            [FromQuery] int? idMovimiento = null,
            [FromQuery] string? tipoServicio = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            try
            {
                var pagosServicio = await _pagoServicioService.ConsultarPagosServicioAsync(idMovimiento, tipoServicio, fechaInicio, fechaFin);

                return Ok(pagosServicio);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener pagos de servicio");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener pagos de servicio");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo pago de servicio
        /// POST /api/pagoServicios
        /// </summary>
        /// <param name="idMovimiento">ID del movimiento (obligatorio)</param>
        /// <param name="tipoServicio">Tipo de servicio (obligatorio)</param>
        /// <param name="monto">Monto del pago (obligatorio)</param>
        /// <param name="referencia">Referencia del pago (opcional)</param>
        /// <returns>Resultado de la operación con el ID del pago creado</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePagoServicio(
            [FromQuery] int idMovimiento,
            [FromQuery] string tipoServicio,
            [FromQuery] decimal monto,
            [FromQuery] string? referencia = null)
        {
            try
            {
                if (idMovimiento <= 0)
                {
                    return BadRequest(new { message = "El campo 'idMovimiento' es obligatorio y debe ser mayor a 0." });
                }

                if (string.IsNullOrWhiteSpace(tipoServicio))
                {
                    return BadRequest(new { message = "El campo 'tipoServicio' es obligatorio." });
                }

                if (monto <= 0)
                {
                    return BadRequest(new { message = "El campo 'monto' debe ser mayor a 0." });
                }

                var query = new PagoServicioQueryDto
                {
                    IdMovimiento = idMovimiento,
                    TipoServicio = tipoServicio,
                    Referencia = referencia,
                    Monto = monto
                };

                var result = await _pagoServicioService.CrearPagoServicioAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear pago de servicio");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear pago de servicio");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
