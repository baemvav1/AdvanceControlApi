namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para operaciones de Deposito
    /// </summary>
    public class DepositoQueryDto
    {
        public int? IdMovimiento { get; set; }
        public string? TipoDeposito { get; set; }
        public string? Referencia { get; set; }
        public decimal? Monto { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
