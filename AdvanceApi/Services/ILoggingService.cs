using AdvanceApi.DTOs;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Servicio para gestionar el registro de logs en la base de datos
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Registra una entrada de log en la base de datos
        /// </summary>
        /// <param name="logEntry">Entrada de log a registrar</param>
        /// <returns>Tupla con el ID del log insertado y el ID de alerta (si aplica)</returns>
        Task<(string LogId, long? AlertId)> LogAsync(LogEntryDto logEntry);
    }
}
