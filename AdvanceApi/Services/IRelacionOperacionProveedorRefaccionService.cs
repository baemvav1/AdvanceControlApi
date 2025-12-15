using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de relaciones operación-proveedor-refacción
    /// </summary>
    public interface IRelacionOperacionProveedorRefaccionService
    {
        /// <summary>
        /// Obtiene relaciones operación-proveedor-refacción usando el procedimiento almacenado sp_relacionOperacion_ProveedorRefaccion_edit
        /// </summary>
        /// <param name="query">Parámetros de búsqueda</param>
        /// <returns>Lista de relaciones que cumplen con los criterios de búsqueda</returns>
        Task<List<RelacionOperacionProveedorRefaccion>> GetRelacionesAsync(RelacionOperacionProveedorRefaccionQueryDto query);

        /// <summary>
        /// Crea una nueva relación operación-proveedor-refacción
        /// </summary>
        /// <param name="query">Datos de la relación a crear</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> CreateRelacionAsync(RelacionOperacionProveedorRefaccionQueryDto query);

        /// <summary>
        /// Elimina (soft delete) una relación operación-proveedor-refacción
        /// </summary>
        /// <param name="idRelacionOperacionProveedorRefaccion">ID de la relación operación-proveedor-refacción</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteRelacionAsync(int idRelacionOperacionProveedorRefaccion);

        /// <summary>
        /// Actualiza la nota de una relación operación-proveedor-refacción
        /// </summary>
        /// <param name="query">Datos de la relación a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdateNotaAsync(RelacionOperacionProveedorRefaccionQueryDto query);
    }
}
