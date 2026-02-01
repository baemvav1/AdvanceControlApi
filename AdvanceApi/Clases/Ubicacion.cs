using System;

namespace Clases
{
    public class Ubicacion
    {
        public int IdUbicacion { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public string? DireccionCompleta { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public string? PlaceId { get; set; }
        public string? Icono { get; set; }
        public string? ColorIcono { get; set; }
        public int? NivelZoom { get; set; }
        public string? InfoWindowHTML { get; set; }
        public string? Categoria { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? MetadataJSON { get; set; }
        public bool? Activo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioCreacion { get; set; }
        public string? UsuarioModificacion { get; set; }
    }
}
