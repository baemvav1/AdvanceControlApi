namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros del procedimiento almacenado sp_equipo_edit
    /// </summary>
    public class EquipoQueryDto
    {
        /// <summary>
        /// Tipo de operación: 'select', 'delete', 'update'
        /// </summary>
        public string Operacion { get; set; } = string.Empty;

        /// <summary>
        /// ID del equipo (requerido para delete y update)
        /// </summary>
        public int IdEquipo { get; set; } = 0;

        /// <summary>
        /// Marca del equipo (búsqueda parcial en select, actualización en update)
        /// </summary>
        public string? Marca { get; set; }

        /// <summary>
        /// Año de creación del equipo
        /// </summary>
        public int? Creado { get; set; }

        /// <summary>
        /// Número de paradas del equipo
        /// </summary>
        public int? Paradas { get; set; }

        /// <summary>
        /// Capacidad en kilogramos del equipo
        /// </summary>
        public int? Kilogramos { get; set; }

        /// <summary>
        /// Capacidad de personas del equipo
        /// </summary>
        public int? Personas { get; set; }

        /// <summary>
        /// Descripción del equipo (búsqueda parcial en select, actualización en update)
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Identificador único del equipo (búsqueda parcial en select, actualización en update)
        /// </summary>
        public string? Identificador { get; set; }

        /// <summary>
        /// Estatus del equipo (por defecto true)
        /// </summary>
        public bool Estatus { get; set; } = true;
    }
}
