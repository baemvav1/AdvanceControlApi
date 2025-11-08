using Microsoft.AspNetCore.SignalR;

namespace AdvanceApi.Hubs
{
    /// <summary>
    /// SignalR Hub para manejar notificaciones en tiempo real a los clientes conectados.
    /// Los clientes se conectan a este hub para recibir notificaciones de cambios en la base de datos.
    /// </summary>
    public class NotificationHub : Hub
    {
        /// <summary>
        /// Método llamado cuando un cliente se conecta al hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            // Opcionalmente, puedes notificar que un cliente se conectó
            Console.WriteLine($"Cliente conectado: {Context.ConnectionId}");
        }

        /// <summary>
        /// Método llamado cuando un cliente se desconecta del hub.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Cliente desconectado: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
