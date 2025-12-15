namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros del procedimiento almacenado sp_relacionOperacion_ProveedorRefaccion_edit
    /// </summary>
    public class RelacionOperacionProveedorRefaccionQueryDto
    {
        /// <summary>
        /// Tipo de operación: 'select', 'delete', 'put', 'update_nota'
        /// </summary>
        public string Operacion { get; set; } = string.Empty;

        /// <summary>
        /// ID de la relación operación-proveedor-refacción (para delete, update_nota)
        /// </summary>
        public int IdRelacionOperacionProveedorRefaccion { get; set; } = 0;

        /// <summary>
        /// ID de la operación (obligatorio para select y put)
        /// </summary>
        public int IdOperacion { get; set; } = 0;

        /// <summary>
        /// ID del proveedor refacción (obligatorio para put)
        /// </summary>
        public int IdProveedorRefaccion { get; set; } = 0;

        /// <summary>
        /// Precio de la refacción por el proveedor en la operación
        /// </summary>
        public float Precio { get; set; } = 0.0f;

        /// <summary>
        /// Nota asociada a la relación
        /// </summary>
        public string? Nota { get; set; }
    }
}
