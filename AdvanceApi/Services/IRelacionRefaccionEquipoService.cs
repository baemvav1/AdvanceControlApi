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
        /// Obtiene refacciones asociadas a un equipo usando la operación select_refacciones
        /// </summary>
        /// <param name="idEquipo">ID del equipo</param>
        /// <returns>Lista de refacciones asociadas al equipo</returns>
        Task<List<RelacionRefaccionEquipo>> GetRefaccionesByEquipoAsync(int idEquipo);

        /// <summary>
        /// Obtiene equipos asociados a una refacción usando la operación select_equipos
        /// </summary>
        /// <param name="idRefaccion">ID de la refacción</param>
        /// <returns>Lista de equipos asociados a la refacción</returns>
        Task<List<EquipoRelacionDto>> GetEquiposByRefaccionAsync(int idRefaccion);

        /// <summary>
        /// Crea una nueva relación refacción-equipo
        /// </summary>
        /// <param name="query">Datos de la relación a crear</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> CreateRelacionAsync(RelacionRefaccionEquipoQueryDto query);

        /// <summary>
        /// Elimina (soft delete) una relación refacción-equipo
        /// </summary>
        /// <param name="idRelacionRefaccion">ID de la relación refacción</param>
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
