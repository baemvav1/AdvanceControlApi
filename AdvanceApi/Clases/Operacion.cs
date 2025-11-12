using System;

namespace Clases
{
    public class Operacion
    {
        public int IdOperacion { get; set; }
        public string? Concepto { get; set; }
        public int? IdCliente { get; set; }
        public decimal? Monto { get; set; }
        public decimal? Abono { get; set; }
        public decimal? Restante { get; set; }
        public string? Nota { get; set; }
        public bool? Estatus { get; set; }
    }
}
