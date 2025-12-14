using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad RelacionProveedorRefaccion
    /// </summary>
    public class RelacionProveedorRefaccion
    {
        public int? IdRefaccion { get; set; }
        public string? Marca { get; set; }
        public string? Serie { get; set; }
        public double? Costo { get; set; }
        public string? Descripcion { get; set; }
    }
}
