# Ejemplo de Integración del Sistema de Notificaciones

Este documento muestra cómo integrar el sistema de notificaciones en tiempo real con tus controladores y servicios existentes.

## Escenario Real: Notificar cuando un usuario inicia sesión

Cuando un usuario hace login exitosamente, podemos notificar a todos los clientes conectados.

### Modificar AuthController (Ejemplo)

```csharp
public class AuthController : ControllerBase
{
    private readonly DbHelper _dbHelper;
    private readonly INotificationService _notificationService; // ← Agregar

    public AuthController(
        DbHelper dbHelper, 
        IConfiguration configuration,
        INotificationService notificationService) // ← Agregar
    {
        _dbHelper = dbHelper;
        _notificationService = notificationService; // ← Agregar
        // ... resto del código
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] usuario request)
    {
        // ... código existente de autenticación ...

        if (!isAuthorized)
            return Unauthorized(new { message = "Credenciales inválidas." });

        // Generar tokens
        var accessToken = GenerateJwtToken(request.Usuario, out DateTime accessExpiry);
        var refreshPlain = GenerateRefreshTokenPlain();
        // ... código existente ...

        // ✅ NOTIFICAR A TODOS LOS CLIENTES
        await _notificationService.NotifyDatabaseChangeAsync(
            changeType: "INSERT",
            tableName: "sessions",
            data: new 
            { 
                usuario = request.Usuario,
                timestamp = DateTime.UtcNow,
                action = "user_logged_in"
            }
        );

        return Ok(new { accessToken, refreshToken = refreshPlain, /* ... */ });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest body)
    {
        // ... código existente de logout ...

        // ✅ NOTIFICAR CIERRE DE SESIÓN
        if (record != null)
        {
            await _notificationService.NotifyDatabaseChangeAsync(
                changeType: "DELETE",
                tableName: "sessions",
                data: new 
                { 
                    usuario = record.Usuario,
                    timestamp = DateTime.UtcNow,
                    action = "user_logged_out"
                }
            );
        }

        return NoContent();
    }
}
```

## Crear un Controlador para Gestión de Usuarios (Ejemplo Completo)

```csharp
using AdvanceApi.Helpers;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly DbHelper _dbHelper;
        private readonly INotificationService _notificationService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            DbHelper dbHelper,
            INotificationService notificationService,
            ILogger<UsuariosController> logger)
        {
            _dbHelper = dbHelper;
            _notificationService = notificationService;
            _logger = logger;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDto dto)
        {
            try
            {
                await using var conn = _dbHelper.GetConnection();
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("CrearUsuario", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@usuario", SqlDbType.NVarChar, 150) 
                    { Value = dto.Usuario });
                cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar, 200) 
                    { Value = dto.Email });
                
                var userId = await cmd.ExecuteScalarAsync();

                // ✅ NOTIFICAR CREACIÓN
                await _notificationService.NotifyDatabaseChangeAsync(
                    changeType: "INSERT",
                    tableName: "usuarios",
                    data: new 
                    { 
                        id = userId,
                        usuario = dto.Usuario,
                        email = dto.Email
                    }
                );

                return Ok(new { id = userId, message = "Usuario creado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, new { message = "Error al crear usuario" });
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] ActualizarUsuarioDto dto)
        {
            try
            {
                await using var conn = _dbHelper.GetConnection();
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("ActualizarUsuario", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
                cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar, 200) 
                    { Value = dto.Email });
                
                await cmd.ExecuteNonQueryAsync();

                // ✅ NOTIFICAR ACTUALIZACIÓN
                await _notificationService.NotifyDatabaseChangeAsync(
                    changeType: "UPDATE",
                    tableName: "usuarios",
                    data: new 
                    { 
                        id = id,
                        camposModificados = new[] { "email" },
                        nuevoEmail = dto.Email
                    }
                );

                return Ok(new { message = "Usuario actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                return StatusCode(500, new { message = "Error al actualizar usuario" });
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            try
            {
                await using var conn = _dbHelper.GetConnection();
                await conn.OpenAsync();

                // Primero obtener el usuario para saber su nombre
                await using var cmdGet = new SqlCommand("SELECT Usuario FROM Usuarios WHERE Id = @id", conn);
                cmdGet.Parameters.AddWithValue("@id", id);
                var nombreUsuario = await cmdGet.ExecuteScalarAsync() as string;

                // Eliminar usuario
                await using var cmd = new SqlCommand("EliminarUsuario", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
                await cmd.ExecuteNonQueryAsync();

                // ✅ NOTIFICAR ELIMINACIÓN
                await _notificationService.NotifyDatabaseChangeAsync(
                    changeType: "DELETE",
                    tableName: "usuarios",
                    data: new 
                    { 
                        id = id,
                        usuario = nombreUsuario
                    }
                );

                return Ok(new { message = "Usuario eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario");
                return StatusCode(500, new { message = "Error al eliminar usuario" });
            }
        }

        public class CrearUsuarioDto
        {
            public string Usuario { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        public class ActualizarUsuarioDto
        {
            public string Email { get; set; } = string.Empty;
        }
    }
}
```

## Notificaciones desde Servicios de Fondo (Background Services)

Si tienes procesos en segundo plano que modifican la base de datos:

