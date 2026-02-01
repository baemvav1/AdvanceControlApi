using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    /// <summary>
    /// Controlador para búsqueda de lugares usando Google Places API
    /// Actúa como proxy para evitar exponer la API key en el cliente
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GooglePlacesController : ControllerBase
    {
        private readonly IGooglePlacesService _placesService;
        private readonly ILogger<GooglePlacesController> _logger;

        public GooglePlacesController(
            IGooglePlacesService placesService,
            ILogger<GooglePlacesController> logger)
        {
            _placesService = placesService ?? throw new ArgumentNullException(nameof(placesService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Valida el formato y rango de coordenadas de ubicación
        /// </summary>
        private IActionResult? ValidateLocation(string? location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return null;

            var parts = location.Split(',');
            if (parts.Length != 2 ||
                !double.TryParse(parts[0].Trim(), out var lat) ||
                !double.TryParse(parts[1].Trim(), out var lng))
            {
                return BadRequest(new { message = "El parámetro 'location' debe estar en formato 'lat,lng'" });
            }

            if (lat < -90 || lat > 90 || lng < -180 || lng > 180)
            {
                return BadRequest(new { message = "Coordenadas inválidas. Lat: -90 a 90, Lng: -180 a 180" });
            }

            return null;
        }

        /// <summary>
        /// Valida el rango del radio de búsqueda
        /// </summary>
        private IActionResult? ValidateRadius(int? radius)
        {
            if (radius.HasValue && (radius.Value <= 0 || radius.Value > 50000))
            {
                return BadRequest(new { message = "El parámetro 'radius' debe estar entre 1 y 50000 metros" });
            }

            return null;
        }

        /// <summary>
        /// Busca lugares por texto
        /// GET /api/GooglePlaces/search?query=restaurant&location=19.4326,-99.1332&radius=5000
        /// </summary>
        /// <param name="query">Texto de búsqueda (requerido)</param>
        /// <param name="location">Ubicación en formato "lat,lng" (opcional)</param>
        /// <param name="radius">Radio de búsqueda en metros (opcional)</param>
        /// <returns>Resultados de la búsqueda en formato JSON de Google Places API</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchPlaces(
            [FromQuery] string query,
            [FromQuery] string? location = null,
            [FromQuery] int? radius = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { message = "El parámetro 'query' es requerido" });
                }

                // Validar formato de location y radius
                var locationError = ValidateLocation(location);
                if (locationError != null) return locationError;

                var radiusError = ValidateRadius(radius);
                if (radiusError != null) return radiusError;

                _logger.LogInformation("Búsqueda de lugares solicitada: {Query}", query);

                var result = await _placesService.SearchPlacesAsync(query, location, radius);

                // Retornar el JSON directamente desde Google Places API
                return Content(result, "application/json");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al buscar lugares");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al buscar lugares en Google Places API" });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al buscar lugares");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor" });
#endif
            }
        }

        /// <summary>
        /// Obtiene detalles de un lugar específico
        /// GET /api/GooglePlaces/details?placeId=ChIJN1t_tDeuEmsRUsoyG83frY4
        /// </summary>
        /// <param name="placeId">ID del lugar en Google Places (requerido)</param>
        /// <returns>Detalles del lugar en formato JSON de Google Places API</returns>
        [HttpGet("details")]
        public async Task<IActionResult> GetPlaceDetails([FromQuery] string placeId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(placeId))
                {
                    return BadRequest(new { message = "El parámetro 'placeId' es requerido" });
                }

                _logger.LogInformation("Detalles de lugar solicitados: {PlaceId}", placeId);

                var result = await _placesService.GetPlaceDetailsAsync(placeId);

                return Content(result, "application/json");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener detalles del lugar");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al obtener detalles del lugar" });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener detalles del lugar");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor" });
#endif
            }
        }

        /// <summary>
        /// Autocompletado de lugares para búsquedas
        /// GET /api/GooglePlaces/autocomplete?input=rest&location=19.4326,-99.1332&radius=5000
        /// </summary>
        /// <param name="input">Texto de entrada del usuario (requerido)</param>
        /// <param name="location">Ubicación en formato "lat,lng" (opcional)</param>
        /// <param name="radius">Radio de búsqueda en metros (opcional)</param>
        /// <returns>Sugerencias de autocompletado en formato JSON de Google Places API</returns>
        [HttpGet("autocomplete")]
        public async Task<IActionResult> AutocompletePlaces(
            [FromQuery] string input,
            [FromQuery] string? location = null,
            [FromQuery] int? radius = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return BadRequest(new { message = "El parámetro 'input' es requerido" });
                }

                // Validar formato de location y radius
                var locationError = ValidateLocation(location);
                if (locationError != null) return locationError;

                var radiusError = ValidateRadius(radius);
                if (radiusError != null) return radiusError;

                _logger.LogInformation("Autocompletado de lugares solicitado: {Input}", input);

                var result = await _placesService.AutocompletePlacesAsync(input, location, radius);

                return Content(result, "application/json");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al autocompletar lugares");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al autocompletar lugares" });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al autocompletar lugares");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor" });
#endif
            }
        }
    }
}
