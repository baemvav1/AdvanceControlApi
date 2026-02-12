using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/movimientos")]
    [Authorize]
    public class MovimientoController : ControllerBase
    {
        private readonly IMovimientoService _movimientoService;
        private readonly ILogger<MovimientoController> _logger;

        public MovimientoController(IMovimientoService movimientoService, ILogger<MovimientoController> logger)
        {
            _movimientoService = movimientoService ?? throw new ArgumentNullException(nameof(movimientoService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Consulta movimientos según los criterios de búsqueda proporcionados
        /// GET /api/movimientos
        /// </summary>
        /// <param name="idEstadoCuenta">ID del estado de cuenta (opcional)</param>
        /// <param name="fechaInicio">Fecha inicio del período (opcional)</param>
        /// <param name="fechaFin">Fecha fin del período (opcional)</param>
        /// <param name="tipoOperacion">Tipo de operación (opcional)</param>
        /// <returns>Lista de movimientos que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetMovimientos(
            [FromQuery] int? idEstadoCuenta = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] string? tipoOperacion = null)
        {
            try
            {
                var movimientos = await _movimientoService.ConsultarMovimientosAsync(idEstadoCuenta, fechaInicio, fechaFin, tipoOperacion);

                return Ok(movimientos);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener movimientos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener movimientos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo movimiento
        /// POST /api/movimientos
        /// </summary>
        /// <param name="idEstadoCuenta">ID del estado de cuenta (obligatorio)</param>
        /// <param name="fecha">Fecha del movimiento (obligatorio)</param>
        /// <param name="descripcion">Descripción del movimiento (obligatorio)</param>
        /// <param name="saldo">Saldo después del movimiento (obligatorio)</param>
        /// <param name="referencia">Referencia del movimiento (opcional)</param>
        /// <param name="cargo">Monto del cargo (opcional)</param>
        /// <param name="abono">Monto del abono (opcional)</param>
        /// <param name="tipoOperacion">Tipo de operación (opcional)</param>
        /// <returns>Resultado de la operación con el ID del movimiento creado</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMovimiento(
            [FromQuery] int idEstadoCuenta,
            [FromQuery] DateTime fecha,
            [FromQuery] string descripcion,
            [FromQuery] decimal saldo,
            [FromQuery] string? referencia = null,
            [FromQuery] decimal? cargo = null,
            [FromQuery] decimal? abono = null,
            [FromQuery] string? tipoOperacion = null)
        {
            try
            {
                if (idEstadoCuenta <= 0)
                {
                    return BadRequest(new { message = "El campo 'idEstadoCuenta' es obligatorio y debe ser mayor a 0." });
                }

                if (string.IsNullOrWhiteSpace(descripcion))
                {
                    return BadRequest(new { message = "El campo 'descripcion' es obligatorio." });
                }

                var query = new MovimientoQueryDto
                {
                    IdEstadoCuenta = idEstadoCuenta,
                    Fecha = fecha,
                    Descripcion = descripcion,
                    Referencia = referencia,
                    Cargo = cargo,
                    Abono = abono,
                    Saldo = saldo,
                    TipoOperacion = tipoOperacion
                };

                var result = await _movimientoService.CrearMovimientoAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear movimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear movimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza un movimiento existente
        /// PUT /api/movimientos/{id}
        /// </summary>
        /// <param name="id">ID del movimiento a actualizar</param>
        /// <param name="fecha">Nueva fecha del movimiento (opcional)</param>
        /// <param name="descripcion">Nueva descripción del movimiento (opcional)</param>
        /// <param name="referencia">Nueva referencia del movimiento (opcional)</param>
        /// <param name="cargo">Nuevo monto del cargo (opcional)</param>
        /// <param name="abono">Nuevo monto del abono (opcional)</param>
        /// <param name="saldo">Nuevo saldo (opcional)</param>
        /// <param name="tipoOperacion">Nuevo tipo de operación (opcional)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovimiento(
            int id,
            [FromQuery] DateTime? fecha = null,
            [FromQuery] string? descripcion = null,
            [FromQuery] string? referencia = null,
            [FromQuery] decimal? cargo = null,
            [FromQuery] decimal? abono = null,
            [FromQuery] decimal? saldo = null,
            [FromQuery] string? tipoOperacion = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var query = new MovimientoQueryDto
                {
                    IdMovimiento = id,
                    Fecha = fecha,
                    Descripcion = descripcion,
                    Referencia = referencia,
                    Cargo = cargo,
                    Abono = abono,
                    Saldo = saldo,
                    TipoOperacion = tipoOperacion
                };

                var result = await _movimientoService.EditarMovimientoAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar movimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar movimiento");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
