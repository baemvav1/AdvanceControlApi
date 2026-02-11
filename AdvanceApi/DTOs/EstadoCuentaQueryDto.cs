using System;

namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros del procedimiento almacenado sp_GestionEstadoCuenta
    /// </summary>
    public class EstadoCuentaQueryDto
    {
        /// <summary>
        /// Tipo de operación: 'Select', 'Create_Estado', 'Create_Deposito', 
        /// 'Select_Depositos', 'Select_Resumen', 'Verificar_Deposito', 'Buscar_Duplicados'
        /// </summary>
        public string Operacion { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de corte del estado de cuenta
        /// </summary>
        public DateTime? FechaCorte { get; set; }

        /// <summary>
        /// Inicio del periodo del estado de cuenta
        /// </summary>
        public DateTime? PeriodoDesde { get; set; }

        /// <summary>
        /// Fin del periodo del estado de cuenta
        /// </summary>
        public DateTime? PeriodoHasta { get; set; }

        /// <summary>
        /// Saldo inicial del periodo
        /// </summary>
        public decimal? SaldoInicial { get; set; }

        /// <summary>
        /// Saldo al corte
        /// </summary>
        public decimal? SaldoCorte { get; set; }

        /// <summary>
        /// Total de depósitos del periodo
        /// </summary>
        public decimal? TotalDepositos { get; set; }

        /// <summary>
        /// Total de retiros del periodo
        /// </summary>
        public decimal? TotalRetiros { get; set; }

        /// <summary>
        /// Comisiones del periodo
        /// </summary>
        public decimal? Comisiones { get; set; }

        /// <summary>
        /// Nombre del archivo del estado de cuenta
        /// </summary>
        public string? NombreArchivo { get; set; }

        /// <summary>
        /// ID del estado de cuenta (para operaciones que lo requieren)
        /// </summary>
        public int? EstadoCuentaID { get; set; }

        /// <summary>
        /// Fecha del depósito
        /// </summary>
        public DateTime? FechaDeposito { get; set; }

        /// <summary>
        /// Descripción del depósito
        /// </summary>
        public string? DescripcionDeposito { get; set; }

        /// <summary>
        /// Monto del depósito
        /// </summary>
        public decimal? MontoDeposito { get; set; }

        /// <summary>
        /// Tipo de depósito (ej: Transferencia, Efectivo, Cheque)
        /// </summary>
        public string? TipoDeposito { get; set; }
    }
}
