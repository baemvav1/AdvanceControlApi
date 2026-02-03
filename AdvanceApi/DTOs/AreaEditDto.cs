namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para operaciones CRUD de áreas/zonas de Google Maps
    /// </summary>
    public class AreaEditDto
    {
        public int IdArea { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? ColorMapa { get; set; }
        public decimal? Opacidad { get; set; }
        public string? ColorBorde { get; set; }
        public int? AnchoBorde { get; set; }
        public bool? Activo { get; set; }
        public string? TipoGeometria { get; set; }
        public decimal? CentroLatitud { get; set; }
        public decimal? CentroLongitud { get; set; }
        public decimal? Radio { get; set; }
        public decimal? BoundingBoxNE_Lat { get; set; }
        public decimal? BoundingBoxNE_Lng { get; set; }
        public decimal? BoundingBoxSW_Lat { get; set; }
        public decimal? BoundingBoxSW_Lng { get; set; }
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
