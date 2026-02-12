using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad TransferenciaSPEI
    /// </summary>
    public class TransferenciaSPEI
    {
        public int IdTransferencia { get; set; }
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
        public DateTime? Fecha { get; set; }
        public string? Referencia { get; set; }
    }
}
