using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UbicacionController : ControllerBase
    {
        private readonly IUbicacionService _ubicacionService;
        private readonly ILogger<UbicacionController> _logger;

        public UbicacionController(IUbicacionService ubicacionService, ILogger<UbicacionController> logger)
        {
            _ubicacionService = ubicacionService ?? throw new ArgumentNullException(nameof(ubicacionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todas las ubicaciones activas
        /// GET /api/Ubicacion
        /// </summary>
        /// <returns>Lista de ubicaciones activas</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUbicaciones()
        {
            try
            {
                var ubicaciones = await _ubicacionService.GetAllUbicacionesAsync();
                return Ok(ubicaciones);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener ubicaciones");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener ubicaciones");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Obtiene una ubicación por su ID
        /// GET /api/Ubicacion/{id}
        /// </summary>
        /// <param name="id">ID de la ubicación</param>
        /// <returns>La ubicación encontrada</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUbicacionById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID inválido" });
                }

                var ubicacion = await _ubicacionService.GetUbicacionByIdAsync(id);

                if (ubicacion == null)
                {
                    return NotFound(new { message = "Ubicación no encontrada" });
                }

                return Ok(ubicacion);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener ubicación por ID");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener ubicación");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Obtiene una ubicación por su nombre exacto
        /// GET /api/Ubicacion/buscar/{nombre}
        /// </summary>
        /// <param name="nombre">Nombre de la ubicación</param>
        /// <returns>La ubicación encontrada</returns>
        [HttpGet("buscar/{nombre}")]
        public async Task<IActionResult> GetUbicacionByName(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return BadRequest(new { message = "El nombre es requerido" });
                }

                var ubicacion = await _ubicacionService.GetUbicacionByNameAsync(nombre);

                if (ubicacion == null)
                {
                    return NotFound(new { message = "Ubicación no encontrada" });
                }

                return Ok(ubicacion);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener ubicación por nombre");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener ubicación");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea una nueva ubicación para Google Maps
        /// POST /api/Ubicacion
        /// </summary>
        /// <param name="ubicacion">Datos de la ubicación a crear</param>
        /// <returns>Resultado de la operación con el ID de la nueva ubicación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateUbicacion([FromBody] UbicacionDto ubicacion)
        {
            try
            {
                if (ubicacion == null)
                {
                    return BadRequest(new { message = "Los datos de la ubicación son requeridos" });
                }

                if (string.IsNullOrWhiteSpace(ubicacion.Nombre))
                {
                    return BadRequest(new { message = "El nombre es requerido" });
                }

                if (ubicacion.Latitud == null || ubicacion.Longitud == null)
                {
                    return BadRequest(new { message = "Las coordenadas (latitud y longitud) son requeridas" });
                }

                var result = await _ubicacionService.CreateUbicacionAsync(ubicacion);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear ubicación");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear ubicación");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza una ubicación existente
        /// PUT /api/Ubicacion/{id}
        /// </summary>
        /// <param name="id">ID de la ubicación a actualizar</param>
        /// <param name="ubicacion">Datos de la ubicación a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUbicacion(int id, [FromBody] UbicacionDto ubicacion)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID inválido" });
                }

                if (ubicacion == null)
                {
                    return BadRequest(new { message = "Los datos de la ubicación son requeridos" });
                }

                ubicacion.IdUbicacion = id;

                var result = await _ubicacionService.UpdateUbicacionAsync(ubicacion);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar ubicación");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar ubicación");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina físicamente una ubicación
        /// DELETE /api/Ubicacion/{id}
        /// </summary>
        /// <param name="id">ID de la ubicación a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUbicacion(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID inválido" });
                }

                var result = await _ubicacionService.DeleteUbicacionAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar ubicación");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar ubicación");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
