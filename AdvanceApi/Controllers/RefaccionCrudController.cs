using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/refaccion_crud")]
    [Authorize]
    public class RefaccionCrudController : ControllerBase
    {
        private readonly IRefaccionService _refaccionService;
        private readonly ILogger<RefaccionCrudController> _logger;

        public RefaccionCrudController(IRefaccionService refaccionService, ILogger<RefaccionCrudController> logger)
        {
            _refaccionService = refaccionService ?? throw new ArgumentNullException(nameof(refaccionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene refacciones según los criterios de búsqueda proporcionados
        /// GET /api/refaccion_crud
        /// </summary>
        /// <param name="marca">Búsqueda parcial por marca</param>
        /// <param name="serie">Búsqueda parcial por serie</param>
        /// <param name="descripcion">Búsqueda parcial en descripción</param>
        /// <returns>Lista de refacciones que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetRefacciones(
            [FromQuery] string? marca = null,
            [FromQuery] string? serie = null,
            [FromQuery] string? descripcion = null)
        {
            try
            {
                var query = new RefaccionQueryDto
                {
                    Operacion = "select",
                    IdRefaccion = 0,
                    Marca = marca,
                    Serie = serie,
                    Descripcion = descripcion,
                    Estatus = true
                };

                var refacciones = await _refaccionService.ExecuteRefaccionOperationAsync(query);

                return Ok(refacciones);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener refacciones");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener refacciones");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) una refacción por su ID
        /// DELETE /api/refaccion_crud/{id}
        /// </summary>
        /// <param name="id">ID de la refacción a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRefaccion(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var result = await _refaccionService.DeleteRefaccionAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza una refacción existente
        /// PUT /api/refaccion_crud/{id}
        /// </summary>
        /// <param name="id">ID de la refacción a actualizar</param>
        /// <param name="marca">Nueva marca de la refacción</param>
        /// <param name="serie">Nueva serie de la refacción</param>
        /// <param name="costo">Nuevo costo de la refacción</param>
        /// <param name="descripcion">Nueva descripción</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRefaccion(
            int id,
            [FromQuery] string? marca = null,
            [FromQuery] string? serie = null,
            [FromQuery] double? costo = null,
            [FromQuery] string? descripcion = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var query = new RefaccionQueryDto
                {
                    Operacion = "update",
                    IdRefaccion = id,
                    Marca = marca,
                    Serie = serie,
                    Costo = costo,
                    Descripcion = descripcion,
                    Estatus = true
                };

                var result = await _refaccionService.UpdateRefaccionAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea una nueva refacción
        /// POST /api/refaccion_crud
        /// </summary>
        /// <param name="marca">Marca de la refacción (obligatorio)</param>
        /// <param name="serie">Serie de la refacción (obligatorio)</param>
        /// <param name="costo">Costo de la refacción (obligatorio)</param>
        /// <param name="descripcion">Descripción de la refacción (obligatorio)</param>
        /// <param name="estatus">Estatus de la refacción (opcional, default true)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateRefaccion(
            [FromQuery] string marca,
            [FromQuery] string serie,
            [FromQuery] double costo,
            [FromQuery] string descripcion,
            [FromQuery] bool estatus = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(marca))
                {
                    return BadRequest(new { message = "El campo 'marca' es obligatorio." });
                }

                if (string.IsNullOrWhiteSpace(serie))
                {
                    return BadRequest(new { message = "El campo 'serie' es obligatorio." });
                }

                if (string.IsNullOrWhiteSpace(descripcion))
                {
                    return BadRequest(new { message = "El campo 'descripcion' es obligatorio." });
                }

                var query = new RefaccionQueryDto
                {
                    Marca = marca,
                    Serie = serie,
                    Costo = costo,
                    Descripcion = descripcion,
                    Estatus = estatus
                };

                var result = await _refaccionService.CreateRefaccionAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Verifica si una refacción tiene proveedores relacionados
        /// GET /api/refaccion_crud/{id}/check-proveedor
        /// </summary>
        /// <param name="id">ID de la refacción a verificar</param>
        /// <returns>Resultado indicando si existe relación con proveedores</returns>
        [HttpGet("{id}/check-proveedor")]
        public async Task<IActionResult> CheckProveedorExists(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var result = await _refaccionService.CheckProveedorExistsAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al verificar proveedores de refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al verificar proveedores de refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
