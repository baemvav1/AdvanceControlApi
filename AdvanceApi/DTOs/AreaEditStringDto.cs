namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para recibir datos de área como strings desde el cliente.
    /// Todos los campos (excepto IdArea) se reciben como strings para evitar
    /// errores de conversión numérica en el cliente.
    /// La conversión a los tipos correctos se realiza en el servicio.
    /// </summary>
    public class AreaEditStringDto
    {
        public int IdArea { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? ColorMapa { get; set; }
        public string? Opacidad { get; set; }  // decimal
        public string? ColorBorde { get; set; }
        public string? AnchoBorde { get; set; }  // int
        public string? Activo { get; set; }  // bool
        public string? TipoGeometria { get; set; }
        public string? CentroLatitud { get; set; }  // decimal
        public string? CentroLongitud { get; set; }  // decimal
        public string? Radio { get; set; }  // decimal
        public string? BoundingBoxNE_Lat { get; set; }  // decimal
        public string? BoundingBoxNE_Lng { get; set; }  // decimal
        public string? BoundingBoxSW_Lat { get; set; }  // decimal
        public string? BoundingBoxSW_Lng { get; set; }  // decimal
        public string? EtiquetaMostrar { get; set; }  // bool
        public string? EtiquetaTexto { get; set; }
        public string? NivelZoom { get; set; }  // int
        public string? MetadataJSON { get; set; }
        public string? UsuarioCreacion { get; set; }
        public string? UsuarioModificacion { get; set; }
        public string? Coordenadas { get; set; }  // JSON array de coordenadas
        public string? AutoCalcularCentro { get; set; }  // bool
        public string? ValidarPoligonoLargo { get; set; }  // bool
    }
}
