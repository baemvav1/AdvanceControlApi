using AdvanceApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Clases;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly DbHelper _dbHelper;
        private readonly int _accessTokenMinutes;
        private readonly int _refreshTokenDays;
        private readonly string _refreshSecret;

        public AuthController(DbHelper dbHelper, IConfiguration configuration)
        {
            _dbHelper = dbHelper;
            _secretKey = configuration["Jwt:Key"] ?? throw new Exception("No se encontró Jwt:Key en la configuración.");
            _issuer = configuration["Jwt:Issuer"] ?? throw new Exception("No se encontró Jwt:Issuer en la configuración.");
            _audience = configuration["Jwt:Audience"] ?? throw new Exception("No se encontró Jwt:Audience en la configuración.");

            _accessTokenMinutes = int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
            _refreshTokenDays = int.TryParse(configuration["RefreshToken:Days"], out var d) ? d : 30;
            _refreshSecret = configuration["RefreshToken:Secret"] ?? throw new Exception("No se encontró RefreshToken:Secret en la configuración.");
        }

        // Modelo esperado (ajusta a tu clase real si tiene otro nombre/propiedades)
        // public class usuario { public string Usuario { get; set; } = string.Empty; public string Pass { get; set; } = string.Empty; }

        // -----------------------
        // LOGIN
        // POST /api/Auth/login
        // Respuesta: { accessToken, refreshToken, expiresIn, tokenType, user }
        // -----------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] usuario request)
        {
            if (request == null)
                return BadRequest(new { message = "Request body es requerido." });

            if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Pass))
                return BadRequest(new { message = "Usuario y contraseña son requeridos." });

            try
            {
                await using var conn = _dbHelper.GetConnection();
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("login_credencial", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@usuario", SqlDbType.NVarChar, 150) { Value = request.Usuario });
                cmd.Parameters.Add(new SqlParameter("@contraseña", SqlDbType.NVarChar, 100) { Value = request.Pass });

                var scalar = await cmd.ExecuteScalarAsync();

                if (scalar == null || scalar == DBNull.Value)
                    return Unauthorized(new { message = "Credenciales inválidas." });

                bool isAuthorized;
                if (scalar is bool b) isAuthorized = b;
                else if (scalar is int i) isAuthorized = i != 0;
                else if (bool.TryParse(scalar.ToString(), out var parsedBool)) isAuthorized = parsedBool;
                else if (int.TryParse(scalar.ToString(), out var parsedInt)) isAuthorized = parsedInt != 0;
                else isAuthorized = false;

                if (!isAuthorized)
                    return Unauthorized(new { message = "Credenciales inválidas." });

                // Generar access token
                var accessToken = GenerateJwtToken(request.Usuario, out DateTime accessExpiry);
                var expiresIn = (int)(accessExpiry - DateTime.UtcNow).TotalSeconds;

                // Generar refresh token (plano), hashearlo con HMAC, guardar hash en DB via DbHelper.InsertRefreshTokenAsync
                var refreshPlain = GenerateRefreshTokenPlain();
                var refreshHash = ComputeHmacSha256(refreshPlain);
                var refreshExpiry = DateTime.UtcNow.AddDays(_refreshTokenDays);

                // Use DbHelper wrapper (opens its own connection)
                await _dbHelper.InsertRefreshTokenAsync(request.Usuario, refreshHash, refreshExpiry, Request.HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"].ToString());

                return Ok(new
                {
                    accessToken,
                    refreshToken = refreshPlain,
                    expiresIn,
                    tokenType = "Bearer",
                    user = new { username = request.Usuario }
                });
            }
            catch (SqlException sqlEx)
            {
#if DEBUG
                return BadRequest(new { message = sqlEx.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        // -----------------------
        // REFRESH
        // POST /api/Auth/refresh
        // Body: { refreshToken }
        // Respuesta: similar a login (nuevo access token y refresh token rotado)
        // -----------------------
        public class RefreshRequest { public string RefreshToken { get; set; } = string.Empty; }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.RefreshToken))
                return BadRequest(new { message = "refreshToken es requerido." });

            var incomingHash = ComputeHmacSha256(body.RefreshToken);

            try
            {
                // Use DbHelper to fetch record (DbHelper opens connection internally)
                var record = await _dbHelper.GetRefreshTokenByHashAsync(incomingHash);
                if (record == null)
                    return Unauthorized(new { message = "Refresh token inválido." });

                if (record.Revoked)
                {
                    // Reuse detectado -> revocar todos los tokens del usuario
                    await _dbHelper.RevokeAllRefreshTokensForUserAsync(record.Usuario);
                    return Unauthorized(new { message = "Refresh token revocado. Se han revocado las sesiones." });
                }

                if (record.ExpiresAt <= DateTime.UtcNow)
                    return Unauthorized(new { message = "Refresh token expirado." });

                // Rotación: crear nuevo refresh token y revocar el antiguo apuntando al nuevo hash
                var newPlain = GenerateRefreshTokenPlain();
                var newHash = ComputeHmacSha256(newPlain);
                var newExpiry = DateTime.UtcNow.AddDays(_refreshTokenDays);

                // Revoke old and insert new using DbHelper (each opens its own connection)
                await _dbHelper.RevokeRefreshTokenByIdAsync(record.Id, newHash);
                await _dbHelper.InsertRefreshTokenAsync(record.Usuario, newHash, newExpiry, Request.HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"].ToString());

                // Generar nuevo access token
                var accessToken = GenerateJwtToken(record.Usuario, out DateTime accessExpiry);
                var expiresIn = (int)(accessExpiry - DateTime.UtcNow).TotalSeconds;

                return Ok(new
                {
                    accessToken,
                    refreshToken = newPlain,
                    expiresIn,
                    tokenType = "Bearer",
                    user = new { username = record.Usuario }
                });
            }
            catch (SqlException sqlEx)
            {
#if DEBUG
                return BadRequest(new { message = sqlEx.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        // -----------------------
        // VALIDATE
        // POST /api/Auth/validate
        // Body optional: { token } - if omitted, will try Authorization header
        // Returns 200 with claims if valid, 401 if invalid
        // -----------------------
        public class TokenValidateRequest { public string? Token { get; set; } }

        [HttpPost("validate")]
        public IActionResult ValidateToken([FromBody] TokenValidateRequest? body)
        {
            string token = body?.Token ?? string.Empty;
            if (string.IsNullOrWhiteSpace(token))
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = authHeader.Substring("Bearer ".Length).Trim();
            }

            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Token no proporcionado." });

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = GetTokenValidationParameters();

            try
            {
                var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
                var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);
                return Ok(new { valid = true, claims });
            }
            catch (SecurityTokenException)
            {
                return Unauthorized(new { message = "Token inválido o expirado." });
            }
            catch (Exception ex)
            {
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        // -----------------------
        // LOGOUT
        // POST /api/Auth/logout
        // Body: { refreshToken } -> revoca ese refresh token (idempotente)
        // -----------------------
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequest body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.RefreshToken))
                return BadRequest(new { message = "refreshToken es requerido." });

            var incomingHash = ComputeHmacSha256(body.RefreshToken);

            try
            {
                var record = await _dbHelper.GetRefreshTokenByHashAsync(incomingHash);
                if (record == null)
                    return NoContent(); // idempotente

                await _dbHelper.RevokeRefreshTokenByIdAsync(record.Id, null);
                return NoContent();
            }
            catch (SqlException sqlEx)
            {
#if DEBUG
                return BadRequest(new { message = sqlEx.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        // -----------------------
        // Helpers: JWT generation, refresh token generation & hashing
        // -----------------------
        private string GenerateJwtToken(string subject)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // overload that returns expiry
        private string GenerateJwtToken(string subject, out DateTime expiresAt)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            expiresAt = DateTime.UtcNow.AddMinutes(_accessTokenMinutes);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshTokenPlain()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private string ComputeHmacSha256(string tokenPlain)
        {
            var key = Encoding.UTF8.GetBytes(_refreshSecret);
            using var hmac = new HMACSHA256(key);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(tokenPlain));
            return Convert.ToBase64String(hash);
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        }
    }
}