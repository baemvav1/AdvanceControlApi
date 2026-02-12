using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad Movimiento
    /// </summary>
    public class Movimiento
    {
        public int IdMovimiento { get; set; }
        public int IdEstadoCuenta { get; set; }
        public DateTime Fecha { get; set; }
        public string? Descripcion { get; set; }
        public string? Referencia { get; set; }
        public decimal? Cargo { get; set; }
        public decimal? Abono { get; set; }
        public decimal Saldo { get; set; }
        public string? TipoOperacion { get; set; }
        public DateTime? FechaCarga { get; set; }
        public string? NumeroCuenta { get; set; }
        public string? Clabe { get; set; }
    }
}
