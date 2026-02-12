using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BancoCtaHabienteController : ControllerBase
    {
        private readonly IBancoCtaHabienteService _bancoCtaHabienteService;
        private readonly ILogger<BancoCtaHabienteController> _logger;

        public BancoCtaHabienteController(IBancoCtaHabienteService bancoCtaHabienteService, ILogger<BancoCtaHabienteController> logger)
        {
            _bancoCtaHabienteService = bancoCtaHabienteService ?? throw new ArgumentNullException(nameof(bancoCtaHabienteService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // =============================================
        // BANCO ENDPOINTS
        // =============================================

        /// <summary>
        /// Crea un nuevo banco
        /// POST /api/BancoCtaHabiente/banco
        /// </summary>
        /// <param name="nombreBanco">Nombre del banco (obligatorio)</param>
        /// <param name="rfc">RFC del banco (obligatorio)</param>
        /// <param name="nombreSucursal">Nombre de la sucursal</param>
        /// <param name="direccion">Dirección del banco</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("banco")]
        public async Task<IActionResult> CreateBanco(
            [FromQuery] string nombreBanco,
            [FromQuery] string rfc,
            [FromQuery] string? nombreSucursal = null,
            [FromQuery] string? direccion = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombreBanco))
                {
                    return BadRequest(new { message = "El campo 'nombreBanco' es obligatorio." });
                }

                if (string.IsNullOrWhiteSpace(rfc))
                {
                    return BadRequest(new { message = "El campo 'rfc' es obligatorio." });
                }

                var bancoDto = new BancoDto
                {
                    NombreBanco = nombreBanco,
                    Rfc = rfc,
                    NombreSucursal = nombreSucursal,
                    Direccion = direccion
                };

                var result = await _bancoCtaHabienteService.CreateBancoAsync(bancoDto);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear banco");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear banco");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Obtiene bancos según los criterios de búsqueda proporcionados
        /// GET /api/BancoCtaHabiente/banco
        /// </summary>
        /// <param name="idBanco">ID del banco</param>
        /// <param name="rfc">RFC del banco</param>
        /// <returns>Lista de bancos que cumplen con los criterios</returns>
        [HttpGet("banco")]
        public async Task<IActionResult> GetBancos(
            [FromQuery] int? idBanco = null,
            [FromQuery] string? rfc = null)
        {
            try
            {
                var query = new BancoDto
                {
                    IdBanco = idBanco,
                    Rfc = rfc
                };

                var bancos = await _bancoCtaHabienteService.GetBancosAsync(query);

                return Ok(bancos);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener bancos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener bancos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        // =============================================
        // CUENTA HABIENTE ENDPOINTS
        // =============================================

        /// <summary>
        /// Crea una nueva cuenta habiente
        /// POST /api/BancoCtaHabiente/cuentahabiente
        /// </summary>
        /// <param name="nombre">Nombre de la cuenta habiente (obligatorio)</param>
        /// <param name="rfc">RFC de la cuenta habiente (obligatorio)</param>
        /// <param name="numeroCuenta">Número de cuenta (obligatorio)</param>
        /// <param name="direccion">Dirección</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("cuentahabiente")]
        public async Task<IActionResult> CreateCuentaHabiente(
            [FromQuery] string nombre,
            [FromQuery] string rfc,
            [FromQuery] string numeroCuenta,
            [FromQuery] string? direccion = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return BadRequest(new { message = "El campo 'nombre' es obligatorio." });
                }

                if (string.IsNullOrWhiteSpace(rfc))
                {
                    return BadRequest(new { message = "El campo 'rfc' es obligatorio." });
                }

                if (string.IsNullOrWhiteSpace(numeroCuenta))
                {
                    return BadRequest(new { message = "El campo 'numeroCuenta' es obligatorio." });
                }

                var cuentaHabienteDto = new CuentaHabienteDto
                {
                    Nombre = nombre,
                    Rfc = rfc,
                    NumeroCuenta = numeroCuenta,
                    Direccion = direccion
                };

                var result = await _bancoCtaHabienteService.CreateCuentaHabienteAsync(cuentaHabienteDto);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear cuenta habiente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear cuenta habiente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Obtiene cuentas habiente según los criterios de búsqueda proporcionados
        /// GET /api/BancoCtaHabiente/cuentahabiente
        /// </summary>
        /// <param name="idCuentaHabiente">ID de la cuenta habiente</param>
        /// <param name="numeroCuenta">Número de cuenta</param>
        /// <param name="rfc">RFC de la cuenta habiente</param>
        /// <returns>Lista de cuentas habiente que cumplen con los criterios</returns>
        [HttpGet("cuentahabiente")]
        public async Task<IActionResult> GetCuentasHabiente(
            [FromQuery] int? idCuentaHabiente = null,
            [FromQuery] string? numeroCuenta = null,
            [FromQuery] string? rfc = null)
        {
            try
            {
                var query = new CuentaHabienteDto
                {
                    IdCuentaHabiente = idCuentaHabiente,
                    NumeroCuenta = numeroCuenta,
                    Rfc = rfc
                };

                var cuentasHabiente = await _bancoCtaHabienteService.GetCuentasHabienteAsync(query);

                return Ok(cuentasHabiente);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener cuentas habiente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener cuentas habiente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
