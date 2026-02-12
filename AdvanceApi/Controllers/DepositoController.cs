using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/deposito")]
    [Authorize]
    public class DepositoController : ControllerBase
    {
        private readonly IDepositoService _depositoService;
        private readonly ILogger<DepositoController> _logger;

        public DepositoController(IDepositoService depositoService, ILogger<DepositoController> logger)
        {
            _depositoService = depositoService ?? throw new ArgumentNullException(nameof(depositoService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Consulta depósitos según los criterios de búsqueda proporcionados
        /// GET /api/deposito
        /// </summary>
        /// <param name="idMovimiento">ID del movimiento (opcional)</param>
        /// <param name="tipoDeposito">Tipo de depósito (opcional)</param>
        /// <param name="fechaInicio">Fecha inicio del período (opcional)</param>
        /// <param name="fechaFin">Fecha fin del período (opcional)</param>
        /// <returns>Lista de depósitos que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetDepositos(
            [FromQuery] int? idMovimiento = null,
            [FromQuery] string? tipoDeposito = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            try
            {
                var depositos = await _depositoService.ConsultarDepositosAsync(idMovimiento, tipoDeposito, fechaInicio, fechaFin);

                return Ok(depositos);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener depósitos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener depósitos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo depósito
        /// POST /api/deposito
        /// </summary>
        /// <param name="idMovimiento">ID del movimiento (obligatorio)</param>
        /// <param name="tipoDeposito">Tipo de depósito (obligatorio)</param>
        /// <param name="referencia">Referencia del depósito (opcional)</param>
        /// <param name="monto">Monto del depósito (obligatorio)</param>
        /// <returns>Resultado de la operación con el ID del depósito creado</returns>
        [HttpPost]
        public async Task<IActionResult> CreateDeposito(
            [FromQuery] int idMovimiento,
            [FromQuery] string tipoDeposito,
            [FromQuery] string? referencia = null,
            [FromQuery] decimal monto = 0)
        {
            try
            {
                if (idMovimiento <= 0)
                {
                    return BadRequest(new { message = "El campo 'idMovimiento' es obligatorio y debe ser mayor a 0." });
                }

                if (string.IsNullOrWhiteSpace(tipoDeposito))
                {
                    return BadRequest(new { message = "El campo 'tipoDeposito' es obligatorio." });
                }

                if (monto <= 0)
                {
                    return BadRequest(new { message = "El campo 'monto' es obligatorio y debe ser mayor a 0." });
                }

                var query = new DepositoQueryDto
                {
                    IdMovimiento = idMovimiento,
                    TipoDeposito = tipoDeposito,
                    Referencia = referencia,
                    Monto = monto
                };

                var result = await _depositoService.CrearDepositoAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear depósito");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear depósito");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
