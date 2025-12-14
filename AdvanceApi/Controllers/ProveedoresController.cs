using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/proveedores")]
    [Authorize]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;
        private readonly ILogger<ProveedoresController> _logger;

        public ProveedoresController(IProveedorService proveedorService, ILogger<ProveedoresController> logger)
        {
            _proveedorService = proveedorService ?? throw new ArgumentNullException(nameof(proveedorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene proveedores según los criterios de búsqueda proporcionados
        /// GET /api/proveedores
        /// </summary>
        /// <param name="razonSocial">Búsqueda parcial por razón social</param>
        /// <param name="nombreComercial">Búsqueda parcial por nombre comercial</param>
        /// <param name="nota">Búsqueda parcial en nota</param>
        /// <returns>Lista de proveedores que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetProveedores(
            [FromQuery] string? razonSocial = null,
            [FromQuery] string? nombreComercial = null,
            [FromQuery] string? nota = null)
        {
            try
            {
                var query = new ProveedorQueryDto
                {
                    Operacion = "select",
                    IdProveedor = 0,
                    RazonSocial = razonSocial,
                    NombreComercial = nombreComercial,
                    Nota = nota,
                    Estatus = true
                };

                var proveedores = await _proveedorService.ExecuteProveedorOperationAsync(query);

                return Ok(proveedores);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener proveedores");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener proveedores");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) un proveedor por su ID
        /// DELETE /api/proveedores/{id}
        /// </summary>
        /// <param name="id">ID del proveedor a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var result = await _proveedorService.DeleteProveedorAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar proveedor");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar proveedor");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza un proveedor existente
        /// PUT /api/proveedores/{id}
        /// </summary>
        /// <param name="id">ID del proveedor a actualizar</param>
        /// <param name="rfc">Nuevo RFC del proveedor</param>
        /// <param name="razonSocial">Nueva razón social</param>
        /// <param name="nombreComercial">Nuevo nombre comercial</param>
        /// <param name="nota">Nueva nota</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProveedor(
            int id,
            [FromQuery] string? rfc = null,
            [FromQuery] string? razonSocial = null,
            [FromQuery] string? nombreComercial = null,
            [FromQuery] string? nota = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var query = new ProveedorQueryDto
                {
                    Operacion = "update",
                    IdProveedor = id,
                    Rfc = rfc,
                    RazonSocial = razonSocial,
                    NombreComercial = nombreComercial,
                    Nota = nota,
                    Estatus = true
                };

                var result = await _proveedorService.UpdateProveedorAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar proveedor");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar proveedor");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo proveedor
        /// POST /api/proveedores
        /// </summary>
        /// <param name="rfc">RFC del proveedor (obligatorio)</param>
        /// <param name="razonSocial">Razón social del proveedor (opcional)</param>
        /// <param name="nombreComercial">Nombre comercial (opcional)</param>
        /// <param name="nota">Nota o comentario (opcional)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProveedor(
            [FromQuery] string rfc,
            [FromQuery] string? razonSocial = null,
            [FromQuery] string? nombreComercial = null,
            [FromQuery] string? nota = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rfc))
                {
                    return BadRequest(new { message = "El campo 'rfc' es obligatorio." });
                }

                var query = new ProveedorQueryDto
                {
                    Rfc = rfc,
                    RazonSocial = razonSocial,
                    NombreComercial = nombreComercial,
                    Nota = nota,
                    Estatus = true
                };

                var result = await _proveedorService.CreateProveedorAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear proveedor");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear proveedor");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
