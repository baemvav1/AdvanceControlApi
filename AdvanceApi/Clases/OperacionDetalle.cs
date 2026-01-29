using System;

namespace Clases
{
    /// <summary>
    /// Modelo para los resultados del procedimiento almacenado sp_OperacionEdit con operacion='select'
    /// </summary>
    public class OperacionDetalle
    {
        public int IdOperacion { get; set; } = 0;
        public int? IdTipo { get; set; } = 0;
        public string? RazonSocial { get; set; } = null;
        public string? Identificador { get; set; } = null;
        public string? Atiende { get; set; } = null;
        public int? IdAtiende { get; set; } = 0;
        public double? Monto { get; set; } = 0;
        public string? Nota { get; set; } = "";
        public DateTime? FechaInicio { get; set; } = DateTime.Now;
        public DateTime? FechaFinal { get; set; } = null;
        public bool? Finalizado { get; set; } = false;
    }
}
