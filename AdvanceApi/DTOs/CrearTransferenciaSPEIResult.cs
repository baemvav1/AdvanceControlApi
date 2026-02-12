namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para el resultado de crear una transferencia SPEI
    /// </summary>
    public class CrearTransferenciaSPEIResult
    {
        public bool Success { get; set; }
        public int IdTransferencia { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
