using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AreasController : ControllerBase
    {
        private readonly IAreaService _areaService;
        private readonly ILogger<AreasController> _logger;

        public AreasController(IAreaService areaService, ILogger<AreasController> logger)
        {
            _areaService = areaService ?? throw new ArgumentNullException(nameof(areaService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene áreas según los criterios de búsqueda proporcionados
        /// GET /api/Areas
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAreas(
            [FromQuery] int idArea = 0,
            [FromQuery] string? nombre = null,
            [FromQuery] bool? activo = null,
            [FromQuery] string? tipoGeometria = null)
        {
            try
            {
                var query = new AreaEditDto
                {
                    IdArea = idArea,
                    Nombre = nombre,
                    Activo = activo,
                    TipoGeometria = tipoGeometria
                };

                var areas = await _areaService.GetAreasAsync(query);

                return Ok(areas);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener áreas");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener áreas");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Obtiene áreas en formato optimizado para Google Maps JavaScript API
        /// GET /api/Areas/googlemaps
        /// </summary>
        [HttpGet("googlemaps")]
        public async Task<IActionResult> GetAreasGoogleMaps(
            [FromQuery] int idArea = 0,
            [FromQuery] bool? activo = null)
        {
            try
            {
                var result = await _areaService.GetAreasGoogleMapsAsync(idArea, activo);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener áreas Google Maps");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener áreas Google Maps");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Obtiene áreas en formato GeoJSON estándar
        /// GET /api/Areas/geojson
        /// </summary>
        [HttpGet("geojson")]
        public async Task<IActionResult> GetAreasGeoJson(
            [FromQuery] int idArea = 0,
            [FromQuery] bool? activo = null)
        {
            try
            {
                var result = await _areaService.GetAreasGeoJsonAsync(idArea, activo);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener áreas GeoJSON");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener áreas GeoJSON");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea una nueva área
        /// POST /api/Areas
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateArea(
            [FromQuery] string nombre,
            [FromQuery] string? descripcion = null,
            [FromQuery] string? colorMapa = null,
            [FromQuery] decimal? opacidad = null,
            [FromQuery] string? colorBorde = null,
            [FromQuery] int? anchoBorde = null,
            [FromQuery] bool? activo = null,
            [FromQuery] string? tipoGeometria = null,
            [FromQuery] decimal? centroLatitud = null,
            [FromQuery] decimal? centroLongitud = null,
            [FromQuery] decimal? radio = null,
            [FromQuery] bool? etiquetaMostrar = null,
            [FromQuery] string? etiquetaTexto = null,
            [FromQuery] int? nivelZoom = null,
            [FromQuery] string? metadataJSON = null,
            [FromQuery] string? usuarioCreacion = null,
            [FromQuery] string? coordenadas = null,
            [FromQuery] bool? autoCalcularCentro = null,
            [FromQuery] bool? validarPoligonoLargo = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return BadRequest(new { message = "El campo 'nombre' es obligatorio." });
                }

                var query = new AreaEditDto
                {
                    Nombre = nombre,
                    Descripcion = descripcion,
                    ColorMapa = colorMapa,
                    Opacidad = opacidad,
                    ColorBorde = colorBorde,
                    AnchoBorde = anchoBorde,
                    Activo = activo,
                    TipoGeometria = tipoGeometria,
                    CentroLatitud = centroLatitud,
                    CentroLongitud = centroLongitud,
                    Radio = radio,
                    EtiquetaMostrar = etiquetaMostrar,
                    EtiquetaTexto = etiquetaTexto,
                    NivelZoom = nivelZoom,
                    MetadataJSON = metadataJSON,
                    UsuarioCreacion = usuarioCreacion,
                    Coordenadas = coordenadas,
                    AutoCalcularCentro = autoCalcularCentro,
                    ValidarPoligonoLargo = validarPoligonoLargo
                };

                var result = await _areaService.CreateAreaAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear área");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear área");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza un área existente
        /// PUT /api/Areas/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArea(
            int id,
            [FromQuery] string? nombre = null,
            [FromQuery] string? descripcion = null,
            [FromQuery] string? colorMapa = null,
            [FromQuery] decimal? opacidad = null,
            [FromQuery] string? colorBorde = null,
            [FromQuery] int? anchoBorde = null,
            [FromQuery] bool? activo = null,
            [FromQuery] string? tipoGeometria = null,
            [FromQuery] decimal? centroLatitud = null,
            [FromQuery] decimal? centroLongitud = null,
            [FromQuery] decimal? radio = null,
            [FromQuery] bool? etiquetaMostrar = null,
            [FromQuery] string? etiquetaTexto = null,
            [FromQuery] int? nivelZoom = null,
            [FromQuery] string? metadataJSON = null,
            [FromQuery] string? usuarioModificacion = null,
            [FromQuery] string? coordenadas = null,
            [FromQuery] bool? autoCalcularCentro = null,
            [FromQuery] bool? validarPoligonoLargo = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Inválido" });
                }

                var query = new AreaEditDto
                {
                    IdArea = id,
                    Nombre = nombre,
                    Descripcion = descripcion,
                    ColorMapa = colorMapa,
                    Opacidad = opacidad,
                    ColorBorde = colorBorde,
                    AnchoBorde = anchoBorde,
                    Activo = activo,
                    TipoGeometria = tipoGeometria,
                    CentroLatitud = centroLatitud,
                    CentroLongitud = centroLongitud,
                    Radio = radio,
                    EtiquetaMostrar = etiquetaMostrar,
                    EtiquetaTexto = etiquetaTexto,
                    NivelZoom = nivelZoom,
                    MetadataJSON = metadataJSON,
                    UsuarioModificacion = usuarioModificacion,
                    Coordenadas = coordenadas,
                    AutoCalcularCentro = autoCalcularCentro,
                    ValidarPoligonoLargo = validarPoligonoLargo
                };

                var result = await _areaService.UpdateAreaAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar área");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar área");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) un área por su ID
        /// DELETE /api/Areas/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Inválido" });
                }

                var result = await _areaService.DeleteAreaAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar área");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar área");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina físicamente un área por su ID
        /// DELETE /api/Areas/{id}/physical
        /// </summary>
        [HttpDelete("{id}/physical")]
        public async Task<IActionResult> DeleteAreaPhysical(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Inválido" });
                }

                var result = await _areaService.DeleteAreaPhysicalAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar área físicamente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar área físicamente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Valida si un punto está dentro de un polígono
        /// GET /api/Areas/validate-point
        /// </summary>
        [HttpGet("validate-point")]
        public async Task<IActionResult> ValidatePointInPolygon(
            [FromQuery] int idArea = 0,
            [FromQuery] decimal latitud = 0,
            [FromQuery] decimal longitud = 0)
        {
            try
            {
                if (latitud < -90 || latitud > 90 || longitud < -180 || longitud > 180)
                {
                    return BadRequest(new { message = "Las coordenadas están fuera del rango válido (-90/90 para latitud, -180/180 para longitud)" });
                }

                var result = await _areaService.ValidatePointInPolygonAsync(idArea, latitud, longitud);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al validar punto en polígono");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al validar punto en polígono");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
