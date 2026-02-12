using System;

namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para consultar transferencias SPEI
    /// </summary>
    public class TransferenciaSPEIQueryDto
    {
        public int? IdMovimiento { get; set; }
        public string? TipoTransferencia { get; set; }
        public string? ClaveRastreo { get; set; }
        public string? RfcEmisor { get; set; }
        public string? RfcDestinatario { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
