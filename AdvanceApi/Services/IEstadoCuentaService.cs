using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de EstadoCuenta
    /// </summary>
    public interface IEstadoCuentaService
    {
        /// <summary>
        /// Crea un nuevo estado de cuenta usando el procedimiento almacenado sp_CrearEstadoCuenta
        /// </summary>
        /// <param name="query">Datos del estado de cuenta a crear</param>
        /// <returns>Resultado de la operación con el ID del estado de cuenta creado</returns>
        Task<object> CrearEstadoCuentaAsync(EstadoCuentaQueryDto query);

        /// <summary>
        /// Edita un estado de cuenta existente usando el procedimiento almacenado sp_EditarEstadoCuenta
        /// </summary>
        /// <param name="query">Datos del estado de cuenta a editar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> EditarEstadoCuentaAsync(EstadoCuentaQueryDto query);

        /// <summary>
        /// Consulta estados de cuenta usando el procedimiento almacenado sp_ConsultarEstadoCuenta
        /// </summary>
        /// <param name="idEstadoCuenta">ID del estado de cuenta (opcional)</param>
        /// <param name="numeroCuenta">Número de cuenta (opcional)</param>
        /// <param name="fechaInicio">Fecha inicio del período (opcional)</param>
        /// <param name="fechaFin">Fecha fin del período (opcional)</param>
        /// <returns>Lista de estados de cuenta que cumplen con los criterios</returns>
        Task<List<EstadoCuenta>> ConsultarEstadoCuentaAsync(int? idEstadoCuenta = null, string? numeroCuenta = null, DateTime? fechaInicio = null, DateTime? fechaFin = null);
    }
}
