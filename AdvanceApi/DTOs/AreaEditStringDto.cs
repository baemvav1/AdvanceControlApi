namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para operaciones CRUD de áreas/zonas de Google Maps
    /// Recibe valores decimales como strings desde el cliente para evitar
    /// problemas de conversión numérica
    /// </summary>
    public class AreaEditStringDto
    {
        public int IdArea { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? ColorMapa { get; set; }
        public string? Opacidad { get; set; }  // Será convertido a decimal en el servicio
        public string? ColorBorde { get; set; }
        public int? AnchoBorde { get; set; }
        public bool? Activo { get; set; }
        public string? TipoGeometria { get; set; }
        public string? CentroLatitud { get; set; }  // Será convertido a decimal en el servicio
        public string? CentroLongitud { get; set; }  // Será convertido a decimal en el servicio
        public string? Radio { get; set; }  // Será convertido a decimal en el servicio
        public string? BoundingBoxNE_Lat { get; set; }  // Será convertido a decimal en el servicio
        public string? BoundingBoxNE_Lng { get; set; }  // Será convertido a decimal en el servicio
        public string? BoundingBoxSW_Lat { get; set; }  // Será convertido a decimal en el servicio
        public string? BoundingBoxSW_Lng { get; set; }  // Será convertido a decimal en el servicio
        public bool? EtiquetaMostrar { get; set; }
        public string? EtiquetaTexto { get; set; }
        public int? NivelZoom { get; set; }
        public string? MetadataJSON { get; set; }
        public string? UsuarioCreacion { get; set; }
        public string? UsuarioModificacion { get; set; }
        public string? Coordenadas { get; set; }  // JSON array de coordenadas
        public bool? AutoCalcularCentro { get; set; }
        public bool? ValidarPoligonoLargo { get; set; }
    }
}
