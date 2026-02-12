namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros de consulta de TimbreFiscal
    /// </summary>
    public class TimbreFiscalQueryDto
    {
        /// <summary>
        /// ID del estado de cuenta (opcional)
        /// </summary>
        public int? IdEstadoCuenta { get; set; }

        /// <summary>
        /// UUID del timbre fiscal (opcional)
        /// </summary>
        public string? Uuid { get; set; }
    }
}
