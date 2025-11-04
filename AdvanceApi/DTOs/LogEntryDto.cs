using System.ComponentModel.DataAnnotations;

namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para recibir entradas de log desde clientes
    /// </summary>
    public class LogEntryDto
    {
        /// <summary>
        /// ID único del log (opcional, se genera automáticamente si no se proporciona)
        /// </summary>
        [StringLength(50)]
        public string? Id { get; set; }

        /// <summary>
        /// Nivel de log: 0=Trace, 1=Debug, 2=Info, 3=Warning, 4=Error, 5=Critical
        /// </summary>
        [Required(ErrorMessage = "Level is required")]
        [Range(0, 5, ErrorMessage = "Level must be between 0 and 5")]
        public int Level { get; set; }

        /// <summary>
        /// Mensaje de log
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Información de excepción si aplica
        /// </summary>
        public string? Exception { get; set; }

        /// <summary>
        /// StackTrace de la excepción
        /// </summary>
        public string? StackTrace { get; set; }

        /// <summary>
        /// Fuente que generó el log
        /// </summary>
        [StringLength(255)]
        public string? Source { get; set; }

        /// <summary>
        /// Método que generó el log
        /// </summary>
        [StringLength(255)]
        public string? Method { get; set; }

        /// <summary>
        /// Usuario que ejecutó la operación
        /// </summary>
        [StringLength(255)]
        public string? Username { get; set; }

        /// <summary>
        /// Nombre de la máquina donde se generó el log
        /// </summary>
        [StringLength(255)]
        public string? MachineName { get; set; }

        /// <summary>
        /// Versión de la aplicación
        /// </summary>
        [StringLength(50)]
        public string? AppVersion { get; set; }

        /// <summary>
        /// Timestamp del log (opcional, se usa UTC actual si no se proporciona)
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Datos adicionales en formato JSON u otro formato
        /// </summary>
        public string? AdditionalData { get; set; }
    }
}
