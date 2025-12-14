namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros del procedimiento almacenado sp_proveedor_edit
    /// </summary>
    public class ProveedorQueryDto
    {
        /// <summary>
        /// Tipo de operación: 'select', 'delete', 'update', 'create'
        /// </summary>
        public string Operacion { get; set; } = string.Empty;

        /// <summary>
        /// ID del proveedor (requerido para delete y update)
        /// </summary>
        public int IdProveedor { get; set; } = 0;

        /// <summary>
        /// RFC del proveedor
        /// </summary>
        public string? Rfc { get; set; }

        /// <summary>
        /// Razón social del proveedor (búsqueda parcial en select, actualización en update)
        /// </summary>
        public string? RazonSocial { get; set; }

        /// <summary>
        /// Nombre comercial del proveedor (búsqueda parcial en select, actualización en update)
        /// </summary>
        public string? NombreComercial { get; set; }

        /// <summary>
        /// Estatus del proveedor (por defecto true)
        /// </summary>
        public bool Estatus { get; set; } = true;

        /// <summary>
        /// Nota o comentario del proveedor
        /// </summary>
        public string? Nota { get; set; }
    }
}
