var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AdvanceApi.Helpers.DbHelper>();
builder.Services.AddScoped<AdvanceApi.Services.ILoggingService, AdvanceApi.Services.LoggingService>();
builder.Services.AddScoped<AdvanceApi.Services.INotificationService, AdvanceApi.Services.NotificationService>();

// Configurar SignalR para notificaciones en tiempo real
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Mapear el hub de SignalR
app.MapHub<AdvanceApi.Hubs.NotificationHub>("/notificationHub");

app.Run();
