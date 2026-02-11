using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad EstadoCuenta
    /// </summary>
    public class EstadoCuenta
    {
        public int EstadoCuentaID { get; set; }
        public DateTime? FechaCorte { get; set; }
        public DateTime? PeriodoDesde { get; set; }
        public DateTime? PeriodoHasta { get; set; }
        public decimal? SaldoInicial { get; set; }
        public decimal? SaldoCorte { get; set; }
        public decimal? TotalDepositos { get; set; }
        public decimal? TotalRetiros { get; set; }
        public decimal? Comisiones { get; set; }
        public string? NombreArchivo { get; set; }
        public DateTime? FechaProcesamiento { get; set; }
        public int? CantidadDepositos { get; set; }
        public decimal? TotalDepositosRegistrados { get; set; }
    }
}
