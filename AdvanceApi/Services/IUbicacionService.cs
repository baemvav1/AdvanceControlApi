using Clases;
using AdvanceApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de ubicaciones de Google Maps
    /// </summary>
    public interface IUbicacionService
    {
        /// <summary>
        /// Crea una nueva ubicación
        /// </summary>
        /// <param name="ubicacion">Datos de la ubicación a crear</param>
        /// <returns>Resultado de la operación con el ID de la nueva ubicación</returns>
        Task<object> CreateUbicacionAsync(UbicacionDto ubicacion);

        /// <summary>
        /// Obtiene una ubicación por su ID
        /// </summary>
        /// <param name="idUbicacion">ID de la ubicación</param>
        /// <returns>La ubicación encontrada o null</returns>
        Task<Ubicacion?> GetUbicacionByIdAsync(int idUbicacion);

        /// <summary>
        /// Obtiene una ubicación por su nombre exacto
        /// </summary>
        /// <param name="nombre">Nombre de la ubicación</param>
        /// <returns>La ubicación encontrada o null</returns>
        Task<Ubicacion?> GetUbicacionByNameAsync(string nombre);

        /// <summary>
        /// Obtiene todas las ubicaciones activas
        /// </summary>
        /// <returns>Lista de ubicaciones</returns>
        Task<List<Ubicacion>> GetAllUbicacionesAsync();

        /// <summary>
        /// Actualiza una ubicación existente
        /// </summary>
        /// <param name="ubicacion">Datos de la ubicación a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdateUbicacionAsync(UbicacionDto ubicacion);

        /// <summary>
        /// Elimina físicamente una ubicación
        /// </summary>
        /// <param name="idUbicacion">ID de la ubicación a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteUbicacionAsync(int idUbicacion);
    }
}
