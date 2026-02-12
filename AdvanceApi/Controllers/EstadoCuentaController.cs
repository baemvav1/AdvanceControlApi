using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
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
        /// Consulta estados de cuenta según los criterios de búsqueda proporcionados
        /// GET /api/estadocuenta
        /// </summary>
        /// <param name="idEstadoCuenta">ID del estado de cuenta (opcional)</param>
        /// <param name="numeroCuenta">Número de cuenta (opcional)</param>
        /// <param name="fechaInicio">Fecha inicio del período para filtrar por fechaCorte (opcional)</param>
        /// <param name="fechaFin">Fecha fin del período para filtrar por fechaCorte (opcional)</param>
        /// <returns>Lista de estados de cuenta que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetEstadosCuenta(
            [FromQuery] int? idEstadoCuenta = null,
            [FromQuery] string? numeroCuenta = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            try
            {
                var estadosCuenta = await _estadoCuentaService.ConsultarEstadoCuentaAsync(idEstadoCuenta, numeroCuenta, fechaInicio, fechaFin);

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
        /// <param name="numeroCuenta">Número de cuenta (obligatorio)</param>
        /// <param name="clabe">CLABE interbancaria (obligatorio)</param>
        /// <param name="tipoCuenta">Tipo de cuenta (opcional)</param>
        /// <param name="tipoMoneda">Tipo de moneda (opcional, default: MXN)</param>
        /// <param name="fechaInicio">Fecha de inicio del período (obligatorio)</param>
        /// <param name="fechaFin">Fecha de fin del período (obligatorio)</param>
        /// <param name="fechaCorte">Fecha de corte (obligatorio)</param>
        /// <param name="saldoInicial">Saldo inicial (opcional, default: 0)</param>
        /// <param name="totalCargos">Total de cargos (opcional, default: 0)</param>
        /// <param name="totalAbonos">Total de abonos (opcional, default: 0)</param>
        /// <param name="saldoFinal">Saldo final (opcional, default: 0)</param>
        /// <param name="totalComisiones">Total de comisiones (opcional, default: 0)</param>
        /// <param name="totalISR">Total de ISR (opcional, default: 0)</param>
        /// <param name="totalIVA">Total de IVA (opcional, default: 0)</param>
        /// <returns>Resultado de la operación con el ID del estado de cuenta creado</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEstadoCuenta(
            [FromQuery] string numeroCuenta,
            [FromQuery] string clabe,
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin,
            [FromQuery] DateTime fechaCorte,
            [FromQuery] string? tipoCuenta = null,
            [FromQuery] string tipoMoneda = "MXN",
            [FromQuery] decimal saldoInicial = 0,
            [FromQuery] decimal totalCargos = 0,
            [FromQuery] decimal totalAbonos = 0,
            [FromQuery] decimal saldoFinal = 0,
            [FromQuery] decimal totalComisiones = 0,
            [FromQuery] decimal totalISR = 0,
            [FromQuery] decimal totalIVA = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(numeroCuenta))
                {
                    return BadRequest(new { message = "El campo 'numeroCuenta' es obligatorio." });
                }

                if (string.IsNullOrWhiteSpace(clabe))
                {
                    return BadRequest(new { message = "El campo 'clabe' es obligatorio." });
                }

                var query = new EstadoCuentaQueryDto
                {
                    NumeroCuenta = numeroCuenta,
                    Clabe = clabe,
                    TipoCuenta = tipoCuenta,
                    TipoMoneda = tipoMoneda,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    FechaCorte = fechaCorte,
                    SaldoInicial = saldoInicial,
                    TotalCargos = totalCargos,
                    TotalAbonos = totalAbonos,
                    SaldoFinal = saldoFinal,
                    TotalComisiones = totalComisiones,
                    TotalISR = totalISR,
                    TotalIVA = totalIVA
                };

                var result = await _estadoCuentaService.CrearEstadoCuentaAsync(query);

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
        /// Actualiza un estado de cuenta existente
        /// PUT /api/estadocuenta/{id}
        /// </summary>
        /// <param name="id">ID del estado de cuenta a actualizar</param>
        /// <param name="numeroCuenta">Nuevo número de cuenta (opcional)</param>
        /// <param name="clabe">Nueva CLABE interbancaria (opcional)</param>
        /// <param name="tipoCuenta">Nuevo tipo de cuenta (opcional)</param>
        /// <param name="tipoMoneda">Nuevo tipo de moneda (opcional)</param>
        /// <param name="fechaInicio">Nueva fecha de inicio del período (opcional)</param>
        /// <param name="fechaFin">Nueva fecha de fin del período (opcional)</param>
        /// <param name="fechaCorte">Nueva fecha de corte (opcional)</param>
        /// <param name="saldoInicial">Nuevo saldo inicial (opcional)</param>
        /// <param name="totalCargos">Nuevo total de cargos (opcional)</param>
        /// <param name="totalAbonos">Nuevo total de abonos (opcional)</param>
        /// <param name="saldoFinal">Nuevo saldo final (opcional)</param>
        /// <param name="totalComisiones">Nuevo total de comisiones (opcional)</param>
        /// <param name="totalISR">Nuevo total de ISR (opcional)</param>
        /// <param name="totalIVA">Nuevo total de IVA (opcional)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEstadoCuenta(
            int id,
            [FromQuery] string? numeroCuenta = null,
            [FromQuery] string? clabe = null,
            [FromQuery] string? tipoCuenta = null,
            [FromQuery] string? tipoMoneda = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] DateTime? fechaCorte = null,
            [FromQuery] decimal? saldoInicial = null,
            [FromQuery] decimal? totalCargos = null,
            [FromQuery] decimal? totalAbonos = null,
            [FromQuery] decimal? saldoFinal = null,
            [FromQuery] decimal? totalComisiones = null,
            [FromQuery] decimal? totalISR = null,
            [FromQuery] decimal? totalIVA = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var query = new EstadoCuentaQueryDto
                {
                    IdEstadoCuenta = id,
                    NumeroCuenta = numeroCuenta,
                    Clabe = clabe,
                    TipoCuenta = tipoCuenta,
                    TipoMoneda = tipoMoneda,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    FechaCorte = fechaCorte,
                    SaldoInicial = saldoInicial,
                    TotalCargos = totalCargos,
                    TotalAbonos = totalAbonos,
                    SaldoFinal = saldoFinal,
                    TotalComisiones = totalComisiones,
                    TotalISR = totalISR,
                    TotalIVA = totalIVA
                };

                var result = await _estadoCuentaService.EditarEstadoCuentaAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar estado de cuenta");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar estado de cuenta");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
