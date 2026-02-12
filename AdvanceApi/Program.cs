using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el formato: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddScoped<AdvanceApi.Helpers.DbHelper>();
builder.Services.AddScoped<AdvanceApi.Services.ILoggingService, AdvanceApi.Services.LoggingService>();
builder.Services.AddScoped<AdvanceApi.Services.IClienteService, AdvanceApi.Services.ClienteService>();
builder.Services.AddScoped<AdvanceApi.Services.IOperacionService, AdvanceApi.Services.OperacionService>();
builder.Services.AddScoped<AdvanceApi.Services.IContactoUsuarioService, AdvanceApi.Services.ContactoUsuarioService>();
builder.Services.AddScoped<AdvanceApi.Services.IEquipoService, AdvanceApi.Services.EquipoService>();
builder.Services.AddScoped<AdvanceApi.Services.IRelacionEquipoClienteService, AdvanceApi.Services.RelacionEquipoClienteService>();
builder.Services.AddScoped<AdvanceApi.Services.IMantenimientoService, AdvanceApi.Services.MantenimientoService>();
builder.Services.AddScoped<AdvanceApi.Services.IRefaccionService, AdvanceApi.Services.RefaccionService>();
builder.Services.AddScoped<AdvanceApi.Services.IRelacionRefaccionEquipoService, AdvanceApi.Services.RelacionRefaccionEquipoService>();
builder.Services.AddScoped<AdvanceApi.Services.IProveedorService, AdvanceApi.Services.ProveedorService>();
builder.Services.AddScoped<AdvanceApi.Services.IRelacionProveedorRefaccionService, AdvanceApi.Services.RelacionProveedorRefaccionService>();
builder.Services.AddScoped<AdvanceApi.Services.IRelacionOperacionProveedorRefaccionService, AdvanceApi.Services.RelacionOperacionProveedorRefaccionService>();
builder.Services.AddScoped<AdvanceApi.Services.ICargoService, AdvanceApi.Services.CargoService>();
builder.Services.AddScoped<AdvanceApi.Services.IServicioService, AdvanceApi.Services.ServicioService>();
builder.Services.AddScoped<AdvanceApi.Services.IAreaService, AdvanceApi.Services.AreaService>();
builder.Services.AddScoped<AdvanceApi.Services.IUbicacionService, AdvanceApi.Services.UbicacionService>();
builder.Services.AddScoped<AdvanceApi.Services.IEntidadService, AdvanceApi.Services.EntidadService>();
builder.Services.AddScoped<AdvanceApi.Services.IContactoService, AdvanceApi.Services.ContactoService>();
builder.Services.AddScoped<AdvanceApi.Services.IEstadoCuentaService, AdvanceApi.Services.EstadoCuentaService>();
builder.Services.AddScoped<AdvanceApi.Services.IBancoCtaHabienteService, AdvanceApi.Services.BancoCtaHabienteService>();
builder.Services.AddScoped<AdvanceApi.Services.IMovimientoService, AdvanceApi.Services.MovimientoService>();
builder.Services.AddScoped<AdvanceApi.Services.ITransferenciaSPEIService, AdvanceApi.Services.TransferenciaSPEIService>();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("No se encontró Jwt:Key en la configuración.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new Exception("No se encontró Jwt:Issuer en la configuración.");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new Exception("No se encontró Jwt:Audience en la configuración.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
