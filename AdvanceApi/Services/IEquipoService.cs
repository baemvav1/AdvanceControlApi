using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de equipos
    /// </summary>
    public interface IEquipoService
    {
        /// <summary>
        /// Ejecuta operaciones CRUD de equipos usando el procedimiento almacenado sp_equipo_edit
        /// </summary>
        /// <param name="query">Parámetros de la operación</param>
        /// <returns>Lista de equipos o resultado de la operación</returns>
        Task<List<Equipo>> ExecuteEquipoOperationAsync(EquipoQueryDto query);

        /// <summary>
        /// Elimina (soft delete) un equipo por su ID
        /// </summary>
        /// <param name="idEquipo">ID del equipo a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteEquipoAsync(int idEquipo);

        /// <summary>
        /// Actualiza un equipo por su ID
        /// </summary>
        /// <param name="query">Datos del equipo a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdateEquipoAsync(EquipoQueryDto query);

        /// <summary>
        /// Crea un nuevo equipo usando el procedimiento almacenado sp_equipo_edit con operacion='create'
        /// </summary>
        /// <param name="query">Datos del equipo a crear</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> CreateEquipoAsync(EquipoQueryDto query);
    }
}
