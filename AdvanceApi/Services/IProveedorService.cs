using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de proveedores
    /// </summary>
    public interface IProveedorService
    {
        /// <summary>
        /// Ejecuta operaciones CRUD de proveedores usando el procedimiento almacenado sp_proveedor_edit
        /// </summary>
        /// <param name="query">Parámetros de la operación</param>
        /// <returns>Lista de proveedores o resultado de la operación</returns>
        Task<List<Proveedor>> ExecuteProveedorOperationAsync(ProveedorQueryDto query);

        /// <summary>
        /// Elimina (soft delete) un proveedor por su ID
        /// </summary>
        /// <param name="idProveedor">ID del proveedor a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteProveedorAsync(int idProveedor);

        /// <summary>
        /// Actualiza un proveedor por su ID
        /// </summary>
        /// <param name="query">Datos del proveedor a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdateProveedorAsync(ProveedorQueryDto query);

        /// <summary>
        /// Crea un nuevo proveedor usando el procedimiento almacenado sp_proveedor_edit
        /// </summary>
        /// <param name="query">Datos del proveedor a crear</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> CreateProveedorAsync(ProveedorQueryDto query);
    }
}
