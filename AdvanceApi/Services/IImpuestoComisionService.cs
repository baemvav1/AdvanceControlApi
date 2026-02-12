using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz del servicio de ImpuestoComision que define los métodos para gestionar impuestos y comisiones de movimientos
    /// </summary>
    public interface IImpuestoComisionService
    {
        /// <summary>
        /// Crea un nuevo impuesto de movimiento usando el procedimiento almacenado sp_CrearImpuestoMovimiento
        /// </summary>
        Task<object> CrearImpuestoMovimientoAsync(ImpuestoMovimientoDto dto);

        /// <summary>
        /// Consulta impuestos de movimiento usando el procedimiento almacenado sp_ConsultarImpuestosMovimiento
        /// </summary>
        Task<List<ImpuestoMovimiento>> ConsultarImpuestosMovimientoAsync(int? idMovimiento, string? tipoImpuesto);

        /// <summary>
        /// Crea una nueva comisión bancaria usando el procedimiento almacenado sp_CrearComisionBancaria
        /// </summary>
        Task<object> CrearComisionBancariaAsync(ComisionBancariaDto dto);

        /// <summary>
        /// Consulta comisiones bancarias usando el procedimiento almacenado sp_ConsultarComisionesBancarias
        /// </summary>
        Task<List<ComisionBancaria>> ConsultarComisionesBancariasAsync(int? idMovimiento, string? tipoComision, DateTime? fechaInicio, DateTime? fechaFin);
    }
}
