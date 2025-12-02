namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los resultados de equipos en la operación select_equipos del procedimiento sp_relacionRefaccionEquipo_edit
    /// </summary>
    public class EquipoRelacionDto
    {
        /// <summary>
        /// ID del equipo
        /// </summary>
        public int? IdEquipo { get; set; }

        /// <summary>
        /// Marca del equipo
        /// </summary>
        public string? Marca { get; set; }

        /// <summary>
        /// Identificador del equipo
        /// </summary>
        public string? Identificador { get; set; }

        /// <summary>
        /// Creado (valor entero)
        /// </summary>
        public int? Creado { get; set; }
    }
}
