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
    }
}
