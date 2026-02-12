using System;

namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para las operaciones de creación de ImpuestoMovimiento
    /// </summary>
    public class ImpuestoMovimientoDto
    {
        public int? IdMovimiento { get; set; }
        public string? TipoImpuesto { get; set; }
        public string? Rfc { get; set; }
        public decimal? Monto { get; set; }
    }
}
