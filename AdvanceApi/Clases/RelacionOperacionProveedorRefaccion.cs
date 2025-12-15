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
        public string? Descripcion { get; set; }
        public string? Marca { get; set; }
        public string? Serie { get; set; }
        public float? Precio { get; set; }
        public string? Nota { get; set; }
    }
}
