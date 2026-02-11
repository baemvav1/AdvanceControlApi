using System;

namespace Clases
{
    /// <summary>
    /// Modelo para el resultado de verificación de depósito
    /// </summary>
    public class DepositoVerificacion
    {
        public string? Estado { get; set; }
        public int? DepositoID { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Monto { get; set; }
        public string? TipoDeposito { get; set; }
    }
}
