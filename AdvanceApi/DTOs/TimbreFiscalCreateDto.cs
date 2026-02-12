using System;

namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros de creación de TimbreFiscal
    /// </summary>
    public class TimbreFiscalCreateDto
    {
        /// <summary>
        /// ID del estado de cuenta
        /// </summary>
        public int IdEstadoCuenta { get; set; }

        /// <summary>
        /// UUID del timbre fiscal
        /// </summary>
        public string? Uuid { get; set; }

        /// <summary>
        /// Fecha de timbrado
        /// </summary>
        public DateTime FechaTimbrado { get; set; }

        /// <summary>
        /// Número de proveedor (opcional)
        /// </summary>
        public string? NumeroProveedor { get; set; }
    }
}
