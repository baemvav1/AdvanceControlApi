namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros del procedimiento almacenado sp_MatenimientoEdit
    /// </summary>
    public class MantenimientoQueryDto
    {
        /// <summary>
        /// Tipo de operación: 'select', 'delete', 'put', 'update_atendido'
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
        /// Nota asociada al mantenimiento
        /// </summary>
        public string? Nota { get; set; }

        /// <summary>
        /// ID del mantenimiento (usado en operación delete y update_atendido)
        /// </summary>
        public int? IdMantenimiento { get; set; }

        /// <summary>
        /// ID del equipo (usado en operación put)
        /// </summary>
        public int? IdEquipo { get; set; }

        /// <summary>
        /// ID del tipo de mantenimiento (usado en operación put)
        /// </summary>
        public int? IdTipoMantenimiento { get; set; }

        /// <summary>
        /// Indica si el mantenimiento fue atendido (usado en operación update_atendido)
        /// </summary>
        public bool? Atendido { get; set; }

        /// <summary>
        /// ID del usuario que atendió el mantenimiento (usado en operación update_atendido)
        /// </summary>
        public int IdAtendio { get; set; } = 0;
    }
}
