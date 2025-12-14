using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad Proveedor
    /// </summary>
    public class Proveedor
    {
        public int IdProveedor { get; set; }
        public string? Rfc { get; set; }
        public string? RazonSocial { get; set; }
        public string? NombreComercial { get; set; }
        public bool? Estatus { get; set; }
        public string? Nota { get; set; }
    }
}
