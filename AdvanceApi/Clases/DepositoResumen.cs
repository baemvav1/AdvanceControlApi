namespace Clases
{
    /// <summary>
    /// Modelo para el resumen de depósitos por tipo
    /// </summary>
    public class DepositoResumen
    {
        public string? TipoDeposito { get; set; }
        public int Cantidad { get; set; }
        public decimal? Total { get; set; }
        public decimal? Promedio { get; set; }
    }
}
