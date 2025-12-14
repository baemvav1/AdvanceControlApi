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
        /// Obtiene operaciones usando el procedimiento almacenado sp_OperacionEdit con operacion='select'
        /// </summary>
        /// <param name="query">Parámetros de búsqueda</param>
        /// <returns>Lista de operaciones que cumplen con los criterios de búsqueda</returns>
        Task<List<OperacionDetalle>> GetOperacionesAsync(OperacionQueryDto query);

        /// <summary>
        /// Elimina (soft delete) una operación usando el procedimiento almacenado sp_OperacionEdit con operacion='delete'
        /// </summary>
        /// <param name="idOperacion">ID de la operación a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteOperacionAsync(int idOperacion);
    }
}