```csharp
public class ProcesadorPedidosService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProcesadorPedidosService> _logger;

    public ProcesadorPedidosService(
        IServiceProvider serviceProvider,
        ILogger<ProcesadorPedidosService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider
                .GetRequiredService<INotificationService>();

            // Procesar pedidos pendientes
            var pedidosProcesados = await ProcesarPedidosPendientes();

            if (pedidosProcesados > 0)
            {
                // ✅ NOTIFICAR PROCESAMIENTO
                await notificationService.SendMessageToAllAsync(
                    message: $"{pedidosProcesados} pedidos procesados",
                    data: new 
                    { 
                        cantidad = pedidosProcesados,
                        timestamp = DateTime.UtcNow
                    }
                );
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task<int> ProcesarPedidosPendientes()
    {
        // Lógica de procesamiento
        return 0;
    }
}
```

## Filtrar Notificaciones por Usuario o Grupo

Si solo ciertos usuarios deben recibir ciertas notificaciones:

```csharp
public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    // Enviar solo a usuarios específicos
    public async Task NotifySpecificUsersAsync(
        string[] connectionIds, 
        string message, 
        object? data = null)
    {
        await _hubContext.Clients.Clients(connectionIds)
            .SendAsync("ReceiveMessage", new { message, data });
    }

    // Enviar solo a un grupo específico
    public async Task NotifyGroupAsync(
        string groupName, 
        string message, 
        object? data = null)
    {
        await _hubContext.Clients.Group(groupName)
            .SendAsync("ReceiveMessage", new { message, data });
    }
}
```

Y modificar el Hub para agregar usuarios a grupos:

```csharp
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Obtener el rol del usuario desde el token JWT
        var userRole = Context.User?.FindFirst("role")?.Value;
        
        if (!string.IsNullOrEmpty(userRole))
        {
            // Agregar al grupo según su rol
            await Groups.AddToGroupAsync(Context.ConnectionId, userRole);
            Console.WriteLine($"Usuario agregado al grupo: {userRole}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userRole = Context.User?.FindFirst("role")?.Value;
        
        if (!string.IsNullOrEmpty(userRole))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userRole);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
```

## Mejores Prácticas

1. **Siempre enviar notificaciones después de confirmar cambios en BD**
   ```csharp
   // ❌ INCORRECTO
   await _notificationService.NotifyDatabaseChangeAsync(...);
   await _dbHelper.InsertUsuarioAsync(usuario);

   // ✅ CORRECTO
   await _dbHelper.InsertUsuarioAsync(usuario);
   await _notificationService.NotifyDatabaseChangeAsync(...);
   ```

2. **Manejar errores en notificaciones sin afectar la lógica principal**
   ```csharp
   try
   {
       await _notificationService.NotifyDatabaseChangeAsync(...);
   }
   catch (Exception ex)
   {
       // Log pero no fallar la operación
       _logger.LogWarning(ex, "No se pudo enviar notificación");
   }
   ```

3. **No enviar datos sensibles en las notificaciones**
   ```csharp
   // ❌ NO enviar contraseñas, tokens, etc.
   await _notificationService.NotifyDatabaseChangeAsync(
       "INSERT", "usuarios",
       new { password = usuario.Password } // ¡MAL!
   );

   // ✅ Solo información básica y no sensible
   await _notificationService.NotifyDatabaseChangeAsync(
       "INSERT", "usuarios",
       new { id = usuario.Id, nombre = usuario.Nombre }
   );
   ```

4. **Ser específico en los mensajes**
   ```csharp
   // ❌ Mensaje vago
   await _notificationService.SendMessageToAllAsync("Cambio");

   // ✅ Mensaje descriptivo
   await _notificationService.SendMessageToAllAsync(
       "Nuevo usuario registrado en el sistema",
       new { usuarioId = 123, nombreUsuario = "Juan" }
   );
   ```

## Patrón Recomendado: Método Helper

Crea un método helper en tus controladores para simplificar:

```csharp
public class BaseApiController : ControllerBase
{
    protected readonly INotificationService _notificationService;

    protected BaseApiController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    protected async Task NotificarCambioAsync(
        string changeType, 
        string tableName, 
        object? data = null)
    {
        try
        {
            await _notificationService.NotifyDatabaseChangeAsync(
                changeType, tableName, data);
        }
        catch (Exception ex)
        {
            // Log silenciosamente, no afectar el flujo principal
            Console.WriteLine($"Error al notificar: {ex.Message}");
        }
    }
}

// Usar en tus controladores
public class UsuariosController : BaseApiController
{
    public UsuariosController(INotificationService notificationService)
        : base(notificationService)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Usuario usuario)
    {
        // Lógica de creación
        await _dbHelper.InsertUsuarioAsync(usuario);

        // Notificar de forma simple
        await NotificarCambioAsync("INSERT", "usuarios", 
            new { id = usuario.Id, nombre = usuario.Nombre });

        return Ok(usuario);
    }
}
```

## Conclusión

El sistema de notificaciones es fácil de integrar en cualquier parte de tu aplicación:
- Controladores
- Servicios
- Background Services
- Middleware

Solo recuerda:
1. Inyectar `INotificationService`
2. Llamar al método apropiado después de cambios en BD
3. Manejar errores apropiadamente
4. No enviar datos sensibles
