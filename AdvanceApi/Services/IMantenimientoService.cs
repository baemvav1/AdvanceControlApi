using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de mantenimiento
    /// </summary>
    public interface IMantenimientoService
    {
        /// <summary>
        /// Obtiene mantenimientos usando el procedimiento almacenado sp_MatenimientoEdit
        /// </summary>
        /// <param name="query">Parámetros de búsqueda</param>
        /// <returns>Lista de mantenimientos que cumplen con los criterios de búsqueda</returns>
        Task<List<Mantenimiento>> GetMantenimientosAsync(MantenimientoQueryDto query);

        /// <summary>
        /// Crea un nuevo mantenimiento
        /// </summary>
        /// <param name="query">Datos del mantenimiento a crear</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> CreateMantenimientoAsync(MantenimientoQueryDto query);

        /// <summary>
        /// Elimina (soft delete) un mantenimiento
        /// </summary>
        /// <param name="idMantenimiento">ID del mantenimiento</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteMantenimientoAsync(int idMantenimiento);
    }
}
