using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RelacionOperacionProveedorRefaccionController : ControllerBase
    {
        private readonly IRelacionOperacionProveedorRefaccionService _relacionService;
        private readonly ILogger<RelacionOperacionProveedorRefaccionController> _logger;

        public RelacionOperacionProveedorRefaccionController(IRelacionOperacionProveedorRefaccionService relacionService, ILogger<RelacionOperacionProveedorRefaccionController> logger)
        {
            _relacionService = relacionService ?? throw new ArgumentNullException(nameof(relacionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene relaciones operación-proveedor-refacción asociadas a una operación
        /// GET /api/RelacionOperacionProveedorRefaccion
        /// </summary>
        /// <param name="idOperacion">ID de la operación (obligatorio, mayor que 0)</param>
        /// <returns>Lista de relaciones que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetRelaciones(
            [FromQuery] int idOperacion)
        {
            try
            {
                if (idOperacion <= 0)
                {
                    return BadRequest(new { message = "El campo 'idOperacion' debe ser mayor que 0." });
                }

                var query = new RelacionOperacionProveedorRefaccionQueryDto
                {
                    Operacion = "select",
                    IdOperacion = idOperacion
                };

                var relaciones = await _relacionService.GetRelacionesAsync(query);

                return Ok(relaciones);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener relaciones operación-proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener relaciones operación-proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea una nueva relación operación-proveedor-refacción
        /// POST /api/RelacionOperacionProveedorRefaccion
        /// </summary>
        /// <param name="idOperacion">ID de la operación (obligatorio, mayor que 0)</param>
        /// <param name="idProveedorRefaccion">ID del proveedor refacción (obligatorio, mayor que 0)</param>
        /// <param name="precio">Precio de la refacción (obligatorio, mayor que 0)</param>
        /// <param name="nota">Nota asociada a la relación (opcional)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateRelacion(
            [FromQuery] int idOperacion,
            [FromQuery] int idProveedorRefaccion,
            [FromQuery] double precio,
            [FromQuery] string? nota = null)
        {
            try
            {
                if (idOperacion <= 0)
                {
                    return BadRequest(new { message = "El campo 'idOperacion' debe ser mayor que 0." });
                }

                if (idProveedorRefaccion <= 0)
                {
                    return BadRequest(new { message = "El campo 'idProveedorRefaccion' debe ser mayor que 0." });
                }

                if (precio <= 0)
                {
                    return BadRequest(new { message = "El campo 'precio' debe ser mayor que 0." });
                }

                var query = new RelacionOperacionProveedorRefaccionQueryDto
                {
                    Operacion = "put",
                    IdOperacion = idOperacion,
                    IdProveedorRefaccion = idProveedorRefaccion,
                    Precio = precio,
                    Nota = nota
                };

                var result = await _relacionService.CreateRelacionAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear relación operación-proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear relación operación-proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) una relación operación-proveedor-refacción
        /// DELETE /api/RelacionOperacionProveedorRefaccion
        /// </summary>
        /// <param name="idRelacionOperacionProveedorRefaccion">ID de la relación (obligatorio, mayor que 0)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteRelacion(
            [FromQuery] int idRelacionOperacionProveedorRefaccion)
        {
            try
            {
                if (idRelacionOperacionProveedorRefaccion <= 0)
                {
                    return BadRequest(new { message = "El campo 'idRelacionOperacionProveedorRefaccion' debe ser mayor que 0." });
                }

                var result = await _relacionService.DeleteRelacionAsync(idRelacionOperacionProveedorRefaccion);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar relación operación-proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar relación operación-proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza la nota de una relación operación-proveedor-refacción
        /// PUT /api/RelacionOperacionProveedorRefaccion/nota
        /// </summary>
        /// <param name="idRelacionOperacionProveedorRefaccion">ID de la relación (obligatorio, mayor que 0)</param>
        /// <param name="nota">Nueva nota (opcional, puede ser null para limpiar)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("nota")]
        public async Task<IActionResult> UpdateNota(
            [FromQuery] int idRelacionOperacionProveedorRefaccion,
            [FromQuery] string? nota = null)
        {
            try
            {
                if (idRelacionOperacionProveedorRefaccion <= 0)
                {
                    return BadRequest(new { message = "El campo 'idRelacionOperacionProveedorRefaccion' debe ser mayor que 0." });
                }

                var query = new RelacionOperacionProveedorRefaccionQueryDto
                {
                    Operacion = "update_nota",
                    IdRelacionOperacionProveedorRefaccion = idRelacionOperacionProveedorRefaccion,
                    Nota = nota
                };

                var result = await _relacionService.UpdateNotaAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar nota de relación operación-proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar nota de relación operación-proveedor-refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
