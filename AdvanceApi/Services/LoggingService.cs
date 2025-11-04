using AdvanceApi.DTOs;
using AdvanceApi.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Implementación del servicio de logging que usa el procedimiento almacenado sp_InsertLog
    /// </summary>
    public class LoggingService : ILoggingService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(DbHelper dbHelper, ILogger<LoggingService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Registra una entrada de log en la base de datos usando el SP sp_InsertLog
        /// </summary>
        public async Task<(string LogId, long? AlertId)> LogAsync(LogEntryDto logEntry)
        {
            if (logEntry == null)
                throw new ArgumentNullException(nameof(logEntry));

            // Generar ID si no se proporcionó
            var logId = logEntry.Id ?? Guid.NewGuid().ToString();

            // Usar timestamp actual UTC si no se proporcionó
            var timestamp = logEntry.Timestamp ?? DateTime.UtcNow;

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_InsertLog", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@Id", logId);
                command.Parameters.AddWithValue("@Level", logEntry.Level);
                command.Parameters.AddWithValue("@Message", logEntry.Message ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Exception", logEntry.Exception ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@StackTrace", logEntry.StackTrace ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Source", logEntry.Source ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Method", logEntry.Method ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Username", logEntry.Username ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@MachineName", logEntry.MachineName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AppVersion", logEntry.AppVersion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Timestamp", timestamp);
                command.Parameters.AddWithValue("@AdditionalData", logEntry.AdditionalData ?? (object)DBNull.Value);

                // El SP devuelve LogId y AlertId en un SELECT
                long? alertId = null;
                await using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    // Leer opcionalmente el AlertId si está presente
                    if (reader.FieldCount > 1 && !reader.IsDBNull(1))
                    {
                        alertId = reader.GetInt64(1);
                    }
                }

                _logger.LogDebug("Log insertado exitosamente. LogId: {LogId}, AlertId: {AlertId}", logId, alertId);
                
                return (logId, alertId);
            }
            catch (SqlException sqlEx)
            {
                // Log detallado del error SQL
                _logger.LogError(sqlEx, "Error SQL al insertar log. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al insertar el log en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al insertar log");
                throw;
            }
        }
    }
}
