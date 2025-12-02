namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros del procedimiento almacenado sp_relacionEquipoCliente_edit (para refacciones)
    /// </summary>
    public class RelacionRefaccionEquipoQueryDto
    {
        /// <summary>
        /// Tipo de operación: 'select', 'delete', 'put', 'update_nota'
        /// </summary>
        public string Operacion { get; set; } = string.Empty;

        /// <summary>
        /// ID de la relación refaccion-equipo
        /// </summary>
        public int? IdRelacionRefaccion { get; set; }

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
        public int? IdEquipo { get; set; }
    }
}
