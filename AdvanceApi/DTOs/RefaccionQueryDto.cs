namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros del procedimiento almacenado sp_refaccion_edit
    /// </summary>
    public class RefaccionQueryDto
    {
        /// <summary>
        /// Tipo de operación: 'select', 'delete', 'update', 'put'
        /// </summary>
        public string Operacion { get; set; } = string.Empty;

        /// <summary>
        /// ID de la refacción (requerido para delete y update)
        /// </summary>
        public int IdRefaccion { get; set; } = 0;

        /// <summary>
        /// Marca de la refacción (búsqueda parcial en select, actualización en update)
        /// </summary>
        public string? Marca { get; set; }

        /// <summary>
        /// Serie de la refacción (búsqueda parcial en select, actualización en update)
        /// </summary>
        public string? Serie { get; set; }

        /// <summary>
        /// Costo de la refacción
        /// </summary>
        public double? Costo { get; set; }

        /// <summary>
        /// Descripción de la refacción (búsqueda parcial en select, actualización en update)
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Estatus de la refacción (por defecto true)
        /// </summary>
        public bool Estatus { get; set; } = true;
    }
}
