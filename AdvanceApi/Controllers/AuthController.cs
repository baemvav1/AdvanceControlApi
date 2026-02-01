using AdvanceApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(DbHelper dbHelper, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _dbHelper = dbHelper;
            _logger = logger;
            _secretKey = configuration["Jwt:Key"] ?? throw new Exception("No se encontró Jwt:Key en la configuración.");
            _issuer = configuration["Jwt:Issuer"] ?? throw new Exception("No se encontró Jwt:Issuer en la configuración.");
            _audience = configuration["Jwt:Audience"] ?? throw new Exception("No se encontró Jwt:Audience en la configuración.");

            _accessTokenMinutes = int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
            _refreshTokenDays = int.TryParse(configuration["RefreshToken:Days"], out var d) ? d : 30;
            _refreshSecret = configuration["RefreshToken:Secret"] ?? throw new Exception("No se encontró RefreshToken:Secret en la configuración.");
        }

        /// <summary>
        /// Inicia sesión con credenciales de usuario y genera tokens de acceso y refresco
        /// </summary>
        /// <param name="request">Credenciales del usuario</param>
        /// <returns>Tokens de acceso y refresco junto con información de expiración</returns>
        /// <response code="200">Login exitoso, retorna tokens</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="401">Credenciales incorrectas</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario? request)
        {
            // Input validation - CodeQL may flag this as "user-controlled bypass" but this is
            // the correct behavior for a login endpoint. The actual authentication is performed
            // by the database stored procedure 'login_credencial' which validates credentials securely.
            // This check only validates that the input format is correct before hitting the database.
            if (!ModelState.IsValid || request == null)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Usuario y contraseña son requeridos." });

            try
            {
                await using var conn = _dbHelper.GetConnection();
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("login_credencial", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@usuario", SqlDbType.NVarChar, 150) { Value = request.Username });
                cmd.Parameters.Add(new SqlParameter("@contraseña", SqlDbType.NVarChar, 100) { Value = request.Password });

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
                var accessToken = GenerateJwtToken(request.Username, out DateTime accessExpiry);
                var expiresIn = (int)(accessExpiry - DateTime.UtcNow).TotalSeconds;

                // Generar refresh token (plano), hashearlo con HMAC, guardar hash en DB via DbHelper.InsertRefreshTokenAsync
                var refreshPlain = GenerateRefreshTokenPlain();
                var refreshHash = ComputeHmacSha256(refreshPlain);
                var refreshExpiry = DateTime.UtcNow.AddDays(_refreshTokenDays);

                // Use DbHelper wrapper (opens its own connection)
                await _dbHelper.InsertRefreshTokenAsync(request.Username, refreshHash, refreshExpiry, Request.HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"].ToString());

                return Ok(new
                {
                    accessToken,
                    refreshToken = refreshPlain,
                    expiresIn,
                    tokenType = "Bearer",
                    user = new { username = request.Username }
                });
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error de SQL al iniciar sesión para el usuario {Username}", request.Username);
#if DEBUG
                return BadRequest(new { message = sqlEx.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al iniciar sesión para el usuario {Username}", request.Username);
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Modelo para solicitudes de refresh token
        /// </summary>
        public class RefreshRequest 
        { 
            /// <summary>
            /// Token de refresco previamente emitido
            /// </summary>
            [Required(ErrorMessage = "El refresh token es requerido")]
            public string RefreshToken { get; set; } = string.Empty; 
        }

        /// <summary>
        /// Renueva el token de acceso usando un refresh token válido
        /// Implementa rotación de tokens: el refresh token antiguo se revoca y se emite uno nuevo
        /// </summary>
        /// <param name="body">Objeto con el refresh token actual</param>
        /// <returns>Nuevos tokens de acceso y refresco</returns>
        /// <response code="200">Tokens renovados exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="401">Token inválido, expirado o revocado</response>
        /// <response code="500">Error interno del servidor</response>
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
                _logger.LogError(sqlEx, "Error de SQL al refrescar token");
#if DEBUG
                return BadRequest(new { message = sqlEx.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al refrescar token");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Modelo para validación de token
        /// </summary>
        public class TokenValidateRequest 
        { 
            /// <summary>
            /// Token JWT a validar (opcional si se envía en el header Authorization)
            /// </summary>
            public string? Token { get; set; } 
        }

        /// <summary>
        /// Valida un token JWT y retorna sus claims si es válido
        /// </summary>
        /// <param name="body">Objeto con el token a validar (opcional si se usa el header Authorization)</param>
        /// <returns>Claims del token si es válido</returns>
        /// <response code="200">Token válido, retorna los claims</response>
        /// <response code="400">Token no proporcionado</response>
        /// <response code="401">Token inválido o expirado</response>
        /// <response code="500">Error interno del servidor</response>
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
                _logger.LogError(ex, "Error al validar token");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Cierra la sesión revocando el refresh token
        /// Esta operación es idempotente
        /// </summary>
        /// <param name="body">Objeto con el refresh token a revocar</param>
        /// <returns>204 No Content si la operación fue exitosa</returns>
        /// <response code="204">Sesión cerrada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="500">Error interno del servidor</response>
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
                _logger.LogError(sqlEx, "Error de SQL al revocar token");
#if DEBUG
                return BadRequest(new { message = sqlEx.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al revocar token");
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