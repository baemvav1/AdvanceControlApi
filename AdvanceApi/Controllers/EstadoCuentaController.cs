using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    /// <summary>
    /// Controlador para gestión de estados de cuenta y depósitos
    /// </summary>
    [ApiController]
    [Route("api/estadocuenta")]
    [Authorize]
    public class EstadoCuentaController : ControllerBase
    {
        private readonly IEstadoCuentaService _estadoCuentaService;
        private readonly ILogger<EstadoCuentaController> _logger;

        public EstadoCuentaController(IEstadoCuentaService estadoCuentaService, ILogger<EstadoCuentaController> logger)
        {
            _estadoCuentaService = estadoCuentaService ?? throw new ArgumentNullException(nameof(estadoCuentaService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los estados de cuenta con resumen de depósitos
        /// GET /api/estadocuenta
        /// </summary>
        /// <returns>Lista de estados de cuenta</returns>
        [HttpGet]
        public async Task<IActionResult> GetEstadosCuenta()
        {
            try
            {
                var estadosCuenta = await _estadoCuentaService.GetEstadosCuentaAsync();
                return Ok(estadosCuenta);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener estados de cuenta");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener estados de cuenta");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo estado de cuenta
        /// POST /api/estadocuenta
        /// </summary>
        /// <param name="fechaCorte">Fecha de corte del estado de cuenta (obligatorio)</param>
        /// <param name="periodoDesde">Inicio del periodo (obligatorio)</param>
        /// <param name="periodoHasta">Fin del periodo (obligatorio)</param>
        /// <param name="saldoInicial">Saldo inicial del periodo (obligatorio)</param>
        /// <param name="saldoCorte">Saldo al corte (obligatorio)</param>
        /// <param name="totalDepositos">Total de depósitos (obligatorio)</param>
        /// <param name="totalRetiros">Total de retiros (obligatorio)</param>
        /// <param name="comisiones">Comisiones del periodo (opcional)</param>
        /// <param name="nombreArchivo">Nombre del archivo (opcional)</param>
        /// <returns>Resultado de la operación con el ID creado</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEstadoCuenta(
            [FromQuery] DateTime fechaCorte,
            [FromQuery] DateTime periodoDesde,
            [FromQuery] DateTime periodoHasta,
            [FromQuery] decimal saldoInicial,
            [FromQuery] decimal saldoCorte,
            [FromQuery] decimal totalDepositos,
            [FromQuery] decimal totalRetiros,
            [FromQuery] decimal? comisiones = null,
            [FromQuery] string? nombreArchivo = null)
        {
            try
            {
                var query = new EstadoCuentaQueryDto
                {
                    FechaCorte = fechaCorte,
                    PeriodoDesde = periodoDesde,
                    PeriodoHasta = periodoHasta,
                    SaldoInicial = saldoInicial,
                    SaldoCorte = saldoCorte,
                    TotalDepositos = totalDepositos,
                    TotalRetiros = totalRetiros,
                    Comisiones = comisiones,
                    NombreArchivo = nombreArchivo
                };

                var result = await _estadoCuentaService.CreateEstadoCuentaAsync(query);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear estado de cuenta");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear estado de cuenta");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Obtiene los depósitos de un estado de cuenta específico
        /// GET /api/estadocuenta/{id}/depositos
        /// </summary>
        /// <param name="id">ID del estado de cuenta</param>
        /// <returns>Lista de depósitos</returns>
        [HttpGet("{id}/depositos")]
        public async Task<IActionResult> GetDepositos(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id inválido" });
                }

                var depositos = await _estadoCuentaService.GetDepositosAsync(id);
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
        /// Agrega un depósito a un estado de cuenta
        /// POST /api/estadocuenta/{id}/depositos
        /// </summary>
        /// <param name="id">ID del estado de cuenta</param>
        /// <param name="fechaDeposito">Fecha del depósito (obligatorio)</param>
        /// <param name="descripcionDeposito">Descripción del depósito (obligatorio)</param>
        /// <param name="montoDeposito">Monto del depósito (obligatorio)</param>
        /// <param name="tipoDeposito">Tipo de depósito (obligatorio)</param>
        /// <returns>Resultado de la operación con el ID del depósito</returns>
        [HttpPost("{id}/depositos")]
        public async Task<IActionResult> CreateDeposito(
            int id,
            [FromQuery] DateTime fechaDeposito,
            [FromQuery] string descripcionDeposito,
            [FromQuery] decimal montoDeposito,
            [FromQuery] string tipoDeposito)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id inválido" });
                }

                if (string.IsNullOrWhiteSpace(descripcionDeposito))
                {
                    return BadRequest(new { message = "El campo 'descripcionDeposito' es obligatorio." });
                }

                if (string.IsNullOrWhiteSpace(tipoDeposito))
                {
                    return BadRequest(new { message = "El campo 'tipoDeposito' es obligatorio." });
                }

                var query = new EstadoCuentaQueryDto
                {
                    EstadoCuentaID = id,
                    FechaDeposito = fechaDeposito,
                    DescripcionDeposito = descripcionDeposito,
                    MontoDeposito = montoDeposito,
                    TipoDeposito = tipoDeposito
                };

                var result = await _estadoCuentaService.CreateDepositoAsync(query);
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

        /// <summary>
        /// Obtiene el resumen de depósitos por tipo de un estado de cuenta
        /// GET /api/estadocuenta/{id}/resumen
        /// </summary>
        /// <param name="id">ID del estado de cuenta</param>
        /// <returns>Resumen de depósitos agrupados por tipo</returns>
        [HttpGet("{id}/resumen")]
        public async Task<IActionResult> GetResumenDepositos(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id inválido" });
                }

                var resumen = await _estadoCuentaService.GetResumenDepositosAsync(id);
                return Ok(resumen);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener resumen de depósitos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener resumen de depósitos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Verifica si un depósito específico ya existe
        /// GET /api/estadocuenta/{id}/verificar-deposito
        /// </summary>
        /// <param name="id">ID del estado de cuenta</param>
        /// <param name="fechaDeposito">Fecha del depósito</param>
        /// <param name="descripcionDeposito">Descripción del depósito</param>
        /// <param name="montoDeposito">Monto del depósito</param>
        /// <returns>Resultado de la verificación</returns>
        [HttpGet("{id}/verificar-deposito")]
        public async Task<IActionResult> VerificarDeposito(
            int id,
            [FromQuery] DateTime fechaDeposito,
            [FromQuery] string descripcionDeposito,
            [FromQuery] decimal montoDeposito)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id inválido" });
                }

                if (string.IsNullOrWhiteSpace(descripcionDeposito))
                {
                    return BadRequest(new { message = "El campo 'descripcionDeposito' es obligatorio." });
                }

                var query = new EstadoCuentaQueryDto
                {
                    EstadoCuentaID = id,
                    FechaDeposito = fechaDeposito,
                    DescripcionDeposito = descripcionDeposito,
                    MontoDeposito = montoDeposito
                };

                var resultado = await _estadoCuentaService.VerificarDepositoAsync(query);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al verificar depósito");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al verificar depósito");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Busca posibles depósitos duplicados en un estado de cuenta
        /// GET /api/estadocuenta/{id}/duplicados
        /// </summary>
        /// <param name="id">ID del estado de cuenta</param>
        /// <returns>Lista de posibles duplicados</returns>
        [HttpGet("{id}/duplicados")]
        public async Task<IActionResult> BuscarDuplicados(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id inválido" });
                }

                var duplicados = await _estadoCuentaService.BuscarDuplicadosAsync(id);
                return Ok(duplicados);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al buscar duplicados");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al buscar duplicados");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
