namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros de creación de ComplementoFiscal
    /// </summary>
    public class ComplementoFiscalCreateDto
    {
        /// <summary>
        /// ID del estado de cuenta
        /// </summary>
        public int IdEstadoCuenta { get; set; }

        /// <summary>
        /// RFC del contribuyente
        /// </summary>
        public string? Rfc { get; set; }

        /// <summary>
        /// Forma de pago (opcional)
        /// </summary>
        public string? FormaPago { get; set; }

        /// <summary>
        /// Método de pago (opcional)
        /// </summary>
        public string? MetodoPago { get; set; }

        /// <summary>
        /// Uso de CFDI (opcional)
        /// </summary>
        public string? UsoCFDI { get; set; }

        /// <summary>
        /// Clave del producto (opcional)
        /// </summary>
        public string? ClaveProducto { get; set; }

        /// <summary>
        /// Código postal (opcional)
        /// </summary>
        public string? CodigoPostal { get; set; }
    }
}
