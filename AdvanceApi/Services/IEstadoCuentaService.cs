using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de gestión de estados de cuenta
    /// </summary>
    public interface IEstadoCuentaService
    {
        /// <summary>
        /// Obtiene todos los estados de cuenta con resumen de depósitos
        /// </summary>
        /// <returns>Lista de estados de cuenta</returns>
        Task<List<EstadoCuenta>> GetEstadosCuentaAsync();

        /// <summary>
        /// Crea un nuevo estado de cuenta
        /// </summary>
        /// <param name="query">Datos del estado de cuenta</param>
        /// <returns>Resultado de la operación con el ID creado</returns>
        Task<object> CreateEstadoCuentaAsync(EstadoCuentaQueryDto query);

        /// <summary>
        /// Agrega un depósito a un estado de cuenta
        /// </summary>
        /// <param name="query">Datos del depósito</param>
        /// <returns>Resultado de la operación con el ID del depósito</returns>
        Task<object> CreateDepositoAsync(EstadoCuentaQueryDto query);

        /// <summary>
        /// Obtiene los depósitos de un estado de cuenta específico
        /// </summary>
        /// <param name="estadoCuentaId">ID del estado de cuenta</param>
        /// <returns>Lista de depósitos</returns>
        Task<List<Deposito>> GetDepositosAsync(int estadoCuentaId);

        /// <summary>
        /// Obtiene el resumen de depósitos por tipo de un estado de cuenta
        /// </summary>
        /// <param name="estadoCuentaId">ID del estado de cuenta</param>
        /// <returns>Lista de resumen por tipo de depósito</returns>
        Task<List<DepositoResumen>> GetResumenDepositosAsync(int estadoCuentaId);

        /// <summary>
        /// Verifica si un depósito específico ya existe
        /// </summary>
        /// <param name="query">Datos del depósito a verificar</param>
        /// <returns>Lista con el resultado de la verificación</returns>
        Task<List<DepositoVerificacion>> VerificarDepositoAsync(EstadoCuentaQueryDto query);

        /// <summary>
        /// Busca posibles depósitos duplicados en un estado de cuenta
        /// </summary>
        /// <param name="estadoCuentaId">ID del estado de cuenta</param>
        /// <returns>Lista de posibles duplicados</returns>
        Task<List<DepositoDuplicado>> BuscarDuplicadosAsync(int estadoCuentaId);
    }
}
