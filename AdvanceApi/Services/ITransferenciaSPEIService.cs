using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz del servicio de TransferenciaSPEI que define los métodos para gestionar transferencias SPEI
    /// </summary>
    public interface ITransferenciaSPEIService
    {
        /// <summary>
        /// Crea una nueva transferencia SPEI usando el procedimiento almacenado sp_CrearTransferenciaSPEI
        /// </summary>
        Task<object> CrearTransferenciaSPEIAsync(TransferenciaSPEICreateDto dto);

        /// <summary>
        /// Consulta transferencias SPEI según los criterios especificados usando el procedimiento almacenado sp_ConsultarTransferenciasSPEI
        /// </summary>
        Task<List<TransferenciaSPEI>> ConsultarTransferenciasSPEIAsync(TransferenciaSPEIQueryDto query);
    }
}
