using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/entidad")]
    [Authorize]
    public class EntidadController : ControllerBase
    {
        private readonly IEntidadService _entidadService;
        private readonly ILogger<EntidadController> _logger;

        public EntidadController(IEntidadService entidadService, ILogger<EntidadController> logger)
        {
            _entidadService = entidadService ?? throw new ArgumentNullException(nameof(entidadService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene entidades según los criterios de búsqueda proporcionados
        /// GET /api/entidad
        /// </summary>
        /// <param name="idEntidad">ID de la entidad</param>
        /// <param name="nombreComercial">Búsqueda parcial por nombre comercial</param>
        /// <param name="razonSocial">Búsqueda parcial por razón social</param>
        /// <param name="rfc">Búsqueda parcial por RFC</param>
        /// <param name="cp">Búsqueda parcial por código postal</param>
        /// <param name="estado">Búsqueda parcial por estado</param>
        /// <param name="ciudad">Búsqueda parcial por ciudad</param>
        /// <param name="pais">Búsqueda parcial por país</param>
        /// <param name="calle">Búsqueda parcial por calle</param>
        /// <param name="nomExt">Búsqueda parcial por número exterior</param>
        /// <param name="numInt">Búsqueda parcial por número interior</param>
        /// <param name="colonia">Búsqueda parcial por colonia</param>
        /// <param name="apoderado">Búsqueda parcial por apoderado</param>
        /// <returns>Lista de entidades que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetEntidades(
            [FromQuery] int? idEntidad = null,
            [FromQuery] string? nombreComercial = null,
            [FromQuery] string? razonSocial = null,
            [FromQuery] string? rfc = null,
            [FromQuery] string? cp = null,
            [FromQuery] string? estado = null,
            [FromQuery] string? ciudad = null,
            [FromQuery] string? pais = null,
            [FromQuery] string? calle = null,
            [FromQuery] string? nomExt = null,
            [FromQuery] string? numInt = null,
            [FromQuery] string? colonia = null,
            [FromQuery] string? apoderado = null)
        {
            try
            {
                var query = new EntidadQueryDto
                {
                    Operacion = "select",
                    IdEntidad = idEntidad,
                    NombreComercial = nombreComercial,
                    RazonSocial = razonSocial,
                    RFC = rfc,
                    CP = cp,
                    Estado = estado,
                    Ciudad = ciudad,
                    Pais = pais,
                    Calle = calle,
                    NomExt = nomExt,
                    NumInt = numInt,
                    Colonia = colonia,
                    Apoderado = apoderado,
                    Estatus = true
                };

                var entidades = await _entidadService.ExecuteEntidadOperationAsync(query);

                return Ok(entidades);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener entidades");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener entidades");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) una entidad por su ID
        /// DELETE /api/entidad/{id}
        /// </summary>
        /// <param name="id">ID de la entidad a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntidad(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var result = await _entidadService.DeleteEntidadAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar entidad");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar entidad");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza una entidad existente
        /// PUT /api/entidad/{id}
        /// </summary>
        /// <param name="id">ID de la entidad a actualizar</param>
        /// <param name="nombreComercial">Nuevo nombre comercial</param>
        /// <param name="razonSocial">Nueva razón social</param>
        /// <param name="rfc">Nuevo RFC</param>
        /// <param name="cp">Nuevo código postal</param>
        /// <param name="estado">Nuevo estado</param>
        /// <param name="ciudad">Nueva ciudad</param>
        /// <param name="pais">Nuevo país</param>
        /// <param name="calle">Nueva calle</param>
        /// <param name="nomExt">Nuevo número exterior</param>
        /// <param name="numInt">Nuevo número interior</param>
        /// <param name="colonia">Nueva colonia</param>
        /// <param name="apoderado">Nuevo apoderado</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntidad(
            int id,
            [FromQuery] string? nombreComercial = null,
            [FromQuery] string? razonSocial = null,
            [FromQuery] string? rfc = null,
            [FromQuery] string? cp = null,
            [FromQuery] string? estado = null,
            [FromQuery] string? ciudad = null,
            [FromQuery] string? pais = null,
            [FromQuery] string? calle = null,
            [FromQuery] string? nomExt = null,
            [FromQuery] string? numInt = null,
            [FromQuery] string? colonia = null,
            [FromQuery] string? apoderado = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var query = new EntidadQueryDto
                {
                    Operacion = "update",
                    IdEntidad = id,
                    NombreComercial = nombreComercial,
                    RazonSocial = razonSocial,
                    RFC = rfc,
                    CP = cp,
                    Estado = estado,
                    Ciudad = ciudad,
                    Pais = pais,
                    Calle = calle,
                    NomExt = nomExt,
                    NumInt = numInt,
                    Colonia = colonia,
                    Apoderado = apoderado,
                    Estatus = true
                };

                var result = await _entidadService.UpdateEntidadAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar entidad");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar entidad");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea una nueva entidad
        /// POST /api/entidad
        /// </summary>
        /// <param name="nombreComercial">Nombre comercial de la entidad (obligatorio)</param>
        /// <param name="razonSocial">Razón social de la entidad (obligatorio)</param>
        /// <param name="rfc">RFC de la entidad (opcional)</param>
        /// <param name="cp">Código postal de la entidad (opcional)</param>
        /// <param name="estado">Estado de la entidad (opcional)</param>
        /// <param name="ciudad">Ciudad de la entidad (opcional)</param>
        /// <param name="pais">País de la entidad (opcional)</param>
        /// <param name="calle">Calle de la entidad (opcional)</param>
        /// <param name="nomExt">Número exterior de la entidad (opcional)</param>
        /// <param name="numInt">Número interior de la entidad (opcional)</param>
        /// <param name="colonia">Colonia de la entidad (opcional)</param>
        /// <param name="apoderado">Apoderado de la entidad (opcional)</param>
        /// <param name="estatus">Estatus de la entidad (opcional, default true)</param>
        /// <returns>Resultado de la operación con la entidad creada</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEntidad(
            [FromQuery] string nombreComercial,
            [FromQuery] string razonSocial,
            [FromQuery] string? rfc = null,
            [FromQuery] string? cp = null,
            [FromQuery] string? estado = null,
            [FromQuery] string? ciudad = null,
            [FromQuery] string? pais = null,
            [FromQuery] string? calle = null,
            [FromQuery] string? nomExt = null,
            [FromQuery] string? numInt = null,
            [FromQuery] string? colonia = null,
            [FromQuery] string? apoderado = null,
            [FromQuery] bool estatus = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombreComercial))
                {
                    return BadRequest(new { message = "El campo 'nombreComercial' es obligatorio." });
                }

                if (string.IsNullOrWhiteSpace(razonSocial))
                {
                    return BadRequest(new { message = "El campo 'razonSocial' es obligatorio." });
                }

                var query = new EntidadQueryDto
                {
                    NombreComercial = nombreComercial,
                    RazonSocial = razonSocial,
                    RFC = rfc,
                    CP = cp,
                    Estado = estado,
                    Ciudad = ciudad,
                    Pais = pais,
                    Calle = calle,
                    NomExt = nomExt,
                    NumInt = numInt,
                    Colonia = colonia,
                    Apoderado = apoderado,
                    Estatus = estatus
                };

                var result = await _entidadService.CreateEntidadAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear entidad");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear entidad");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
