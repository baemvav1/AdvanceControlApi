using System;

namespace Clases
{
    public class Cargo
    {
        public int IdCargo { get; set; }
        public int? IdTipoCargo { get; set; }
        public int? IdOperacion { get; set; }
        public int? IdRelacionCargo { get; set; }
        public double? Monto { get; set; }
        public string? Nota { get; set; }
        public string? DetalleRelacionado { get; set; }
        public string? TipoCargo { get; set; }
        public string? Proveedor { get; set; }
        public double? Cantidad { get; set; }
        public double? Unitario { get; set; }
    }
}
