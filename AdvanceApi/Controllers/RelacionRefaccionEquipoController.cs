using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RelacionRefaccionEquipoController : ControllerBase
    {
        private readonly IRelacionRefaccionEquipoService _relacionService;
        private readonly ILogger<RelacionRefaccionEquipoController> _logger;

        public RelacionRefaccionEquipoController(IRelacionRefaccionEquipoService relacionService, ILogger<RelacionRefaccionEquipoController> logger)
        {
            _relacionService = relacionService ?? throw new ArgumentNullException(nameof(relacionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene relaciones refacción-equipo según los criterios de búsqueda proporcionados
        /// GET /api/RelacionRefaccionEquipo
        /// </summary>
        /// <param name="idRefaccion">Filtro por ID de refacción (0 para no filtrar)</param>
        /// <param name="idEquipo">Filtro por ID de equipo (0 para no filtrar)</param>
        /// <returns>Lista de relaciones que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetRelaciones(
            [FromQuery] int idRefaccion = 0,
            [FromQuery] int? idEquipo = null)
        {
            try
            {
                var query = new RelacionRefaccionEquipoQueryDto
                {
                    Operacion = "select",
                    IdRefaccion = idRefaccion,
                    IdEquipo = idEquipo
                };

                var relaciones = await _relacionService.GetRelacionesAsync(query);

                return Ok(relaciones);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener relaciones refacción-equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener relaciones refacción-equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea una nueva relación refacción-equipo
        /// POST /api/RelacionRefaccionEquipo
        /// </summary>
        /// <param name="idRefaccion">ID de la refacción (obligatorio, mayor que 0)</param>
        /// <param name="idEquipo">ID del equipo (obligatorio, mayor que 0)</param>
        /// <param name="nota">Nota asociada a la relación (opcional)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateRelacion(
            [FromQuery] int idRefaccion,
            [FromQuery] int idEquipo,
            [FromQuery] string? nota = null)
        {
            try
            {
                if (idRefaccion <= 0)
                {
                    return BadRequest(new { message = "El campo 'idRefaccion' debe ser mayor que 0." });
                }

                if (idEquipo <= 0)
                {
                    return BadRequest(new { message = "El campo 'idEquipo' debe ser mayor que 0." });
                }

                var query = new RelacionRefaccionEquipoQueryDto
                {
                    Operacion = "put",
                    IdRefaccion = idRefaccion,
                    IdEquipo = idEquipo,
                    Nota = nota
                };

                var result = await _relacionService.CreateRelacionAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear relación refacción-equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear relación refacción-equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) una relación refacción-equipo
        /// DELETE /api/RelacionRefaccionEquipo
        /// </summary>
        /// <param name="idRelacionRefaccion">ID de la relación a eliminar (obligatorio, mayor que 0)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteRelacion(
            [FromQuery] int idRelacionRefaccion)
        {
            try
            {
                if (idRelacionRefaccion <= 0)
                {
                    return BadRequest(new { message = "El campo 'idRelacionRefaccion' debe ser mayor que 0." });
                }

                var result = await _relacionService.DeleteRelacionAsync(idRelacionRefaccion);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar relación refacción-equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar relación refacción-equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza la nota de una relación refacción-equipo
        /// PUT /api/RelacionRefaccionEquipo/nota
        /// </summary>
        /// <param name="idRelacionRefaccion">ID de la relación (obligatorio, mayor que 0)</param>
        /// <param name="nota">Nueva nota (opcional, puede ser null para limpiar)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("nota")]
        public async Task<IActionResult> UpdateNota(
            [FromQuery] int idRelacionRefaccion,
            [FromQuery] string? nota = null)
        {
            try
            {
                if (idRelacionRefaccion <= 0)
                {
                    return BadRequest(new { message = "El campo 'idRelacionRefaccion' debe ser mayor que 0." });
                }

                var query = new RelacionRefaccionEquipoQueryDto
                {
                    Operacion = "update_nota",
                    IdRelacionRefaccion = idRelacionRefaccion,
                    Nota = nota
                };

                var result = await _relacionService.UpdateNotaAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar nota de relación refacción-equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar nota de relación refacción-equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
