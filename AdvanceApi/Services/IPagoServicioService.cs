using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz del servicio de PagoServicio que define los métodos para gestionar pagos de servicio
    /// </summary>
    public interface IPagoServicioService
    {
        /// <summary>
        /// Crea un nuevo pago de servicio usando el procedimiento almacenado sp_CrearPagoServicio
        /// </summary>
        Task<object> CrearPagoServicioAsync(PagoServicioQueryDto query);

        /// <summary>
        /// Consulta pagos de servicio según los criterios especificados usando el procedimiento almacenado sp_ConsultarPagosServicio
        /// </summary>
        Task<List<PagoServicio>> ConsultarPagosServicioAsync(int? idMovimiento, string? tipoServicio, DateTime? fechaInicio, DateTime? fechaFin);
    }
}
