using System;

namespace Clases
{
    public class Operacion
    {
        public int IdOperacion { get; set; }
        public string? Concepto { get; set; }
        public int? IdCliente { get; set; }
        public double? Monto { get; set; }
        public double? Abono { get; set; }
        public double? Restante { get; set; }
        public string? Nota { get; set; }
        public bool? Estatus { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }
        public bool? Finalizado { get; set; }
    }
}
