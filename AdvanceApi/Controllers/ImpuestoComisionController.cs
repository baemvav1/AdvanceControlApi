using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/impuesto_comision")]
    [Authorize]
    public class ImpuestoComisionController : ControllerBase
    {
        private readonly IImpuestoComisionService _impuestoComisionService;
        private readonly ILogger<ImpuestoComisionController> _logger;

        public ImpuestoComisionController(IImpuestoComisionService impuestoComisionService, ILogger<ImpuestoComisionController> logger)
        {
            _impuestoComisionService = impuestoComisionService ?? throw new ArgumentNullException(nameof(impuestoComisionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Consulta impuestos de movimiento según los criterios de búsqueda proporcionados
        /// GET /api/impuesto_comision/impuestos
        /// </summary>
        /// <param name="idMovimiento">ID del movimiento (opcional)</param>
        /// <param name="tipoImpuesto">Tipo de impuesto (opcional)</param>
        /// <returns>Lista de impuestos de movimiento que cumplen con los criterios</returns>
        [HttpGet("impuestos")]
        public async Task<IActionResult> GetImpuestosMovimiento(
            [FromQuery] int? idMovimiento = null,
            [FromQuery] string? tipoImpuesto = null)
        {
            try
            {
                var impuestos = await _impuestoComisionService.ConsultarImpuestosMovimientoAsync(idMovimiento, tipoImpuesto);

                return Ok(impuestos);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener impuestos de movimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener impuestos de movimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo impuesto de movimiento
        /// POST /api/impuesto_comision/impuestos
        /// </summary>
        /// <param name="idMovimiento">ID del movimiento (obligatorio)</param>
        /// <param name="tipoImpuesto">Tipo de impuesto (obligatorio)</param>
        /// <param name="monto">Monto del impuesto (obligatorio)</param>
        /// <param name="rfc">RFC (opcional)</param>
        /// <returns>Resultado de la operación con el ID del impuesto creado</returns>
        [HttpPost("impuestos")]
        public async Task<IActionResult> CreateImpuestoMovimiento(
            [FromQuery] int idMovimiento,
            [FromQuery] string tipoImpuesto,
            [FromQuery] decimal monto,
            [FromQuery] string? rfc = null)
        {
            try
            {
                if (idMovimiento <= 0)
                {
                    return BadRequest(new { message = "El campo 'idMovimiento' es obligatorio y debe ser mayor a 0." });
                }

                if (string.IsNullOrWhiteSpace(tipoImpuesto))
                {
                    return BadRequest(new { message = "El campo 'tipoImpuesto' es obligatorio." });
                }

                if (monto <= 0)
                {
                    return BadRequest(new { message = "El campo 'monto' debe ser mayor a 0." });
                }

                var dto = new ImpuestoMovimientoDto
                {
                    IdMovimiento = idMovimiento,
                    TipoImpuesto = tipoImpuesto,
                    Monto = monto,
                    Rfc = rfc
                };

                var result = await _impuestoComisionService.CrearImpuestoMovimientoAsync(dto);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear impuesto de movimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear impuesto de movimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Consulta comisiones bancarias según los criterios de búsqueda proporcionados
        /// GET /api/impuesto_comision/comisiones
        /// </summary>
        /// <param name="idMovimiento">ID del movimiento (opcional)</param>
        /// <param name="tipoComision">Tipo de comisión (opcional)</param>
        /// <param name="fechaInicio">Fecha inicio del período (opcional)</param>
        /// <param name="fechaFin">Fecha fin del período (opcional)</param>
        /// <returns>Lista de comisiones bancarias que cumplen con los criterios</returns>
        [HttpGet("comisiones")]
        public async Task<IActionResult> GetComisionesBancarias(
            [FromQuery] int? idMovimiento = null,
            [FromQuery] string? tipoComision = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            try
            {
                var comisiones = await _impuestoComisionService.ConsultarComisionesBancariasAsync(idMovimiento, tipoComision, fechaInicio, fechaFin);

                return Ok(comisiones);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener comisiones bancarias");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener comisiones bancarias");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea una nueva comisión bancaria
        /// POST /api/impuesto_comision/comisiones
        /// </summary>
        /// <param name="idMovimiento">ID del movimiento (obligatorio)</param>
        /// <param name="tipoComision">Tipo de comisión (obligatorio)</param>
        /// <param name="monto">Monto de la comisión (obligatorio)</param>
        /// <param name="iva">IVA (opcional)</param>
        /// <param name="referencia">Referencia (opcional)</param>
        /// <returns>Resultado de la operación con el ID de la comisión creada</returns>
        [HttpPost("comisiones")]
        public async Task<IActionResult> CreateComisionBancaria(
            [FromQuery] int idMovimiento,
            [FromQuery] string tipoComision,
            [FromQuery] decimal monto,
            [FromQuery] decimal? iva = null,
            [FromQuery] string? referencia = null)
        {
            try
            {
                if (idMovimiento <= 0)
                {
                    return BadRequest(new { message = "El campo 'idMovimiento' es obligatorio y debe ser mayor a 0." });
                }

                if (string.IsNullOrWhiteSpace(tipoComision))
                {
                    return BadRequest(new { message = "El campo 'tipoComision' es obligatorio." });
                }

                if (monto <= 0)
                {
                    return BadRequest(new { message = "El campo 'monto' debe ser mayor a 0." });
                }

                var dto = new ComisionBancariaDto
                {
                    IdMovimiento = idMovimiento,
                    TipoComision = tipoComision,
                    Monto = monto,
                    Iva = iva,
                    Referencia = referencia
                };

                var result = await _impuestoComisionService.CrearComisionBancariaAsync(dto);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear comisión bancaria");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear comisión bancaria");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
