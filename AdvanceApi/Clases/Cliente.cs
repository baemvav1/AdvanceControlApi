using System;

namespace Clases
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string? Rfc { get; set; }
        public string? RazonSocial { get; set; }
        public string? NombreComercial { get; set; }
        public string? RegimenFiscal { get; set; }
        public string? UsoCfdi { get; set; }
        public int? DiasCredito { get; set; }
        public decimal? LimiteCredito { get; set; }
        public int? Prioridad { get; set; }
        public bool? Estatus { get; set; }
        public int? CredencialId { get; set; }
        public string? Notas { get; set; }
        public DateTime? CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public int? IdUsuarioCreador { get; set; }
        public int? IdUsuarioAct { get; set; }
    }
}
