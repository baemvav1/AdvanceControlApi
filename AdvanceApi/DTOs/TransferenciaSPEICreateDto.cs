using System;

namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para crear una transferencia SPEI
    /// </summary>
    public class TransferenciaSPEICreateDto
    {
        public int IdMovimiento { get; set; }
        public string TipoTransferencia { get; set; } = string.Empty;
        public string? BancoClave { get; set; }
        public string? BancoNombre { get; set; }
        public string? CuentaOrigen { get; set; }
        public string? CuentaDestino { get; set; }
        public string? NombreEmisor { get; set; }
        public string? NombreDestinatario { get; set; }
        public string? RfcEmisor { get; set; }
        public string? RfcDestinatario { get; set; }
        public string? ClaveRastreo { get; set; }
        public string? Concepto { get; set; }
        public TimeSpan? Hora { get; set; }
        public decimal Monto { get; set; }
    }
}
