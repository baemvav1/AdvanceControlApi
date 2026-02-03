using AdvanceApi.DTOs;
using Clases;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz del servicio de áreas/zonas para Google Maps
    /// </summary>
    public interface IAreaService
    {
        /// <summary>
        /// Obtiene áreas según los criterios de búsqueda
        /// </summary>
        Task<List<Area>> GetAreasAsync(AreaEditDto query);

        /// <summary>
        /// Obtiene áreas en formato optimizado para Google Maps
        /// </summary>
        Task<object> GetAreasGoogleMapsAsync(int idArea, bool? activo);

        /// <summary>
        /// Obtiene áreas en formato GeoJSON estándar
        /// </summary>
        Task<object> GetAreasGeoJsonAsync(int idArea, bool? activo);

        /// <summary>
        /// Crea una nueva área
        /// </summary>
        Task<object> CreateAreaAsync(AreaEditDto query);

        /// <summary>
        /// Crea una nueva área (recibe valores decimales como strings)
        /// </summary>
        Task<object> CreateAreaFromStringAsync(AreaEditStringDto query);

        /// <summary>
        /// Actualiza un área existente
        /// </summary>
        Task<object> UpdateAreaAsync(AreaEditDto query);

        /// <summary>
        /// Actualiza un área existente (recibe valores decimales como strings)
        /// </summary>
        Task<object> UpdateAreaFromStringAsync(AreaEditStringDto query);

        /// <summary>
        /// Elimina (soft delete) un área
        /// </summary>
        Task<object> DeleteAreaAsync(int idArea);

        /// <summary>
        /// Elimina físicamente un área
        /// </summary>
        Task<object> DeleteAreaPhysicalAsync(int idArea);

        /// <summary>
        /// Valida si un punto está dentro de un polígono
        /// </summary>
        Task<object> ValidatePointInPolygonAsync(int idArea, decimal latitud, decimal longitud);

        /// <summary>
        /// Valida si un punto está dentro de un polígono (recibe coordenadas como strings)
        /// </summary>
        Task<object> ValidatePointInPolygonFromStringAsync(int idArea, string latitud, string longitud);
    }
}
