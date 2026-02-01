using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GoogleMapsConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleMapsConfigController> _logger;

        public GoogleMapsConfigController(IConfiguration configuration, ILogger<GoogleMapsConfigController> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene la clave de API de Google Maps
        /// GET /api/GoogleMapsConfig/api-key
        /// </summary>
        [HttpGet("api-key")]
        public IActionResult GetApiKey()
        {
            try
            {
                var apiKey = _configuration["GoogleMaps:ApiKey"];

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    _logger.LogWarning("La clave de API de Google Maps no está configurada");
                    return NotFound(new { message = "La clave de API de Google Maps no está configurada" });
                }

                return Ok(new { apiKey });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la clave de API de Google Maps");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Obtiene toda la configuración de Google Maps
        /// GET /api/GoogleMapsConfig
        /// </summary>
        [HttpGet]
        public IActionResult GetConfig()
        {
            try
            {
                var apiKey = _configuration["GoogleMaps:ApiKey"];
                var defaultCenter = _configuration["GoogleMaps:DefaultCenter"];
                var defaultZoom = _configuration["GoogleMaps:DefaultZoom"];

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    _logger.LogWarning("La clave de API de Google Maps no está configurada");
                    return NotFound(new { message = "La configuración de Google Maps no está completa" });
                }

                return Ok(new
                {
                    apiKey,
                    defaultCenter,
                    defaultZoom = !string.IsNullOrWhiteSpace(defaultZoom) ? int.Parse(defaultZoom) : 15
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la configuración de Google Maps");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
