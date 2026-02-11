using AdvanceApi.DTOs;
using AdvanceApi.Helpers;
using Clases;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Implementación del servicio de contactos que usa el procedimiento almacenado sp_contacto_edit
    /// </summary>
    public class ContactoService : IContactoService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<ContactoService> _logger;

        public ContactoService(DbHelper dbHelper, ILogger<ContactoService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene contactos usando el procedimiento almacenado sp_contacto_edit
        /// </summary>
        public async Task<List<Contacto>> GetContactosAsync(ContactoEditDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var contactos = new List<Contacto>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_contacto_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", "select");
                command.Parameters.AddWithValue("@contacto_id", (object?)query.ContactoId ?? DBNull.Value);
                command.Parameters.AddWithValue("@credencial_id", (object?)query.CredencialId ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombre", (object?)query.Nombre ?? DBNull.Value);
                command.Parameters.AddWithValue("@apellido", (object?)query.Apellido ?? DBNull.Value);
                command.Parameters.AddWithValue("@correo", (object?)query.Correo ?? DBNull.Value);
                command.Parameters.AddWithValue("@telefono", (object?)query.Telefono ?? DBNull.Value);
                command.Parameters.AddWithValue("@departamento", (object?)query.Departamento ?? DBNull.Value);
                command.Parameters.AddWithValue("@codigo_interno", (object?)query.CodigoInterno ?? DBNull.Value);
                command.Parameters.AddWithValue("@activo", (object?)query.Activo ?? DBNull.Value);
                command.Parameters.AddWithValue("@notas", (object?)query.Notas ?? DBNull.Value);
                command.Parameters.AddWithValue("@idProveedor", (object?)query.IdProveedor ?? DBNull.Value);
                command.Parameters.AddWithValue("@cargo", (object?)query.Cargo ?? DBNull.Value);
                command.Parameters.AddWithValue("@idCliente", (object?)query.IdCliente ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    // Check if this is an info/error message result set
                    try
                    {
                        var resultOrdinal = reader.GetOrdinal("Result");
                        // If we get here, this is a message result set, skip it
                        continue;
                    }
                    catch
                    {
                        // Not a message result set, proceed with parsing
                    }

                    var contacto = new Contacto
                    {
                        ContactoId = reader.GetInt64(reader.GetOrdinal("contacto_id")),
                        CredencialId = reader.IsDBNull(reader.GetOrdinal("credencial_id")) ? null : reader.GetInt64(reader.GetOrdinal("credencial_id")),
                        Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                        Apellido = reader.IsDBNull(reader.GetOrdinal("apellido")) ? null : reader.GetString(reader.GetOrdinal("apellido")),
                        Correo = reader.IsDBNull(reader.GetOrdinal("correo")) ? null : reader.GetString(reader.GetOrdinal("correo")),
                        Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                        Departamento = reader.IsDBNull(reader.GetOrdinal("departamento")) ? null : reader.GetString(reader.GetOrdinal("departamento")),
                        CodigoInterno = reader.IsDBNull(reader.GetOrdinal("codigo_interno")) ? null : reader.GetString(reader.GetOrdinal("codigo_interno")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("activo")) ? null : reader.GetBoolean(reader.GetOrdinal("activo")),
                        Notas = reader.IsDBNull(reader.GetOrdinal("notas")) ? null : reader.GetString(reader.GetOrdinal("notas")),
                        CreadoEn = reader.IsDBNull(reader.GetOrdinal("creado_en")) ? null : reader.GetDateTime(reader.GetOrdinal("creado_en")),
                        ActualizadoEn = reader.IsDBNull(reader.GetOrdinal("actualizado_en")) ? null : reader.GetDateTime(reader.GetOrdinal("actualizado_en")),
                        IdProveedor = reader.IsDBNull(reader.GetOrdinal("idProveedor")) ? null : reader.GetInt32(reader.GetOrdinal("idProveedor")),
                        Cargo = reader.IsDBNull(reader.GetOrdinal("cargo")) ? null : reader.GetString(reader.GetOrdinal("cargo")),
                        IdCliente = reader.IsDBNull(reader.GetOrdinal("idCliente")) ? null : reader.GetInt32(reader.GetOrdinal("idCliente")),
                    };

                    contactos.Add(contacto);
                }

                _logger.LogDebug("Se obtuvieron {Count} contactos", contactos.Count);

                return contactos;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener contactos. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener contactos de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener contactos");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo contacto usando el procedimiento almacenado sp_contacto_edit
        /// </summary>
        public async Task<object> CreateContactoAsync(ContactoEditDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_contacto_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "create");
                command.Parameters.AddWithValue("@contacto_id", DBNull.Value);
                command.Parameters.AddWithValue("@credencial_id", (object?)query.CredencialId ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombre", (object?)query.Nombre ?? DBNull.Value);
                command.Parameters.AddWithValue("@apellido", (object?)query.Apellido ?? DBNull.Value);
                command.Parameters.AddWithValue("@correo", (object?)query.Correo ?? DBNull.Value);
                command.Parameters.AddWithValue("@telefono", (object?)query.Telefono ?? DBNull.Value);
                command.Parameters.AddWithValue("@departamento", (object?)query.Departamento ?? DBNull.Value);
                command.Parameters.AddWithValue("@codigo_interno", (object?)query.CodigoInterno ?? DBNull.Value);
                command.Parameters.AddWithValue("@activo", (object?)query.Activo ?? DBNull.Value);
                command.Parameters.AddWithValue("@notas", (object?)query.Notas ?? DBNull.Value);
                command.Parameters.AddWithValue("@idProveedor", (object?)query.IdProveedor ?? DBNull.Value);
                command.Parameters.AddWithValue("@cargo", (object?)query.Cargo ?? DBNull.Value);
                command.Parameters.AddWithValue("@idCliente", (object?)query.IdCliente ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        var message = reader.GetString(reader.GetOrdinal("Message"));
                        
                        if (result == "Error")
                        {
                            return new { success = false, message };
                        }
                        
                        long? contactoId = null;
                        try
                        {
                            contactoId = reader.IsDBNull(reader.GetOrdinal("contacto_id")) ? null : reader.GetInt64(reader.GetOrdinal("contacto_id"));
                        }
                        catch
                        {
                            // contacto_id may not be present in response
                        }
                        
                        _logger.LogDebug("Create de contacto devolvió: {Result}, contacto_id: {ContactoId}", result, contactoId);
                        return new { success = true, message, contacto_id = contactoId };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Contacto creado correctamente");
                return new { success = true, message = "Contacto creado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear contacto. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear contacto en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear contacto");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un contacto por su ID
        /// </summary>
        public async Task<object> UpdateContactoAsync(ContactoEditDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_contacto_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update");
                command.Parameters.AddWithValue("@contacto_id", query.ContactoId);
                command.Parameters.AddWithValue("@credencial_id", (object?)query.CredencialId ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombre", (object?)query.Nombre ?? DBNull.Value);
                command.Parameters.AddWithValue("@apellido", (object?)query.Apellido ?? DBNull.Value);
                command.Parameters.AddWithValue("@correo", (object?)query.Correo ?? DBNull.Value);
                command.Parameters.AddWithValue("@telefono", (object?)query.Telefono ?? DBNull.Value);
                command.Parameters.AddWithValue("@departamento", (object?)query.Departamento ?? DBNull.Value);
                command.Parameters.AddWithValue("@codigo_interno", (object?)query.CodigoInterno ?? DBNull.Value);
                command.Parameters.AddWithValue("@activo", (object?)query.Activo ?? DBNull.Value);
                command.Parameters.AddWithValue("@notas", (object?)query.Notas ?? DBNull.Value);
                command.Parameters.AddWithValue("@idProveedor", (object?)query.IdProveedor ?? DBNull.Value);
                command.Parameters.AddWithValue("@cargo", (object?)query.Cargo ?? DBNull.Value);
                command.Parameters.AddWithValue("@idCliente", (object?)query.IdCliente ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        var message = reader.GetString(reader.GetOrdinal("Message"));
                        
                        if (result == "Error")
                        {
                            _logger.LogWarning("Update de contacto devolvió error: {Message}", message);
                            return new { success = false, message };
                        }
                        
                        return new { success = true, message };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Contacto {ContactoId} actualizado", query.ContactoId);
                return new { success = true, message = "Contacto actualizado exitosamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar contacto. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar contacto en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar contacto");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) un contacto por su ID
        /// </summary>
        public async Task<object> DeleteContactoAsync(long contactoId)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_contacto_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@contacto_id", contactoId);
                command.Parameters.AddWithValue("@credencial_id", DBNull.Value);
                command.Parameters.AddWithValue("@nombre", DBNull.Value);
                command.Parameters.AddWithValue("@apellido", DBNull.Value);
                command.Parameters.AddWithValue("@correo", DBNull.Value);
                command.Parameters.AddWithValue("@telefono", DBNull.Value);
                command.Parameters.AddWithValue("@departamento", DBNull.Value);
                command.Parameters.AddWithValue("@codigo_interno", DBNull.Value);
                command.Parameters.AddWithValue("@activo", DBNull.Value);
                command.Parameters.AddWithValue("@notas", DBNull.Value);
                command.Parameters.AddWithValue("@idProveedor", DBNull.Value);
                command.Parameters.AddWithValue("@cargo", DBNull.Value);
                command.Parameters.AddWithValue("@idCliente", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        var message = reader.GetString(reader.GetOrdinal("Message"));
                        
                        if (result == "Error")
                        {
                            _logger.LogWarning("Delete de contacto devolvió error: {Message}", message);
                            return new { success = false, message };
                        }
                        
                        return new { success = true, message };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Contacto {ContactoId} eliminado (soft delete)", contactoId);
                return new { success = true, message = "Contacto eliminado exitosamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar contacto. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar contacto en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar contacto");
                throw;
            }
        }
    }
}
