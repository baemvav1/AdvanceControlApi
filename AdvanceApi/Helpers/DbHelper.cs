using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;

namespace AdvanceApi.Helpers
{
    /// <summary>
    /// Simple helper to obtain SQL connections and execute a few common stored-procedure operations
    /// needed by the AuthController (insert/get/revoke refresh tokens). You can extend this helper
    /// with logging, retry policies, or move SP-specific methods to a repository class if you prefer.
    /// </summary>
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            // Expect appsettings.json: ConnectionStrings:DefaultConnection (or change key as needed)
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("No se encontró la cadena de conexión 'DefaultConnection' en la configuración.");
        }

        /// <summary>
        /// Creates a new SqlConnection. Caller is responsible for OpenAsync/Dispose/Close.
        /// </summary>
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Creates a new SqlConnection and opens it asynchronously.
        /// </summary>
        public async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync();
                return connection;
            }
            catch (Exception)
            {
                connection.Dispose();
                throw;
            }
        }

        // Convenience wrapper methods for the refresh-token stored procedures used by AuthController.
        // These methods open their own connection, call the SP and return results.
        // The controller implementation provided earlier also uses direct SqlCommand with a connection,
        // so you can choose to use these wrappers or continue using raw SqlCommand as before.
        // I include them for convenience and to centralize parameter handling.

        /// <summary>
        /// Inserts a refresh token row and returns the generated Id (SCOPE_IDENTITY()).
        /// </summary>
        public async Task<long> InsertRefreshTokenAsync(string usuario, string tokenHash, DateTime expiresAt, string? ip = null, string? userAgent = null)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.InsertRefreshToken", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@Usuario", SqlDbType.NVarChar, 150) { Value = usuario });
            cmd.Parameters.Add(new SqlParameter("@TokenHash", SqlDbType.NVarChar, 200) { Value = tokenHash });
            cmd.Parameters.Add(new SqlParameter("@ExpiresAt", SqlDbType.DateTime2) { Value = expiresAt });
            cmd.Parameters.Add(new SqlParameter("@IpAddress", SqlDbType.NVarChar, 50) { Value = (object?)ip ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@UserAgent", SqlDbType.NVarChar, 1000) { Value = (object?)userAgent ?? DBNull.Value });

            await using var reader = await cmd.ExecuteReaderAsync();
            long newId = 0;
            if (await reader.ReadAsync())
            {
                var val = reader["NewId"];
                if (val != null && val != DBNull.Value)
                    newId = Convert.ToInt64(val);
            }

            await reader.CloseAsync();
            return newId;
        }

        /// <summary>
        /// Gets a refresh token record by tokenHash. Returns null if not found.
        /// </summary>
        public async Task<RefreshTokenRecord?> GetRefreshTokenByHashAsync(string tokenHash)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.GetRefreshTokenByHash", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@TokenHash", SqlDbType.NVarChar, 200) { Value = tokenHash });

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            var rec = new RefreshTokenRecord
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                Usuario = reader.GetString(reader.GetOrdinal("Usuario")),
                TokenHash = reader.GetString(reader.GetOrdinal("TokenHash")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                ExpiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt")),
                Revoked = reader.GetBoolean(reader.GetOrdinal("Revoked")),
                ReplacedByTokenHash = reader.IsDBNull(reader.GetOrdinal("ReplacedByTokenHash")) ? null : reader.GetString(reader.GetOrdinal("ReplacedByTokenHash")),
                IpAddress = reader.IsDBNull(reader.GetOrdinal("IpAddress")) ? null : reader.GetString(reader.GetOrdinal("IpAddress")),
                UserAgent = reader.IsDBNull(reader.GetOrdinal("UserAgent")) ? null : reader.GetString(reader.GetOrdinal("UserAgent"))
            };

            await reader.CloseAsync();
            return rec;
        }

        /// <summary>
        /// Revokes a refresh token by id and optionally sets ReplacedByTokenHash.
        /// </summary>
        public async Task RevokeRefreshTokenByIdAsync(long id, string? replacedByTokenHash = null)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.RevokeRefreshTokenById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.BigInt) { Value = id });
            cmd.Parameters.Add(new SqlParameter("@ReplacedByTokenHash", SqlDbType.NVarChar, 200) { Value = (object?)replacedByTokenHash ?? DBNull.Value });

            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Revokes all active refresh tokens for a given user.
        /// </summary>
        public async Task RevokeAllRefreshTokensForUserAsync(string usuario)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.RevokeAllRefreshTokensForUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@Usuario", SqlDbType.NVarChar, 150) { Value = usuario });

            await cmd.ExecuteNonQueryAsync();
        }

        // Optionally you may add wrappers for CountActiveRefreshTokensForUser, GetRefreshTokensForUser, and cleanup SPs.
        public async Task<int> CountActiveRefreshTokensForUserAsync(string usuario)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.CountActiveRefreshTokensForUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@Usuario", SqlDbType.NVarChar, 150) { Value = usuario });

            var scalar = await cmd.ExecuteScalarAsync();
            if (scalar == null || scalar == DBNull.Value) return 0;
            return Convert.ToInt32(scalar);
        }

        /// <summary>
        /// Lightweight record type returned by GetRefreshTokenByHashAsync.
        /// </summary>
        public class RefreshTokenRecord
        {
            public long Id { get; set; }
            public string Usuario { get; set; } = string.Empty;
            public string TokenHash { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
            public bool Revoked { get; set; }
            public string? ReplacedByTokenHash { get; set; }
            public string? IpAddress { get; set; }
            public string? UserAgent { get; set; }
        }
    }
}