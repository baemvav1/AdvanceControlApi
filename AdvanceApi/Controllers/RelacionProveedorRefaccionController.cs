using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RelacionProveedorRefaccionController : ControllerBase
    {
        private readonly IRelacionProveedorRefaccionService _relacionService;
        private readonly ILogger<RelacionProveedorRefaccionController> _logger;

        public RelacionProveedorRefaccionController(IRelacionProveedorRefaccionService relacionService, ILogger<RelacionProveedorRefaccionController> logger)
        {
            _relacionService = relacionService ?? throw new ArgumentNullException(nameof(relacionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene refacciones asociadas a un proveedor según los criterios de búsqueda proporcionados
        /// GET /api/RelacionProveedorRefaccion
        /// </summary>
        /// <param name="idProveedor">Filtro por ID de proveedor (0 para no filtrar)</param>
        /// <param name="idRefaccion">Filtro por ID de refacción (0 para no filtrar)</param>
        /// <param name="nota">Filtro por nota (opcional)</param>
        /// <returns>Lista de refacciones que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetRefacciones(
            [FromQuery] int idProveedor = 0,
            [FromQuery] int idRefaccion = 0,
            [FromQuery] string? nota = null)
        {
            try
            {
                var query = new RelacionProveedorRefaccionQueryDto
                {
                    Operacion = "select",
                    IdProveedor = idProveedor,
                    IdRefaccion = idRefaccion,
                    Nota = nota
                };

                var refacciones = await _relacionService.GetRefaccionesAsync(query);

                return Ok(refacciones);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener refacciones por proveedor");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener refacciones por proveedor");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea una nueva relación proveedor-refacción
        /// POST /api/RelacionProveedorRefaccion
        /// </summary>
        /// <param name="idProveedor">ID del proveedor (obligatorio, mayor que 0)</param>
        /// <param name="idRefaccion">ID de la refacción (obligatorio, mayor que 0)</param>
        /// <param name="precio">Precio de la refacción (obligatorio, mayor que 0)</param>
        /// <param name="nota">Nota asociada a la relación (opcional)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateRelacion(
            [FromQuery] int idProveedor,
            [FromQuery] int idRefaccion,
            [FromQuery] double precio,
            [FromQuery] string? nota = null)
        {
            try
            {
                if (idProveedor <= 0)
                {
                    return BadRequest(new { message = "El campo 'idProveedor' debe ser mayor que 0." });
                }

                if (idRefaccion <= 0)
                {
                    return BadRequest(new { message = "El campo 'idRefaccion' debe ser mayor que 0." });
                }

                if (precio <= 0)
                {
                    return BadRequest(new { message = "El campo 'precio' debe ser mayor que 0." });
                }

                var query = new RelacionProveedorRefaccionQueryDto
                {
                    Operacion = "put",
                    IdProveedor = idProveedor,
                    IdRefaccion = idRefaccion,
                    Precio = precio,
                    Nota = nota
                };

                var result = await _relacionService.CreateRelacionAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear relación proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear relación proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) una relación proveedor-refacción
        /// DELETE /api/RelacionProveedorRefaccion
        /// </summary>
        /// <param name="idRelacionProveedor">ID de la relación proveedor (obligatorio, mayor que 0)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteRelacion(
            [FromQuery] int idRelacionProveedor)
        {
            try
            {
                if (idRelacionProveedor <= 0)
                {
                    return BadRequest(new { message = "El campo 'idRelacionProveedor' debe ser mayor que 0." });
                }

                var result = await _relacionService.DeleteRelacionAsync(idRelacionProveedor);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar relación proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar relación proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza la nota de una relación proveedor-refacción
        /// PUT /api/RelacionProveedorRefaccion/nota
        /// </summary>
        /// <param name="idRelacionProveedor">ID de la relación proveedor (obligatorio, mayor que 0)</param>
        /// <param name="nota">Nueva nota (opcional, puede ser null para limpiar)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("nota")]
        public async Task<IActionResult> UpdateNota(
            [FromQuery] int idRelacionProveedor,
            [FromQuery] string? nota = null)
        {
            try
            {
                if (idRelacionProveedor <= 0)
                {
                    return BadRequest(new { message = "El campo 'idRelacionProveedor' debe ser mayor que 0." });
                }

                var query = new RelacionProveedorRefaccionQueryDto
                {
                    Operacion = "update_nota",
                    IdRelacionProveedor = idRelacionProveedor,
                    Nota = nota
                };

                var result = await _relacionService.UpdateNotaAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar nota de relación proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar nota de relación proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza el precio de una relación proveedor-refacción
        /// PUT /api/RelacionProveedorRefaccion/precio
        /// </summary>
        /// <param name="idRelacionProveedor">ID de la relación proveedor (obligatorio, mayor que 0)</param>
        /// <param name="precio">Nuevo precio (obligatorio, mayor que 0)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("precio")]
        public async Task<IActionResult> UpdatePrecio(
            [FromQuery] int idRelacionProveedor,
            [FromQuery] double precio)
        {
            try
            {
                if (idRelacionProveedor <= 0)
                {
                    return BadRequest(new { message = "El campo 'idRelacionProveedor' debe ser mayor que 0." });
                }

                if (precio <= 0)
                {
                    return BadRequest(new { message = "El campo 'precio' debe ser mayor que 0." });
                }

                var query = new RelacionProveedorRefaccionQueryDto
                {
                    Operacion = "update_precio",
                    IdRelacionProveedor = idRelacionProveedor,
                    Precio = precio
                };

                var result = await _relacionService.UpdatePrecioAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar precio de relación proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar precio de relación proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
