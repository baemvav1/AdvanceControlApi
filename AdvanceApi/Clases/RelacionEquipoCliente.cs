namespace Clases
{
    /// <summary>
    /// Modelo para la relación entre equipo y cliente
    /// </summary>
    public class RelacionEquipoCliente
    {
        /// <summary>
        /// ID de la relación
        /// </summary>
        public int? IdRelacion { get; set; }

        /// <summary>
        /// ID del cliente
        /// </summary>
        public int? IdCliente { get; set; }

        /// <summary>
        /// Razón social del cliente
        /// </summary>
        public string? RazonSocial { get; set; }

        /// <summary>
        /// Nombre comercial del cliente
        /// </summary>
        public string? NombreComercial { get; set; }
    }
}
