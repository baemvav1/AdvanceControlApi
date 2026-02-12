namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros de los procedimientos almacenados de EstadoCuenta
    /// </summary>
    public class EstadoCuentaQueryDto
    {
        /// <summary>
        /// ID del estado de cuenta
        /// </summary>
        public int IdEstadoCuenta { get; set; }

        /// <summary>
        /// Número de cuenta
        /// </summary>
        public string? NumeroCuenta { get; set; }

        /// <summary>
        /// CLABE interbancaria
        /// </summary>
        public string? Clabe { get; set; }

        /// <summary>
        /// Tipo de cuenta
        /// </summary>
        public string? TipoCuenta { get; set; }

        /// <summary>
        /// Tipo de moneda (default: MXN)
        /// </summary>
        public string? TipoMoneda { get; set; } = "MXN";

        /// <summary>
        /// Fecha de inicio del período
        /// </summary>
        public DateTime? FechaInicio { get; set; }

        /// <summary>
        /// Fecha de fin del período
        /// </summary>
        public DateTime? FechaFin { get; set; }

        /// <summary>
        /// Fecha de corte
        /// </summary>
        public DateTime? FechaCorte { get; set; }

        /// <summary>
        /// Saldo inicial
        /// </summary>
        public decimal? SaldoInicial { get; set; }

        /// <summary>
        /// Total de cargos
        /// </summary>
        public decimal? TotalCargos { get; set; }

        /// <summary>
        /// Total de abonos
        /// </summary>
        public decimal? TotalAbonos { get; set; }

        /// <summary>
        /// Saldo final
        /// </summary>
        public decimal? SaldoFinal { get; set; }

        /// <summary>
        /// Total de comisiones
        /// </summary>
        public decimal? TotalComisiones { get; set; }

        /// <summary>
        /// Total de ISR
        /// </summary>
        public decimal? TotalISR { get; set; }

        /// <summary>
        /// Total de IVA
        /// </summary>
        public decimal? TotalIVA { get; set; }
    }
}
