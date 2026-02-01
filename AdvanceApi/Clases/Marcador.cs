namespace Clases
{
    /// <summary>
    /// Modelo para marcadores/puntos de interés en Google Maps
    /// </summary>
    public class Marcador
    {
        public int IdMarcador { get; set; }
        public int? IdArea { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public string? Icono { get; set; }
        public string? ColorIcono { get; set; }
        public string? Etiqueta { get; set; }
        public string? Animacion { get; set; }
        public bool? Visible { get; set; }
        public bool? Clickable { get; set; }
        public int? NivelZoom { get; set; }
        public string? InfoWindowHTML { get; set; }
        public string? MetadataJSON { get; set; }
        public bool? Activo { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
