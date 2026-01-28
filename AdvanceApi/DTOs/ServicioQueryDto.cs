namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros del procedimiento almacenado sp_servicio_edit
    /// </summary>
    public class ServicioQueryDto
    {
        /// <summary>
        /// Tipo de operación: 'select', 'delete', 'update', 'put'
        /// </summary>
        public string Operacion { get; set; } = string.Empty;

        /// <summary>
        /// ID del servicio (requerido para delete y update)
        /// </summary>
        public int IdServicio { get; set; } = 0;

        /// <summary>
        /// Concepto del servicio (búsqueda parcial en select, actualización en update)
        /// </summary>
        public string? Concepto { get; set; }

        /// <summary>
        /// Descripción del servicio (búsqueda parcial en select, actualización en update)
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Costo del servicio
        /// </summary>
        public double? Costo { get; set; }

        /// <summary>
        /// Estatus del servicio (por defecto true)
        /// </summary>
        public bool Estatus { get; set; } = true;
    }
}
