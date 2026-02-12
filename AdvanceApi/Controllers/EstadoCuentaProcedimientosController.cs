using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EstadoCuentaProcedimientosController : ControllerBase
    {
        private readonly IEstadoCuentaProcedimientosService _service;
        private readonly ILogger<EstadoCuentaProcedimientosController> _logger;

        public EstadoCuentaProcedimientosController(
            IEstadoCuentaProcedimientosService service,
            ILogger<EstadoCuentaProcedimientosController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Ejecuta cualquiera de los procedimientos almacenados definidos en ProcedimientosEstadoCuenta.sql
        /// </summary>
        /// <param name="request">Nombre del procedimiento y parámetros a enviar.</param>
        /// <returns>Conjuntos de resultados devueltos por el procedimiento.</returns>
        [HttpPost("ejecutar")]
        public async Task<IActionResult> Ejecutar([FromBody] ProcedimientoEstadoCuentaRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Procedimiento))
            {
                return BadRequest(new { message = "Debe indicar el procedimiento a ejecutar." });
            }

            try
            {
                var resultado = await _service.EjecutarAsync(request);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Solicitud inválida al ejecutar el procedimiento {Procedimiento}", request.Procedimiento);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al ejecutar el procedimiento {Procedimiento}", request.Procedimiento);
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno al ejecutar el procedimiento." });
#endif
            }
        }
    }
}
