using AdvanceApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public AuthController(DbHelper dbHelper, IConfiguration configuration)
        {
            _dbHelper = dbHelper;
            _secretKey = configuration["Jwt:Key"] ?? throw new Exception("No se encontró Jwt:Key en la configuración.");
            _issuer = configuration["Jwt:Issuer"] ?? throw new Exception("No se encontró Jwt:Issuer en la configuración.");
            _audience = configuration["Jwt:Audience"] ?? throw new Exception("No se encontró Jwt:Audience en la configuración.");
        }

        // Se usa la clase Clases.Usuario (asegúrate que coincide con la definida en el proyecto):
        // public class Usuario { public string usuario { get; set; } = string.Empty; public string contraseña { get; set; } = string.Empty; }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] usuario request)
        {
            if (request == null)
                return BadRequest(new { message = "Request body es requerido." });

            // Validación mínima
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

                // El SP espera los parámetros @usuario NVARCHAR(150) y @contraseña NVARCHAR(100)
                cmd.Parameters.Add(new SqlParameter("@usuario", SqlDbType.NVarChar, 150) { Value = request.Usuario });
                cmd.Parameters.Add(new SqlParameter("@contraseña", SqlDbType.NVarChar, 100) { Value = request.Pass });

                // El SP hace SELECT @acceso AS log; ExecuteScalar devuelve el valor de la primera columna de la primera fila.
                var scalar = await cmd.ExecuteScalarAsync();

                if (scalar == null || scalar == DBNull.Value)
                {
                    // SP no devolvió valor: tratar como credenciales inválidas
                    return Unauthorized(new { message = "Credenciales inválidas." });
                }

                // Manejar distintos tipos posibles que el SP devuelva (BIT -> boolean, INT -> 0/1, o string)
                bool isAuthorized;
                if (scalar is bool b) isAuthorized = b;
                else if (scalar is int i) isAuthorized = i != 0;
                else if (bool.TryParse(scalar.ToString(), out var parsedBool)) isAuthorized = parsedBool;
                else if (int.TryParse(scalar.ToString(), out var parsedInt)) isAuthorized = parsedInt != 0;
                else isAuthorized = false;

                if (!isAuthorized)
                    return Unauthorized(new { message = "Credenciales inválidas." });

                // Usuario autorizado: generar token JWT y devolverlo.
                var token = GenerateJwtToken(request.Usuario);

                return Ok(new { token });
            }
            catch (SqlException sqlEx)
            {
                // El stored procedure usa RAISERROR para parámetros obligatorios; eso llega como SqlException.
                // Si quieres propagar el mensaje exacto para debugging en desarrollo, lo puedes devolver,
                // pero en producción conviene devolver mensajes genéricos y loggear el detalle.
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
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}