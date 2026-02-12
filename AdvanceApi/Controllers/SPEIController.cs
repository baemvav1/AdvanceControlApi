using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/spei")]
    [Authorize]
    public class SPEIController : ControllerBase
    {
        private readonly ITransferenciaSPEIService _speiService;
        private readonly ILogger<SPEIController> _logger;

        public SPEIController(ITransferenciaSPEIService speiService, ILogger<SPEIController> logger)
        {
            _speiService = speiService ?? throw new ArgumentNullException(nameof(speiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea una nueva transferencia SPEI
        /// POST /api/spei
        /// </summary>
        /// <param name="dto">Datos de la transferencia SPEI a crear</param>
        /// <returns>Resultado de la operación con el ID de la transferencia creada</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTransferenciaSPEI([FromBody] TransferenciaSPEICreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { message = "Los datos de la transferencia son obligatorios." });
                }

                if (dto.IdMovimiento <= 0)
                {
                    return BadRequest(new { message = "El campo 'idMovimiento' es obligatorio y debe ser mayor a 0." });
                }

                if (string.IsNullOrWhiteSpace(dto.TipoTransferencia))
                {
                    return BadRequest(new { message = "El campo 'tipoTransferencia' es obligatorio." });
                }

                if (dto.TipoTransferencia != "ENVIADA" && dto.TipoTransferencia != "RECIBIDA")
                {
                    return BadRequest(new { message = "El tipo de transferencia debe ser ENVIADA o RECIBIDA." });
                }

                if (dto.Monto <= 0)
                {
                    return BadRequest(new { message = "El campo 'monto' debe ser mayor a 0." });
                }

                var result = await _speiService.CrearTransferenciaSPEIAsync(dto);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear transferencia SPEI");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear transferencia SPEI");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Consulta transferencias SPEI según los criterios de búsqueda proporcionados
        /// GET /api/spei
        /// </summary>
        /// <param name="idMovimiento">ID del movimiento (opcional)</param>
        /// <param name="tipoTransferencia">Tipo de transferencia: ENVIADA o RECIBIDA (opcional)</param>
        /// <param name="claveRastreo">Clave de rastreo (opcional)</param>
        /// <param name="rfcEmisor">RFC del emisor (opcional)</param>
        /// <param name="rfcDestinatario">RFC del destinatario (opcional)</param>
        /// <param name="fechaInicio">Fecha inicio del período (opcional)</param>
        /// <param name="fechaFin">Fecha fin del período (opcional)</param>
        /// <returns>Lista de transferencias SPEI que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetTransferenciasSPEI(
            [FromQuery] int? idMovimiento = null,
            [FromQuery] string? tipoTransferencia = null,
            [FromQuery] string? claveRastreo = null,
            [FromQuery] string? rfcEmisor = null,
            [FromQuery] string? rfcDestinatario = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            try
            {
                var query = new TransferenciaSPEIQueryDto
                {
                    IdMovimiento = idMovimiento,
                    TipoTransferencia = tipoTransferencia,
                    ClaveRastreo = claveRastreo,
                    RfcEmisor = rfcEmisor,
                    RfcDestinatario = rfcDestinatario,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                var transferencias = await _speiService.ConsultarTransferenciasSPEIAsync(query);

                return Ok(transferencias);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al consultar transferencias SPEI");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar transferencias SPEI");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
