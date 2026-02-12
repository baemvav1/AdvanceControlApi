using System;

namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para las operaciones de creación y edición de movimientos
    /// </summary>
    public class MovimientoQueryDto
    {
        public int? IdMovimiento { get; set; }
        public int? IdEstadoCuenta { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Descripcion { get; set; }
        public string? Referencia { get; set; }
        public decimal? Cargo { get; set; }
        public decimal? Abono { get; set; }
        public decimal? Saldo { get; set; }
        public string? TipoOperacion { get; set; }
    }
}
