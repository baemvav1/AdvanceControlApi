using System;

namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para las operaciones de creación y consulta de pagos de servicio
    /// </summary>
    public class PagoServicioQueryDto
    {
        public int? IdMovimiento { get; set; }
        public string? TipoServicio { get; set; }
        public string? Referencia { get; set; }
        public decimal? Monto { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
