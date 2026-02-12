namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros de consulta de ComplementoFiscal
    /// </summary>
    public class ComplementoFiscalQueryDto
    {
        /// <summary>
        /// ID del estado de cuenta (opcional)
        /// </summary>
        public int? IdEstadoCuenta { get; set; }

        /// <summary>
        /// RFC del contribuyente (opcional)
        /// </summary>
        public string? Rfc { get; set; }
    }
}
