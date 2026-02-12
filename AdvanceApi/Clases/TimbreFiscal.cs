using System;

namespace Clases
{
    /// <summary>
    /// Modelo de la entidad TimbreFiscal
    /// Representa un timbre fiscal asociado a un estado de cuenta
    /// </summary>
    public class TimbreFiscal
    {
        public int IdTimbre { get; set; }
        public int IdEstadoCuenta { get; set; }
        public string? Uuid { get; set; }
        public DateTime? FechaTimbrado { get; set; }
        public string? NumeroProveedor { get; set; }
        public string? NumeroCuenta { get; set; }
        public DateTime? FechaCorte { get; set; }
    }
}
