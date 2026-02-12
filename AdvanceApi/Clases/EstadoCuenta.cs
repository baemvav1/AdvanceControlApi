using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad EstadoCuenta
    /// </summary>
    public class EstadoCuenta
    {
        public int IdEstadoCuenta { get; set; }
        public string? NumeroCuenta { get; set; }
        public string? Clabe { get; set; }
        public string? TipoCuenta { get; set; }
        public string? TipoMoneda { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? FechaCorte { get; set; }
        public decimal? SaldoInicial { get; set; }
        public decimal? TotalCargos { get; set; }
        public decimal? TotalAbonos { get; set; }
        public decimal? SaldoFinal { get; set; }
        public decimal? TotalComisiones { get; set; }
        public decimal? TotalISR { get; set; }
        public decimal? TotalIVA { get; set; }
        public DateTime? FechaCarga { get; set; }
    }
}
