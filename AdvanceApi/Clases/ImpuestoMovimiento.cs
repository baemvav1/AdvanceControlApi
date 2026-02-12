using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad ImpuestoMovimiento
    /// </summary>
    public class ImpuestoMovimiento
    {
        public int IdImpuesto { get; set; }
        public int IdMovimiento { get; set; }
        public string? TipoImpuesto { get; set; }
        public string? Rfc { get; set; }
        public decimal Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Descripcion { get; set; }
    }
}
