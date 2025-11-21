using AdvanceApi.DTOs;
using AdvanceApi.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Implementación del servicio de información de usuario que usa el procedimiento almacenado sp_contacto_usuario_select
    /// </summary>
    public class ContactoUsuarioService : IContactoUsuarioService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<ContactoUsuarioService> _logger;

        public ContactoUsuarioService(DbHelper dbHelper, ILogger<ContactoUsuarioService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene la información del usuario usando el procedimiento almacenado sp_contacto_usuario_select
        /// </summary>
        public async Task<ContactoUsuarioDto?> GetContactoUsuarioAsync(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("El nombre de usuario no puede estar vacío", nameof(usuario));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_contacto_usuario_select", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetro del procedimiento almacenado
                command.Parameters.Add(new SqlParameter("@usuario", SqlDbType.NVarChar, 100) { Value = usuario });

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // Store ordinals to improve performance
                    var credencialIdOrdinal = reader.GetOrdinal("credencial_id");
                    var nombreCompletoOrdinal = reader.GetOrdinal("nombreCompleto");
                    var correoOrdinal = reader.GetOrdinal("correo");
                    var telefonoOrdinal = reader.GetOrdinal("telefono");
                    var nivelOrdinal = reader.GetOrdinal("nivel");
                    var tipoUsuarioOrdinal = reader.GetOrdinal("tipoUsuario");

                    var contactoUsuario = new ContactoUsuarioDto
                    {
                        CredencialId = reader.GetInt32(credencialIdOrdinal),
                        NombreCompleto = reader.IsDBNull(nombreCompletoOrdinal) 
                            ? string.Empty 
                            : reader.GetString(nombreCompletoOrdinal),
                        Correo = reader.IsDBNull(correoOrdinal) 
                            ? string.Empty 
                            : reader.GetString(correoOrdinal),
                        Telefono = reader.IsDBNull(telefonoOrdinal) 
                            ? string.Empty 
                            : reader.GetString(telefonoOrdinal),
                        Nivel = reader.IsDBNull(nivelOrdinal) 
                            ? 0 
                            : reader.GetInt32(nivelOrdinal),
                        TipoUsuario = reader.IsDBNull(tipoUsuarioOrdinal) 
                            ? string.Empty 
                            : reader.GetString(tipoUsuarioOrdinal)
                    };

                    _logger.LogDebug("Se obtuvo información del usuario {Usuario}", usuario);
                    return contactoUsuario;
                }

                _logger.LogWarning("No se encontró información para el usuario {Usuario}", usuario);
                return null;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener información del usuario {Usuario}. SqlError: {Message}", usuario, sqlEx.Message);
                throw new InvalidOperationException("Error al obtener información del usuario de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener información del usuario {Usuario}", usuario);
                throw;
            }
        }
    }
}
