using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad Equipo
    /// </summary>
    public class Equipo
    {
        public int IdEquipo { get; set; }
        public string? Marca { get; set; }
        public int? Creado { get; set; }
        public string? Descripcion { get; set; }
        public string? Identificador { get; set; }
        public bool? Estatus { get; set; }
    }
}
