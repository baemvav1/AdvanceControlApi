namespace Clases
{
    /// <summary>
    /// Modelo para el mantenimiento de equipos
    /// </summary>
    public class Mantenimiento
    {
        /// <summary>
        /// ID del mantenimiento
        /// </summary>
        public int? IdMantenimiento { get; set; }

        /// <summary>
        /// Tipo de mantenimiento
        /// </summary>
        public string? TipoMantenimiento { get; set; }

        /// <summary>
        /// Nombre comercial del cliente
        /// </summary>
        public string? NombreComercial { get; set; }

        /// <summary>
        /// Razón social del cliente
        /// </summary>
        public string? RazonSocial { get; set; }

        /// <summary>
        /// Nota asociada al mantenimiento
        /// </summary>
        public string? Nota { get; set; }

        /// <summary>
        /// Identificador del equipo
        /// </summary>
        public string? Identificador { get; set; }

        /// <summary>
        /// Costo del mantenimiento
        /// </summary>
        public double? Costo { get; set; }

        /// <summary>
        /// Costo total del mantenimiento
        /// </summary>
        public double? CostoTotal { get; set; }
    }
}
