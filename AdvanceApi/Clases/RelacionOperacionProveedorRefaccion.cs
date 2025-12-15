using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad RelacionOperacionProveedorRefaccion
    /// </summary>
    public class RelacionOperacionProveedorRefaccion
    {
        public int? IdRelacionOperacionProveedorRefaccion { get; set; }
        public int? IdProveedorRefaccion { get; set; }
        public double? Precio { get; set; }
        public string? Nota { get; set; }
    }
}
