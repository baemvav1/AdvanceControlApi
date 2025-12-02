using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de relaciones refacción-equipo
    /// </summary>
    public interface IRelacionRefaccionEquipoService
    {
        /// <summary>
        /// Obtiene relaciones refacción-equipo usando el procedimiento almacenado
        /// </summary>
        /// <param name="query">Parámetros de búsqueda</param>
        /// <returns>Lista de relaciones que cumplen con los criterios de búsqueda</returns>
        Task<List<RelacionRefaccionEquipo>> GetRelacionesAsync(RelacionRefaccionEquipoQueryDto query);

        /// <summary>
        /// Crea una nueva relación refacción-equipo
        /// </summary>
        /// <param name="query">Datos de la relación a crear</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> CreateRelacionAsync(RelacionRefaccionEquipoQueryDto query);

        /// <summary>
        /// Elimina (soft delete) una relación refacción-equipo
        /// </summary>
        /// <param name="idRelacionRefaccion">ID de la relación a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteRelacionAsync(int idRelacionRefaccion);

        /// <summary>
        /// Actualiza la nota de una relación refacción-equipo
        /// </summary>
        /// <param name="query">Datos de la relación a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdateNotaAsync(RelacionRefaccionEquipoQueryDto query);
    }
}
