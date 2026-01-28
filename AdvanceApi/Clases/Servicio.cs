using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad Servicio
    /// </summary>
    public class Servicio
    {
        public int IdServicio { get; set; }
        public string? Concepto { get; set; }
        public string? Descripcion { get; set; }
        public double? Costo { get; set; }
        public bool? Estatus { get; set; }
    }
}
