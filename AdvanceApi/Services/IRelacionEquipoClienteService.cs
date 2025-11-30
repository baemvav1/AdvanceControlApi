using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de relaciones equipo-cliente
    /// </summary>
    public interface IRelacionEquipoClienteService
    {
        /// <summary>
        /// Obtiene relaciones equipo-cliente usando el procedimiento almacenado sp_relacionEquipoCliente_edit
        /// </summary>
        /// <param name="query">Parámetros de búsqueda</param>
        /// <returns>Lista de relaciones que cumplen con los criterios de búsqueda</returns>
        Task<List<RelacionEquipoCliente>> GetRelacionesAsync(RelacionEquipoClienteQueryDto query);

        /// <summary>
        /// Crea una nueva relación equipo-cliente
        /// </summary>
        /// <param name="query">Datos de la relación a crear</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> CreateRelacionAsync(RelacionEquipoClienteQueryDto query);

        /// <summary>
        /// Elimina (soft delete) una relación equipo-cliente
        /// </summary>
        /// <param name="identificador">Identificador del equipo</param>
        /// <param name="idCliente">ID del cliente</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteRelacionAsync(string identificador, int idCliente);

        /// <summary>
        /// Actualiza la nota de una relación equipo-cliente
        /// </summary>
        /// <param name="query">Datos de la relación a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdateNotaAsync(RelacionEquipoClienteQueryDto query);
    }
}
