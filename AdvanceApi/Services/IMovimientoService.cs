using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz del servicio de Movimiento que define los métodos para gestionar movimientos
    /// </summary>
    public interface IMovimientoService
    {
        /// <summary>
        /// Crea un nuevo movimiento usando el procedimiento almacenado sp_CrearMovimiento
        /// </summary>
        Task<object> CrearMovimientoAsync(MovimientoQueryDto query);

        /// <summary>
        /// Edita un movimiento existente usando el procedimiento almacenado sp_EditarMovimiento
        /// </summary>
        Task<object> EditarMovimientoAsync(MovimientoQueryDto query);

        /// <summary>
        /// Consulta movimientos según los criterios especificados usando el procedimiento almacenado sp_ConsultarMovimientos
        /// </summary>
        Task<List<Movimiento>> ConsultarMovimientosAsync(int? idEstadoCuenta, DateTime? fechaInicio, DateTime? fechaFin, string? tipoOperacion);
    }
}
