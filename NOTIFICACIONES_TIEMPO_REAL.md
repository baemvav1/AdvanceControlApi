# Sistema de Notificaciones en Tiempo Real

## Resumen

Este documento explica cómo funciona el sistema de notificaciones en tiempo real implementado en AdvanceControlApi usando **SignalR**.

## ¿Por qué SignalR (Push) en vez de Polling?

Cuando se trata de notificar a los clientes sobre cambios en la base de datos, existen dos enfoques principales:

### 1. **Polling (Cliente pregunta periódicamente)**
- ❌ Los clientes hacen peticiones HTTP cada X segundos
- ❌ Genera tráfico innecesario cuando no hay cambios
- ❌ Mayor carga en el servidor
- ❌ Latencia en recibir notificaciones (depende del intervalo)
- ❌ Desperdicia recursos de red y CPU

### 2. **Push con SignalR (Servidor notifica)** ✅ **RECOMENDADO**
- ✅ Conexión persistente WebSocket (protocolo eficiente)
- ✅ El servidor envía notificaciones solo cuando hay cambios
- ✅ Notificaciones instantáneas (tiempo real)
- ✅ Menor carga en el servidor
- ✅ Mejor experiencia de usuario
- ✅ Estándar de la industria para aplicaciones en tiempo real

## Arquitectura

```
┌─────────────────┐         WebSocket          ┌─────────────────┐
│                 │ ◄──────────────────────────►│                 │
│  Cliente Web    │                             │  SignalR Hub    │
│  (JavaScript)   │                             │  (Servidor)     │
│                 │ ◄──────────────────────────►│                 │
└─────────────────┘    Notificaciones Push     └─────────────────┘
                                                         │
                                                         │
                                                         ▼
                                                ┌────────────────┐
                                                │  Base de Datos │
                                                │  (SQL Server)  │
                                                └────────────────┘
```

## Componentes Implementados

### 1. NotificationHub (`/Hubs/NotificationHub.cs`)
- Hub de SignalR donde los clientes se conectan
- Maneja conexiones y desconexiones de clientes
- Endpoint: `https://tu-api.com/notificationHub`

### 2. INotificationService y NotificationService (`/Services/`)
- Servicio para enviar notificaciones a todos los clientes conectados
- Métodos principales:
  - `NotifyDatabaseChangeAsync()`: Notifica cambios en la BD
  - `SendMessageToAllAsync()`: Envía mensajes personalizados

### 3. NotificationController (`/Controllers/NotificationController.cs`)
- Endpoints de prueba para demostrar el sistema
- `POST /api/Notification/test`: Simula una notificación de cambio en BD
- `POST /api/Notification/message`: Envía un mensaje personalizado

## Cómo Usar

### Lado del Servidor (ASP.NET Core)

#### 1. Inyectar el servicio en tus controladores o servicios

```csharp
public class MiController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public MiController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
}
```

#### 2. Notificar después de cambios en la base de datos

```csharp
[HttpPost]
public async Task<IActionResult> CrearUsuario([FromBody] Usuario usuario)
{
    // Insertar en la base de datos
    await _dbHelper.InsertUsuarioAsync(usuario);

    // Notificar a todos los clientes conectados
    await _notificationService.NotifyDatabaseChangeAsync(
        changeType: "INSERT",
        tableName: "usuarios",
        data: new { id = usuario.Id, nombre = usuario.Nombre }
    );

    return Ok(usuario);
}
```

#### 3. Ejemplo con UPDATE y DELETE

```csharp
// UPDATE
await _notificationService.NotifyDatabaseChangeAsync(
    "UPDATE",
    "usuarios",
    new { id = usuario.Id, camposModificados = new[] { "email", "telefono" } }
);

// DELETE
await _notificationService.NotifyDatabaseChangeAsync(
    "DELETE",
    "usuarios",
    new { id = usuario.Id }
);
```

### Lado del Cliente (JavaScript)

#### 1. Instalar la librería de SignalR

```bash
npm install @microsoft/signalr
```

O incluir desde CDN:
```html
<script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
```

#### 2. Conectarse al Hub y escuchar notificaciones

```javascript
// Crear conexión al hub
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://tu-api.com/notificationHub")
    .withAutomaticReconnect() // Reconexión automática
    .build();

// Escuchar el evento "DatabaseChanged"
connection.on("DatabaseChanged", (notification) => {
    console.log("Cambio en la base de datos:", notification);
    
    // notification contiene:
    // - changeType: "INSERT", "UPDATE", "DELETE"
    // - tableName: nombre de la tabla afectada
    // - timestamp: fecha/hora del cambio
    // - data: datos adicionales del cambio
    
    // Actualizar la UI según el tipo de cambio
    if (notification.tableName === "usuarios") {
        actualizarListaUsuarios();
    }
});

// Escuchar mensajes personalizados
connection.on("ReceiveMessage", (message) => {
    console.log("Mensaje recibido:", message.message);
    console.log("Datos:", message.data);
});

// Iniciar la conexión
connection.start()
    .then(() => {
        console.log("Conectado al hub de notificaciones");
    })
    .catch(err => {
        console.error("Error al conectar:", err);
    });
```

