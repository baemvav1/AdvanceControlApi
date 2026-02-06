using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad Entidad
    /// </summary>
    public class Entidad
    {
        public int IdEntidad { get; set; }
        public string? NombreComercial { get; set; }
        public string? RazonSocial { get; set; }
        public string? RFC { get; set; }
        public string? CP { get; set; }
        public string? Estado { get; set; }
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
        public string? Calle { get; set; }
        public string? NumExt { get; set; }
        public string? NumInt { get; set; }
        public string? Colonia { get; set; }
        public string? Apoderado { get; set; }
        public bool? Estatus { get; set; }
    }
}
