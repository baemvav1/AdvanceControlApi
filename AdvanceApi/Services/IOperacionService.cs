using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de operaciones
    /// </summary>
    public interface IOperacionService
    {
        /// <summary>
        /// Obtiene operaciones usando el procedimiento almacenado sp_operacion_select
        /// </summary>
        /// <param name="query">Parámetros de búsqueda</param>
        /// <returns>Lista de operaciones que cumplen con los criterios de búsqueda</returns>
        Task<List<Operacion>> GetOperacionesAsync(OperacionQueryDto query);
    }
}
