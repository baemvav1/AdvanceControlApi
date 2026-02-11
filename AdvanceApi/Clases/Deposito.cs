using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad Deposito
    /// </summary>
    public class Deposito
    {
        public int DepositoID { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Monto { get; set; }
        public string? TipoDeposito { get; set; }
    }
}
