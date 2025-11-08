namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de notificaciones en tiempo real.
    /// Permite enviar notificaciones a todos los clientes conectados.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Notifica a todos los clientes conectados sobre un cambio en la base de datos.
        /// </summary>
        /// <param name="changeType">Tipo de cambio (INSERT, UPDATE, DELETE)</param>
        /// <param name="tableName">Nombre de la tabla afectada</param>
        /// <param name="data">Datos opcionales relacionados con el cambio</param>
        Task NotifyDatabaseChangeAsync(string changeType, string tableName, object? data = null);

        /// <summary>
        /// Envía un mensaje personalizado a todos los clientes conectados.
        /// </summary>
        /// <param name="message">Mensaje a enviar</param>
        /// <param name="data">Datos opcionales a enviar con el mensaje</param>
        Task SendMessageToAllAsync(string message, object? data = null);
    }
}
