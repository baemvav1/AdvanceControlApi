namespace AdvanceApi.Services
{
    /// <summary>
    /// Servicio para interactuar con Google Places API
    /// </summary>
    public interface IGooglePlacesService
    {
        /// <summary>
        /// Busca lugares usando Google Places API (Text Search)
        /// </summary>
        /// <param name="query">Texto de búsqueda</param>
        /// <param name="location">Ubicación opcional en formato "lat,lng"</param>
        /// <param name="radius">Radio de búsqueda en metros (opcional)</param>
        /// <returns>Resultado de la búsqueda en formato JSON</returns>
        Task<string> SearchPlacesAsync(string query, string? location = null, int? radius = null);

        /// <summary>
        /// Obtiene detalles de un lugar específico por Place ID
        /// </summary>
        /// <param name="placeId">ID del lugar en Google Places</param>
        /// <returns>Detalles del lugar en formato JSON</returns>
        Task<string> GetPlaceDetailsAsync(string placeId);

        /// <summary>
        /// Búsqueda de autocompletado de lugares
        /// </summary>
        /// <param name="input">Texto de entrada del usuario</param>
        /// <param name="location">Ubicación opcional en formato "lat,lng"</param>
        /// <param name="radius">Radio de búsqueda en metros (opcional)</param>
        /// <returns>Sugerencias de autocompletado en formato JSON</returns>
        Task<string> AutocompletePlacesAsync(string input, string? location = null, int? radius = null);
    }
}