#### 3. Ejemplo completo en una aplicación web

```html
<!DOCTYPE html>
<html>
<head>
    <title>Notificaciones en Tiempo Real</title>
    <script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
</head>
<body>
    <h1>Notificaciones de la Base de Datos</h1>
    <div id="notifications"></div>

    <script>
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7000/notificationHub")
            .withAutomaticReconnect()
            .build();

        const notificationsDiv = document.getElementById("notifications");

        connection.on("DatabaseChanged", (notification) => {
            const message = `
                <div class="notification">
                    <strong>${notification.changeType}</strong> en 
                    <em>${notification.tableName}</em> - 
                    ${new Date(notification.timestamp).toLocaleString()}
                    <pre>${JSON.stringify(notification.data, null, 2)}</pre>
                </div>
            `;
            notificationsDiv.innerHTML += message;
        });

        connection.start()
            .then(() => console.log("✅ Conectado"))
            .catch(err => console.error("❌ Error:", err));
    </script>
</body>
</html>
```

## Probando el Sistema

### 1. Iniciar la aplicación
```bash
dotnet run --project AdvanceApi
```

### 2. Probar con Swagger o Postman

**Enviar una notificación de prueba:**
```http
POST https://localhost:7000/api/Notification/test
Content-Type: application/json

{
  "changeType": "INSERT",
  "tableName": "usuarios",
  "data": {
    "id": 123,
    "nombre": "Juan Pérez",
    "email": "juan@example.com"
  }
}
```

**Enviar un mensaje personalizado:**
```http
POST https://localhost:7000/api/Notification/message
Content-Type: application/json

{
  "message": "Nuevo usuario registrado en el sistema",
  "data": {
    "prioridad": "alta"
  }
}
```

### 3. Verificar que los clientes conectados reciben las notificaciones

Abre la consola del navegador y deberías ver los mensajes llegando en tiempo real.

## Integración con SQL Server (Opcional Avanzado)

Si deseas que las notificaciones se envíen automáticamente cuando ocurren cambios en la base de datos (sin modificar código), puedes usar:

### Opción 1: SQL Server Service Broker
- Configura triggers en las tablas
- El trigger envía un mensaje a Service Broker
- La aplicación escucha Service Broker y envía notificaciones SignalR

### Opción 2: SQL Server Change Tracking
- Habilita Change Tracking en las tablas
- Un servicio de fondo consulta periódicamente los cambios
- Envía notificaciones SignalR cuando detecta cambios

### Opción 3: SqlDependency (Legacy, no recomendado para producción)
- Usa SqlDependency para recibir notificaciones de SQL Server
- Requiere configuración especial del servidor

**Recomendación**: Para la mayoría de casos, es mejor llamar manualmente a `NotifyDatabaseChangeAsync()` después de cada operación de BD, como se muestra en este documento.

## Consideraciones de Seguridad

1. **Autenticación**: Considera agregar autenticación al hub para que solo usuarios autenticados se conecten
   ```csharp
   [Authorize]
   public class NotificationHub : Hub
   ```

2. **Filtrado de notificaciones**: Si no todos los usuarios deben recibir todas las notificaciones, implementa lógica de grupos
   ```csharp
   // Enviar solo a usuarios de un grupo específico
   await _hubContext.Clients.Group("administradores").SendAsync("DatabaseChanged", notification);
   ```

3. **Validación de datos**: Siempre valida los datos antes de enviarlos a los clientes

## Escalabilidad

Si tienes múltiples instancias del servidor (load balancing), necesitarás un backplane de SignalR:

- **Azure SignalR Service** (recomendado)
- **Redis** como backplane
- **SQL Server** como backplane

Ejemplo con Redis:
```csharp
builder.Services.AddSignalR()
    .AddStackExchangeRedis("localhost:6379");
```

## Ventajas del Sistema Implementado

1. ✅ **Tiempo real**: Las notificaciones llegan instantáneamente
2. ✅ **Eficiente**: Solo se envían datos cuando hay cambios
3. ✅ **Escalable**: SignalR soporta miles de conexiones concurrentes
4. ✅ **Confiable**: Reconexión automática si se pierde la conexión
5. ✅ **Compatible**: Funciona en todos los navegadores modernos
6. ✅ **Fácil de usar**: API simple y directa
7. ✅ **Mantenible**: Código limpio y bien estructurado

## Conclusión

Este sistema de notificaciones push con SignalR es la **mejor práctica** para notificar cambios en la base de datos a clientes en tiempo real. Es superior al polling en todos los aspectos: eficiencia, latencia, escalabilidad y experiencia de usuario.

Para cualquier pregunta o mejora, consulta la documentación oficial de SignalR:
https://docs.microsoft.com/aspnet/core/signalr/introduction
