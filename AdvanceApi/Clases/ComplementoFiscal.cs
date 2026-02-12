using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad ComplementoFiscal
    /// Representa un complemento fiscal asociado a un estado de cuenta
    /// </summary>
    public class ComplementoFiscal
    {
        public int IdComplemento { get; set; }
        public int IdEstadoCuenta { get; set; }
        public string? Rfc { get; set; }
        public string? FormaPago { get; set; }
        public string? MetodoPago { get; set; }
        public string? UsoCFDI { get; set; }
        public string? ClaveProducto { get; set; }
        public string? CodigoPostal { get; set; }
        public string? NumeroCuenta { get; set; }
        public DateTime? FechaCorte { get; set; }
    }
}
