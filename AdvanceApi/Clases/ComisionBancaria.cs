using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad ComisionBancaria
    /// </summary>
    public class ComisionBancaria
    {
        public int IdComision { get; set; }
        public int IdMovimiento { get; set; }
        public string? TipoComision { get; set; }
        public decimal Monto { get; set; }
        public decimal? Iva { get; set; }
        public string? Referencia { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Descripcion { get; set; }
    }
}
