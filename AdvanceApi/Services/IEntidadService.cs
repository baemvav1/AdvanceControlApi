using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de entidades
    /// </summary>
    public interface IEntidadService
    {
        /// <summary>
        /// Ejecuta operaciones CRUD de entidades usando el procedimiento almacenado sp_entidad_edit
        /// </summary>
        /// <param name="query">Parámetros de la operación</param>
        /// <returns>Lista de entidades o resultado de la operación</returns>
        Task<List<Entidad>> ExecuteEntidadOperationAsync(EntidadQueryDto query);

        /// <summary>
        /// Elimina (soft delete) una entidad por su ID
        /// </summary>
        /// <param name="idEntidad">ID de la entidad a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteEntidadAsync(int idEntidad);

        /// <summary>
        /// Actualiza una entidad por su ID
        /// </summary>
        /// <param name="query">Datos de la entidad a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdateEntidadAsync(EntidadQueryDto query);

        /// <summary>
        /// Crea una nueva entidad usando el procedimiento almacenado sp_entidad_edit con operacion='create'
        /// </summary>
        /// <param name="query">Datos de la entidad a crear</param>
        /// <returns>Resultado de la operación con el ID de la entidad creada</returns>
        Task<object> CreateEntidadAsync(EntidadQueryDto query);
    }
}
