using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de servicios
    /// </summary>
    public interface IServicioService
    {
        /// <summary>
        /// Ejecuta operaciones CRUD de servicios usando el procedimiento almacenado sp_servicio_edit
        /// </summary>
        /// <param name="query">Parámetros de la operación</param>
        /// <returns>Lista de servicios o resultado de la operación</returns>
        Task<List<Servicio>> ExecuteServicioOperationAsync(ServicioQueryDto query);

        /// <summary>
        /// Elimina (soft delete) un servicio por su ID
        /// </summary>
        /// <param name="idServicio">ID del servicio a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteServicioAsync(int idServicio);

        /// <summary>
        /// Actualiza un servicio por su ID
        /// </summary>
        /// <param name="query">Datos del servicio a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdateServicioAsync(ServicioQueryDto query);

        /// <summary>
        /// Crea un nuevo servicio usando el procedimiento almacenado sp_servicio_edit con operación 'put'
        /// </summary>
        /// <param name="query">Datos del servicio a crear</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> CreateServicioAsync(ServicioQueryDto query);
    }
}
