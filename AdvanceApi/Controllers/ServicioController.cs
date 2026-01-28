using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/servicio")]
    [Authorize]
    public class ServicioController : ControllerBase
    {
        private readonly IServicioService _servicioService;
        private readonly ILogger<ServicioController> _logger;

        public ServicioController(IServicioService servicioService, ILogger<ServicioController> logger)
        {
            _servicioService = servicioService ?? throw new ArgumentNullException(nameof(servicioService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene servicios según los criterios de búsqueda proporcionados
        /// GET /api/servicio
        /// </summary>
        /// <param name="concepto">Búsqueda parcial por concepto</param>
        /// <param name="descripcion">Búsqueda parcial en descripción</param>
        /// <param name="costo">Búsqueda por costo exacto</param>
        /// <returns>Lista de servicios que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetServicios(
            [FromQuery] string? concepto = null,
            [FromQuery] string? descripcion = null,
            [FromQuery] double? costo = null)
        {
            try
            {
                var query = new ServicioQueryDto
                {
                    Operacion = "select",
                    IdServicio = 0,
                    Concepto = concepto,
                    Descripcion = descripcion,
                    Costo = costo,
                    Estatus = true
                };

                var servicios = await _servicioService.ExecuteServicioOperationAsync(query);

                return Ok(servicios);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener servicios");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener servicios");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) un servicio por su ID
        /// DELETE /api/servicio/{id}
        /// </summary>
        /// <param name="id">ID del servicio a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServicio(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var result = await _servicioService.DeleteServicioAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar servicio");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar servicio");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza un servicio existente
        /// PUT /api/servicio/{id}
        /// </summary>
        /// <param name="id">ID del servicio a actualizar</param>
        /// <param name="concepto">Nuevo concepto del servicio</param>
        /// <param name="descripcion">Nueva descripción</param>
        /// <param name="costo">Nuevo costo del servicio</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServicio(
            int id,
            [FromQuery] string? concepto = null,
            [FromQuery] string? descripcion = null,
            [FromQuery] double? costo = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var query = new ServicioQueryDto
                {
                    Operacion = "update",
                    IdServicio = id,
                    Concepto = concepto,
                    Descripcion = descripcion,
                    Costo = costo,
                    Estatus = true
                };

                var result = await _servicioService.UpdateServicioAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar servicio");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar servicio");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo servicio
        /// POST /api/servicio
        /// </summary>
        /// <param name="concepto">Concepto del servicio (obligatorio)</param>
        /// <param name="descripcion">Descripción del servicio (obligatorio)</param>
        /// <param name="costo">Costo del servicio (obligatorio)</param>
        /// <param name="estatus">Estatus del servicio (opcional, default true)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateServicio(
            [FromQuery] string concepto,
            [FromQuery] string descripcion,
            [FromQuery] double costo,
            [FromQuery] bool estatus = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(concepto))
                {
                    return BadRequest(new { message = "El campo 'concepto' es obligatorio." });
                }

                if (string.IsNullOrWhiteSpace(descripcion))
                {
                    return BadRequest(new { message = "El campo 'descripcion' es obligatorio." });
                }

                var query = new ServicioQueryDto
                {
                    Concepto = concepto,
                    Descripcion = descripcion,
                    Costo = costo,
                    Estatus = estatus
                };

                var result = await _servicioService.CreateServicioAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear servicio");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear servicio");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
