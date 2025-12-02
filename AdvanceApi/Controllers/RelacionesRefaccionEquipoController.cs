using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RelacionesRefaccionEquipoController : ControllerBase
    {
        private readonly IRelacionRefaccionEquipoService _relacionService;
        private readonly ILogger<RelacionesRefaccionEquipoController> _logger;

        public RelacionesRefaccionEquipoController(IRelacionRefaccionEquipoService relacionService, ILogger<RelacionesRefaccionEquipoController> logger)
        {
            _relacionService = relacionService ?? throw new ArgumentNullException(nameof(relacionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene las refacciones asociadas a un equipo
        /// GET /api/RelacionesRefaccionEquipo/refacciones?idEquipo={idEquipo}
        /// </summary>
        /// <param name="idEquipo">ID del equipo (obligatorio, mayor que 0)</param>
        /// <returns>Lista de refacciones asociadas al equipo</returns>
        [HttpGet("refacciones")]
        public async Task<IActionResult> GetRefaccionesByEquipo([FromQuery] int idEquipo)
        {
            try
            {
                if (idEquipo <= 0)
                {
                    return BadRequest(new { message = "El campo 'idEquipo' debe ser mayor que 0." });
                }

                var refacciones = await _relacionService.GetRefaccionesByEquipoAsync(idEquipo);

                return Ok(refacciones);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener refacciones por equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener refacciones por equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Obtiene los equipos asociados a una refacción
        /// GET /api/RelacionesRefaccionEquipo/equipos?idRefaccion={idRefaccion}
        /// </summary>
        /// <param name="idRefaccion">ID de la refacción (obligatorio, mayor que 0)</param>
        /// <returns>Lista de equipos asociados a la refacción</returns>
        [HttpGet("equipos")]
        public async Task<IActionResult> GetEquiposByRefaccion([FromQuery] int idRefaccion)
        {
            try
            {
                if (idRefaccion <= 0)
                {
                    return BadRequest(new { message = "El campo 'idRefaccion' debe ser mayor que 0." });
                }

                var equipos = await _relacionService.GetEquiposByRefaccionAsync(idRefaccion);

                return Ok(equipos);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener equipos por refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener equipos por refacción");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea una nueva relación refacción-equipo
        /// POST /api/RelacionesRefaccionEquipo
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
        /// DELETE /api/RelacionesRefaccionEquipo
        /// </summary>
        /// <param name="idRelacionRefaccion">ID de la relación refacción (obligatorio, mayor que 0)</param>
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
        /// PUT /api/RelacionesRefaccionEquipo/nota
        /// </summary>
        /// <param name="idRelacionRefaccion">ID de la relación refacción (obligatorio, mayor que 0)</param>
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
