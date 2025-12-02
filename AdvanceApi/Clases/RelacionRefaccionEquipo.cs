namespace Clases
{
    /// <summary>
    /// Modelo para la relación entre refacción y equipo
    /// </summary>
    public class RelacionRefaccionEquipo
    {
        /// <summary>
        /// ID de la refacción
        /// </summary>
        public int? IdRefaccion { get; set; }

        /// <summary>
        /// Marca de la refacción
        /// </summary>
        public string? Marca { get; set; }

        /// <summary>
        /// Serie de la refacción
        /// </summary>
        public string? Serie { get; set; }

        /// <summary>
        /// Costo de la refacción
        /// </summary>
        public double? Costo { get; set; }

        /// <summary>
        /// Descripción de la refacción
        /// </summary>
        public string? Descripcion { get; set; }
    }
}
