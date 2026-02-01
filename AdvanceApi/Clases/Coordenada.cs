namespace Clases
{
    /// <summary>
    /// Modelo para coordenadas de polígonos en Google Maps
    /// </summary>
    public class Coordenada
    {
        public int IdCoordenada { get; set; }
        public int IdArea { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public int Orden { get; set; }
        public bool? EsVerticeControl { get; set; }
        public decimal? Altitud { get; set; }
        public decimal? Precision { get; set; }
    }
}
