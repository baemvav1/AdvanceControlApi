using AdvanceApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz del servicio de Deposito que define los métodos para gestionar depósitos
    /// </summary>
    public interface IDepositoService
    {
        /// <summary>
        /// Crea un nuevo depósito usando el procedimiento almacenado sp_CrearDeposito
        /// </summary>
        Task<object> CrearDepositoAsync(DepositoQueryDto query);

        /// <summary>
        /// Consulta depósitos según los criterios especificados usando el procedimiento almacenado sp_ConsultarDepositos
        /// </summary>
        Task<List<object>> ConsultarDepositosAsync(int? idMovimiento, string? tipoDeposito, DateTime? fechaInicio, DateTime? fechaFin);
    }
}
