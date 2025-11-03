// AdvanceApi/Helpers/DbHelper.cs (versión mejorada)
using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AdvanceApi.Helpers
{
    public class DbHelper
    {
        private readonly string _connectionString;
        private readonly ILogger<DbHelper>? _logger;

        public DbHelper(IConfiguration configuration, ILogger<DbHelper>? logger = null)
        {
            _logger = logger;
            // Leer configuración primero; permitir override con variable de entorno estándar
            var csFromConfig = configuration.GetConnectionString("DefaultConnection");
            var csFromEnv = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                            ?? Environment.GetEnvironmentVariable("DefaultConnection");

            _connectionString = !string.IsNullOrWhiteSpace(csFromEnv) ? csFromEnv : csFromConfig ?? string.Empty;

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                var msg = "Connection string 'DefaultConnection' not found. Set it in appsettings or in the environment variable 'ConnectionStrings__DefaultConnection'.";
                _logger?.LogError(msg);
                throw new InvalidOperationException(msg);
            }
        }

        // Devuelve conexión cerrada — el caller debe abrirla y disponerla.
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Conveniencia: devuelve conexión ya abierta (asincrónico)
        public async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            return conn; // caller debe Dispose()
        }
    }
}