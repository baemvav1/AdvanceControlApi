using AdvanceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    /// <summary>
    /// Controlador para demostrar el uso del servicio de notificaciones en tiempo real.
    /// En un escenario real, llamarías a NotifyDatabaseChangeAsync después de realizar cambios en la BD.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint de prueba para simular una notificación de cambio en la base de datos.
        /// POST /api/Notification/test
        /// Body: { "changeType": "INSERT", "tableName": "usuarios", "data": { "id": 1, "nombre": "Juan" } }
        /// </summary>
        [HttpPost("test")]
        public async Task<IActionResult> TestNotification([FromBody] TestNotificationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.ChangeType) || string.IsNullOrWhiteSpace(request.TableName))
                {
                    return BadRequest(new { message = "changeType y tableName son requeridos." });
                }

                await _notificationService.NotifyDatabaseChangeAsync(
                    request.ChangeType,
                    request.TableName,
                    request.Data
                );

                return Ok(new
                {
                    message = "Notificación enviada exitosamente a todos los clientes conectados.",
                    changeType = request.ChangeType,
                    tableName = request.TableName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación de prueba");
                return StatusCode(500, new { message = "Error al enviar la notificación." });
            }
        }

        /// <summary>
        /// Endpoint de prueba para enviar un mensaje personalizado a todos los clientes.
        /// POST /api/Notification/message
        /// Body: { "message": "Hola a todos", "data": { "info": "adicional" } }
        /// </summary>
        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new { message = "message es requerido." });
                }

                await _notificationService.SendMessageToAllAsync(request.Message, request.Data);

                return Ok(new
                {
                    message = "Mensaje enviado exitosamente a todos los clientes conectados.",
                    sentMessage = request.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar mensaje");
                return StatusCode(500, new { message = "Error al enviar el mensaje." });
            }
        }

        public class TestNotificationRequest
        {
            public string ChangeType { get; set; } = string.Empty;
            public string TableName { get; set; } = string.Empty;
            public object? Data { get; set; }
        }

        public class SendMessageRequest
        {
            public string Message { get; set; } = string.Empty;
            public object? Data { get; set; }
        }
    }
}
