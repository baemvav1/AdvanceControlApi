using System;

namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para las operaciones de creación de ComisionBancaria
    /// </summary>
    public class ComisionBancariaDto
    {
        public int? IdMovimiento { get; set; }
        public string? TipoComision { get; set; }
        public decimal? Monto { get; set; }
        public decimal? Iva { get; set; }
        public string? Referencia { get; set; }
    }
}
