using System.Text.Json;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Implementación del servicio para Google Places API
    /// </summary>
    public class GooglePlacesService : IGooglePlacesService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GooglePlacesService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GooglePlacesService(
            IConfiguration configuration,
            ILogger<GooglePlacesService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // Use named HttpClient with configured timeout
            _httpClient = httpClientFactory?.CreateClient("GooglePlaces") 
                ?? throw new ArgumentNullException(nameof(httpClientFactory));

            _apiKey = _configuration["GoogleMaps:ApiKey"] 
                ?? throw new InvalidOperationException("Google Maps API Key no configurada");

            _logger.LogInformation("GooglePlacesService inicializado correctamente");
        }

        /// <summary>
        /// Busca lugares usando Google Places API (Text Search)
        /// </summary>
        public async Task<string> SearchPlacesAsync(string query, string? location = null, int? radius = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    throw new ArgumentException("El parámetro de búsqueda no puede estar vacío", nameof(query));
                }

                // Construir URL para Places API Text Search
                // Nota: Google Places API requiere la API key como parámetro de consulta en la URL.
                // No es posible usar headers para autenticación con esta API.
                var baseUrl = "https://maps.googleapis.com/maps/api/place/textsearch/json";
                var url = $"{baseUrl}?query={Uri.EscapeDataString(query)}&key={_apiKey}";

                // Agregar ubicación y radio si están especificados
                if (!string.IsNullOrWhiteSpace(location))
                {
                    url += $"&location={location}";
                    
                    if (radius.HasValue && radius.Value > 0)
                    {
                        url += $"&radius={radius.Value}";
                    }
                }

                _logger.LogInformation("Buscando lugares: {Query}", query);

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                
                // Validar la respuesta de Google
                var jsonDoc = JsonDocument.Parse(content);
                var status = jsonDoc.RootElement.GetProperty("status").GetString();
                
                if (status != "OK" && status != "ZERO_RESULTS")
                {
                    _logger.LogWarning("Google Places API retornó estado: {Status}", status);
                    
                    // Agregar mensaje de error más descriptivo
                    if (status == "REQUEST_DENIED")
                    {
                        var errorMessage = jsonDoc.RootElement.TryGetProperty("error_message", out var errorProp) 
                            ? errorProp.GetString() 
                            : "La solicitud fue denegada por Google Places API. Verifica la configuración de la API Key.";
                        throw new InvalidOperationException($"Error de Google Places API: {errorMessage}");
                    }
                }

                _logger.LogInformation("Búsqueda completada con estado: {Status}", status);
                return content;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de red al llamar a Google Places API");
                throw new InvalidOperationException("Error al conectar con Google Places API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al procesar respuesta de Google Places API");
                throw new InvalidOperationException("Error al procesar respuesta de Google Places API", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en búsqueda de lugares");
                throw;
            }
        }

        /// <summary>
        /// Obtiene detalles de un lugar específico por Place ID
        /// </summary>
        public async Task<string> GetPlaceDetailsAsync(string placeId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(placeId))
                {
                    throw new ArgumentException("El Place ID no puede estar vacío", nameof(placeId));
                }

                var baseUrl = "https://maps.googleapis.com/maps/api/place/details/json";
                var url = $"{baseUrl}?place_id={Uri.EscapeDataString(placeId)}&key={_apiKey}";

                _logger.LogInformation("Obteniendo detalles del lugar: {PlaceId}", placeId);

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                // Validar la respuesta
                var jsonDoc = JsonDocument.Parse(content);
                var status = jsonDoc.RootElement.GetProperty("status").GetString();

                if (status != "OK")
                {
                    _logger.LogWarning("Google Places API retornó estado: {Status} para Place ID: {PlaceId}", status, placeId);
                    
                    if (status == "REQUEST_DENIED")
                    {
                        var errorMessage = jsonDoc.RootElement.TryGetProperty("error_message", out var errorProp)
                            ? errorProp.GetString()
                            : "La solicitud fue denegada por Google Places API. Verifica la configuración de la API Key.";
                        throw new InvalidOperationException($"Error de Google Places API: {errorMessage}");
                    }
                }

                return content;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de red al obtener detalles del lugar");
                throw new InvalidOperationException("Error al conectar con Google Places API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al procesar respuesta de Google Places API");
                throw new InvalidOperationException("Error al procesar respuesta de Google Places API", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener detalles del lugar");
                throw;
            }
        }

        /// <summary>
        /// Búsqueda de autocompletado de lugares
        /// </summary>
        public async Task<string> AutocompletePlacesAsync(string input, string? location = null, int? radius = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    throw new ArgumentException("El texto de entrada no puede estar vacío", nameof(input));
                }

                var baseUrl = "https://maps.googleapis.com/maps/api/place/autocomplete/json";
                var url = $"{baseUrl}?input={Uri.EscapeDataString(input)}&key={_apiKey}";

                // Agregar ubicación y radio si están especificados
                if (!string.IsNullOrWhiteSpace(location))
                {
                    url += $"&location={location}";

                    if (radius.HasValue && radius.Value > 0)
                    {
                        url += $"&radius={radius.Value}";
                    }
                }

                _logger.LogInformation("Autocompletando lugares: {Input}", input);

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                // Validar la respuesta
                var jsonDoc = JsonDocument.Parse(content);
                var status = jsonDoc.RootElement.GetProperty("status").GetString();

                if (status != "OK" && status != "ZERO_RESULTS")
                {
                    _logger.LogWarning("Google Places API retornó estado: {Status}", status);

                    if (status == "REQUEST_DENIED")
                    {
                        var errorMessage = jsonDoc.RootElement.TryGetProperty("error_message", out var errorProp)
                            ? errorProp.GetString()
                            : "La solicitud fue denegada por Google Places API. Verifica la configuración de la API Key.";
                        throw new InvalidOperationException($"Error de Google Places API: {errorMessage}");
                    }
                }

                _logger.LogInformation("Autocompletado completado con estado: {Status}", status);
                return content;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de red al llamar a Google Places Autocomplete API");
                throw new InvalidOperationException("Error al conectar con Google Places API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al procesar respuesta de Google Places API");
                throw new InvalidOperationException("Error al procesar respuesta de Google Places API", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en autocompletado de lugares");
                throw;
            }
        }
    }
}
