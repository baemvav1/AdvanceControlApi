using AdvanceApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    /// <summary>
    /// Controlador para gestionar sesiones de usuario (refresh tokens)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController : ControllerBase
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<SessionsController> _logger;

        public SessionsController(DbHelper dbHelper, ILogger<SessionsController> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene el número de sesiones activas (refresh tokens no revocados) para un usuario
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <returns>Número de sesiones activas</returns>
        /// <response code="200">Retorna el número de sesiones activas</response>
        /// <response code="400">Si el nombre de usuario no es válido</response>
        /// <response code="500">Si ocurre un error en el servidor</response>
        [HttpGet("active-count/{username}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveSessionsCount(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(new { success = false, message = "Username es requerido." });

            try
            {
                var count = await _dbHelper.CountActiveRefreshTokensForUserAsync(username);

                return Ok(new
                {
                    success = true,
                    username,
                    activeSessionsCount = count
                });
            }
            catch (Exception ex)
            {
                // Sanitize username for logging to prevent log forging
                var sanitizedUsername = username.Replace("\n", "").Replace("\r", "");
                _logger.LogError(ex, "Error al obtener el conteo de sesiones activas para el usuario {Username}", sanitizedUsername);
                return StatusCode(500, new { success = false, message = "Error al obtener el conteo de sesiones activas." });
            }
        }
    }
}
