using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad PagoServicio
    /// </summary>
    public class PagoServicio
    {
        public int IdPago { get; set; }
        public int IdMovimiento { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public decimal Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Descripcion { get; set; }
    }
}
