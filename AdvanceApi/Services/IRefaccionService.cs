using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de refacciones
    /// </summary>
    public interface IRefaccionService
    {
        /// <summary>
        /// Ejecuta operaciones CRUD de refacciones usando el procedimiento almacenado sp_refaccion_edit
        /// </summary>
        /// <param name="query">Parámetros de la operación</param>
        /// <returns>Lista de refacciones o resultado de la operación</returns>
        Task<List<Refaccion>> ExecuteRefaccionOperationAsync(RefaccionQueryDto query);

        /// <summary>
        /// Elimina (soft delete) una refacción por su ID
        /// </summary>
        /// <param name="idRefaccion">ID de la refacción a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteRefaccionAsync(int idRefaccion);

        /// <summary>
        /// Actualiza una refacción por su ID
        /// </summary>
        /// <param name="query">Datos de la refacción a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdateRefaccionAsync(RefaccionQueryDto query);

        /// <summary>
        /// Crea una nueva refacción usando el procedimiento almacenado sp_refaccion_edit con operación 'put'
        /// </summary>
        /// <param name="query">Datos de la refacción a crear</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> CreateRefaccionAsync(RefaccionQueryDto query);

        /// <summary>
        /// Verifica si una refacción tiene proveedores relacionados
        /// </summary>
        /// <param name="idRefaccion">ID de la refacción a verificar</param>
        /// <returns>Resultado indicando si existe relación con proveedores</returns>
        Task<object> CheckProveedorExistsAsync(int idRefaccion);
    }
}
