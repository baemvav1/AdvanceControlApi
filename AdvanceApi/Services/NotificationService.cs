using AdvanceApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Servicio de notificaciones en tiempo real usando SignalR.
    /// Permite enviar notificaciones push a todos los clientes conectados cuando ocurren cambios en la base de datos.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Notifica a todos los clientes conectados sobre un cambio en la base de datos.
        /// Utiliza el evento "DatabaseChanged" que los clientes deben escuchar.
        /// </summary>
        /// <param name="changeType">Tipo de cambio (INSERT, UPDATE, DELETE)</param>
        /// <param name="tableName">Nombre de la tabla afectada</param>
        /// <param name="data">Datos opcionales relacionados con el cambio</param>
        public async Task NotifyDatabaseChangeAsync(string changeType, string tableName, object? data = null)
        {
            try
            {
                // Sanitizar valores para prevenir log forging
                var sanitizedChangeType = SanitizeForLogging(changeType);
                var sanitizedTableName = SanitizeForLogging(tableName);

                var notification = new
                {
                    changeType = sanitizedChangeType,
                    tableName = sanitizedTableName,
                    timestamp = DateTime.UtcNow,
                    data
                };

                // Enviar la notificación a todos los clientes conectados
                await _hubContext.Clients.All.SendAsync("DatabaseChanged", notification);

                // Los valores sanitizados previenen log forging al eliminar caracteres de control
                _logger.LogInformation("Notificación enviada: {ChangeType} en tabla {TableName}", sanitizedChangeType, sanitizedTableName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación de cambio en base de datos");
                throw;
            }
        }

        /// <summary>
        /// Envía un mensaje personalizado a todos los clientes conectados.
        /// Utiliza el evento "ReceiveMessage" que los clientes deben escuchar.
        /// </summary>
        /// <param name="message">Mensaje a enviar</param>
        /// <param name="data">Datos opcionales a enviar con el mensaje</param>
        public async Task SendMessageToAllAsync(string message, object? data = null)
        {
            try
            {
                // Sanitizar mensaje para prevenir log forging
                var sanitizedMessage = SanitizeForLogging(message);

                var payload = new
                {
                    message = sanitizedMessage,
                    timestamp = DateTime.UtcNow,
                    data
                };

                // Enviar el mensaje a todos los clientes conectados
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", payload);

                // El mensaje sanitizado previene log forging al eliminar caracteres de control
                _logger.LogInformation("Mensaje enviado a todos los clientes: {Message}", sanitizedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar mensaje a los clientes");
                throw;
            }
        }

        /// <summary>
        /// Sanitiza una cadena para prevenir log forging eliminando caracteres de control.
        /// </summary>
        private static string SanitizeForLogging(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Eliminar caracteres de control que podrían usarse para log forging
            return new string(input.Where(c => !char.IsControl(c) || c == ' ').ToArray());
        }
    }
}
