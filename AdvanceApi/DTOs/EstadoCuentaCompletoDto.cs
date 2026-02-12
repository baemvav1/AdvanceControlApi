using Clases;
using System.Collections.Generic;

namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para el resultado completo del estado de cuenta con todos sus datos relacionados
    /// Utilizado por el endpoint edoCuentaConsulta que consume sp_ConsultarEstadoCuentaCompleto
    /// </summary>
    public class EstadoCuentaCompletoDto
    {
        /// <summary>
        /// Información del estado de cuenta
        /// </summary>
        public EstadoCuenta? EstadoCuenta { get; set; }

        /// <summary>
        /// Lista de movimientos asociados al estado de cuenta
        /// </summary>
        public List<Movimiento>? Movimientos { get; set; }

        /// <summary>
        /// Lista de transferencias SPEI asociadas a los movimientos
        /// </summary>
        public List<TransferenciaSPEI>? TransferenciasSPEI { get; set; }

        /// <summary>
        /// Lista de comisiones bancarias asociadas a los movimientos
        /// </summary>
        public List<ComisionBancaria>? Comisiones { get; set; }

        /// <summary>
        /// Lista de impuestos asociados a los movimientos
        /// </summary>
        public List<ImpuestoMovimiento>? Impuestos { get; set; }
    }
}
