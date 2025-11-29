namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros del procedimiento almacenado sp_relacionEquipoCliente_edit
    /// </summary>
    public class RelacionEquipoClienteQueryDto
    {
        /// <summary>
        /// Tipo de operación: 'select', 'delete', 'put', 'update_nota'
        /// </summary>
        public string Operacion { get; set; } = string.Empty;

        /// <summary>
        /// Identificador del equipo (búsqueda parcial en select)
        /// </summary>
        public string? Identificador { get; set; }

        /// <summary>
        /// ID del cliente (0 para no filtrar en select)
        /// </summary>
        public int IdCliente { get; set; } = 0;

        /// <summary>
        /// Nota asociada a la relación
        /// </summary>
        public string? Nota { get; set; }
    }
}
