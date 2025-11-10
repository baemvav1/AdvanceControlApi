using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AdvanceApi.Helpers.DbHelper>();
builder.Services.AddScoped<AdvanceApi.Services.ILoggingService, AdvanceApi.Services.LoggingService>();
builder.Services.AddScoped<AdvanceApi.Services.IClienteService, AdvanceApi.Services.ClienteService>();

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
