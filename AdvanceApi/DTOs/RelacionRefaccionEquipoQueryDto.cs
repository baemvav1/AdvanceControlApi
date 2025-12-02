namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros del procedimiento almacenado sp_relacionRefaccionEquipo_edit
    /// </summary>
    public class RelacionRefaccionEquipoQueryDto
    {
        /// <summary>
        /// Tipo de operación: 'select', 'delete', 'put', 'update_nota'
        /// </summary>
        public string Operacion { get; set; } = string.Empty;

        /// <summary>
        /// ID de la relación refacción (para delete y update_nota)
        /// </summary>
        public int IdRelacionRefaccion { get; set; } = 0;

        /// <summary>
        /// ID de la refacción (0 para no filtrar en select)
        /// </summary>
        public int IdRefaccion { get; set; } = 0;

        /// <summary>
        /// Nota asociada a la relación
        /// </summary>
        public string? Nota { get; set; }

        /// <summary>
        /// ID del equipo (0 para no filtrar en select)
        /// </summary>
        public int IdEquipo { get; set; } = 0;
    }
}
