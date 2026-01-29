namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para la respuesta de la operación select_by_refaccion
    /// Representa un proveedor que tiene una refacción específica con su precio
    /// </summary>
    public class ProveedorPorRefaccion
    {
        /// <summary>
        /// ID del proveedor
        /// </summary>
        public int? IdProveedor { get; set; }

        /// <summary>
        /// Nombre comercial del proveedor
        /// </summary>
        public string? NombreComercial { get; set; }

        /// <summary>
        /// Costo/Precio de la refacción para este proveedor
        /// </summary>
        public double? Costo { get; set; }
    }
}
