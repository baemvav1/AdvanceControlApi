using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de relaciones proveedor-refacción
    /// </summary>
    public interface IRelacionProveedorRefaccionService
    {
        /// <summary>
        /// Obtiene refacciones asociadas a un proveedor usando el procedimiento almacenado sp_relacionProveedorRefaccion_edit
        /// </summary>
        /// <param name="query">Parámetros de búsqueda</param>
        /// <returns>Lista de refacciones que cumplen con los criterios de búsqueda</returns>
        Task<List<RelacionProveedorRefaccion>> GetRefaccionesAsync(RelacionProveedorRefaccionQueryDto query);

        /// <summary>
        /// Crea una nueva relación proveedor-refacción
        /// </summary>
        /// <param name="query">Datos de la relación a crear</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> CreateRelacionAsync(RelacionProveedorRefaccionQueryDto query);

        /// <summary>
        /// Elimina (soft delete) una relación proveedor-refacción
        /// </summary>
        /// <param name="idRelacionProveedor">ID de la relación proveedor-refacción</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteRelacionAsync(int idRelacionProveedor);

        /// <summary>
        /// Actualiza la nota de una relación proveedor-refacción
        /// </summary>
        /// <param name="query">Datos de la relación a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdateNotaAsync(RelacionProveedorRefaccionQueryDto query);

        /// <summary>
        /// Actualiza el precio de una relación proveedor-refacción
        /// </summary>
        /// <param name="query">Datos de la relación a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdatePrecioAsync(RelacionProveedorRefaccionQueryDto query);

        /// <summary>
        /// Obtiene proveedores que tienen una refacción específica con sus precios
        /// </summary>
        /// <param name="idRefaccion">ID de la refacción</param>
        /// <returns>Lista de proveedores con sus precios para la refacción</returns>
        Task<List<ProveedorPorRefaccion>> GetProveedoresByRefaccionAsync(int idRefaccion);
    }
}
