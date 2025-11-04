using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    /// <summary>
    /// Controlador para el registro de logs desde aplicaciones cliente
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LoggingController : ControllerBase
    {
        private readonly ILoggingService _loggingService;
        private readonly ILogger<LoggingController> _logger;

        public LoggingController(ILoggingService loggingService, ILogger<LoggingController> logger)
        {
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Registra una entrada de log en la base de datos
        /// </summary>
        /// <param name="logEntry">Datos del log a registrar</param>
        /// <returns>Resultado de la operación con el ID del log</returns>
        [HttpPost("log")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Log([FromBody] LogEntryDto logEntry)
        {
            // Validar que el body no sea null
            if (logEntry == null)
                return BadRequest(new { success = false, message = "Log entry is required" });

            // Validar ModelState (validaciones de DataAnnotations)
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(new { success = false, message = "Invalid log data", errors });
            }

            try
            {
                // Delegar la lógica al servicio
                var (logId, alertId) = await _loggingService.LogAsync(logEntry);

                var response = new
                {
                    success = true,
                    message = "Log registrado correctamente",
                    logId,
                    alertId
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                // Error de negocio o de base de datos
                _logger.LogError(ex, "Error al procesar el log");
                return StatusCode(500, new { success = false, message = "Error al procesar el log" });
            }
            catch (Exception ex)
            {
                // Error inesperado - no exponer detalles al cliente
                _logger.LogError(ex, "Error inesperado al procesar el log");
                return StatusCode(500, new { success = false, message = "Error al procesar el log" });
            }
        }
    }
}
