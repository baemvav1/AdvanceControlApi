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
                var notification = new
                {
                    changeType,
                    tableName,
                    timestamp = DateTime.UtcNow,
                    data
                };

                // Enviar la notificación a todos los clientes conectados
                await _hubContext.Clients.All.SendAsync("DatabaseChanged", notification);

                _logger.LogInformation("Notificación enviada: {ChangeType} en tabla {TableName}", changeType, tableName);
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
                var payload = new
                {
                    message,
                    timestamp = DateTime.UtcNow,
                    data
                };

                // Enviar el mensaje a todos los clientes conectados
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", payload);

                _logger.LogInformation("Mensaje enviado a todos los clientes: {Message}", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar mensaje a los clientes");
                throw;
            }
        }
    }
}
