using System;

namespace Clases
{
    /// <summary>
    /// Modelo para los resultados del procedimiento almacenado sp_OperacionEdit con operacion='select'
    /// </summary>
    public class OperacionDetalle
    {
        public int IdOperacion { get; set; }
        public int? IdTipo { get; set; }
        public string? RazonSocial { get; set; }
        public string? Identificador { get; set; }
        public string? Atiende { get; set; }
        public double? Monto { get; set; }
        public string? Nota { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }
        public bool? Finalizado { get; set; }
    }
}
