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
    /// Implementación del servicio de entidades que usa el procedimiento almacenado sp_entidad_edit
    /// </summary>
    public class EntidadService : IEntidadService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<EntidadService> _logger;

        public EntidadService(DbHelper dbHelper, ILogger<EntidadService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Ejecuta operaciones CRUD de entidades usando el procedimiento almacenado sp_entidad_edit
        /// </summary>
        public async Task<List<Entidad>> ExecuteEntidadOperationAsync(EntidadQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var entidades = new List<Entidad>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_entidad_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", query.Operacion);
                command.Parameters.AddWithValue("@idEntidad", (object?)query.IdEntidad ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombreComercial", (object?)query.NombreComercial ?? DBNull.Value);
                command.Parameters.AddWithValue("@razonSocial", (object?)query.RazonSocial ?? DBNull.Value);
                command.Parameters.AddWithValue("@RFC", (object?)query.RFC ?? DBNull.Value);
                command.Parameters.AddWithValue("@CP", (object?)query.CP ?? DBNull.Value);
                command.Parameters.AddWithValue("@estado", (object?)query.Estado ?? DBNull.Value);
                command.Parameters.AddWithValue("@ciudad", (object?)query.Ciudad ?? DBNull.Value);
                command.Parameters.AddWithValue("@pais", (object?)query.Pais ?? DBNull.Value);
                command.Parameters.AddWithValue("@calle", (object?)query.Calle ?? DBNull.Value);
                command.Parameters.AddWithValue("@nomExt", (object?)query.NomExt ?? DBNull.Value);
                command.Parameters.AddWithValue("@numInt", (object?)query.NumInt ?? DBNull.Value);
                command.Parameters.AddWithValue("@colonia", (object?)query.Colonia ?? DBNull.Value);
                command.Parameters.AddWithValue("@apoderado", (object?)query.Apoderado ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var entidad = new Entidad
                    {
                        IdEntidad = reader.GetInt32(reader.GetOrdinal("idEntidad")),
                        NombreComercial = reader.IsDBNull(reader.GetOrdinal("nombreComercial")) ? null : reader.GetString(reader.GetOrdinal("nombreComercial")),
                        RazonSocial = reader.IsDBNull(reader.GetOrdinal("razonSocial")) ? null : reader.GetString(reader.GetOrdinal("razonSocial")),
                        RFC = reader.IsDBNull(reader.GetOrdinal("RFC")) ? null : reader.GetString(reader.GetOrdinal("RFC")),
                        CP = reader.IsDBNull(reader.GetOrdinal("CP")) ? null : reader.GetString(reader.GetOrdinal("CP")),
                        Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                        Ciudad = reader.IsDBNull(reader.GetOrdinal("ciudad")) ? null : reader.GetString(reader.GetOrdinal("ciudad")),
                        Pais = reader.IsDBNull(reader.GetOrdinal("pais")) ? null : reader.GetString(reader.GetOrdinal("pais")),
                        Calle = reader.IsDBNull(reader.GetOrdinal("calle")) ? null : reader.GetString(reader.GetOrdinal("calle")),
                        NumExt = reader.IsDBNull(reader.GetOrdinal("numExt")) ? null : reader.GetString(reader.GetOrdinal("numExt")),
                        NumInt = reader.IsDBNull(reader.GetOrdinal("numInt")) ? null : reader.GetString(reader.GetOrdinal("numInt")),
                        Colonia = reader.IsDBNull(reader.GetOrdinal("colonia")) ? null : reader.GetString(reader.GetOrdinal("colonia")),
                        Apoderado = reader.IsDBNull(reader.GetOrdinal("apoderado")) ? null : reader.GetString(reader.GetOrdinal("apoderado")),
                        Estatus = reader.IsDBNull(reader.GetOrdinal("estatus")) ? null : reader.GetBoolean(reader.GetOrdinal("estatus"))
                    };

                    entidades.Add(entidad);
                }

                _logger.LogDebug("Operación '{Operacion}' ejecutada. Se obtuvieron {Count} entidades", query.Operacion, entidades.Count);

                return entidades;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al ejecutar operación de entidad. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al ejecutar operación de entidad en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al ejecutar operación de entidad");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) una entidad por su ID
        /// </summary>
        public async Task<object> DeleteEntidadAsync(int idEntidad)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_entidad_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@idEntidad", idEntidad);
                command.Parameters.AddWithValue("@nombreComercial", DBNull.Value);
                command.Parameters.AddWithValue("@razonSocial", DBNull.Value);
                command.Parameters.AddWithValue("@RFC", DBNull.Value);
                command.Parameters.AddWithValue("@CP", DBNull.Value);
                command.Parameters.AddWithValue("@estado", DBNull.Value);
                command.Parameters.AddWithValue("@ciudad", DBNull.Value);
                command.Parameters.AddWithValue("@pais", DBNull.Value);
                command.Parameters.AddWithValue("@calle", DBNull.Value);
                command.Parameters.AddWithValue("@nomExt", DBNull.Value);
                command.Parameters.AddWithValue("@numInt", DBNull.Value);
                command.Parameters.AddWithValue("@colonia", DBNull.Value);
                command.Parameters.AddWithValue("@apoderado", DBNull.Value);
                command.Parameters.AddWithValue("@estatus", true);

                await using var reader = await command.ExecuteReaderAsync();

                // Leer el resultado del stored procedure
                if (await reader.ReadAsync())
                {
                    var result = reader.GetString(reader.GetOrdinal("Result"));
                    var message = reader.GetString(reader.GetOrdinal("Message"));
                    
                    if (result == "Success")
                    {
                        var entidadId = reader.GetInt32(reader.GetOrdinal("idEntidad"));
                        _logger.LogDebug("Entidad {IdEntidad} eliminada (soft delete)", entidadId);
                        return new { success = true, message = message, idEntidad = entidadId };
                    }
                    else
                    {
                        _logger.LogWarning("Error al eliminar entidad: {Message}", message);
                        return new { success = false, message = message };
                    }
                }

                _logger.LogWarning("No se recibió respuesta del stored procedure");
                return new { success = false, message = "No se recibió respuesta del servidor" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar entidad. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar entidad en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar entidad");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una entidad por su ID
        /// </summary>
        public async Task<object> UpdateEntidadAsync(EntidadQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_entidad_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update");
                command.Parameters.AddWithValue("@idEntidad", query.IdEntidad);
                command.Parameters.AddWithValue("@nombreComercial", (object?)query.NombreComercial ?? DBNull.Value);
                command.Parameters.AddWithValue("@razonSocial", (object?)query.RazonSocial ?? DBNull.Value);
                command.Parameters.AddWithValue("@RFC", (object?)query.RFC ?? DBNull.Value);
                command.Parameters.AddWithValue("@CP", (object?)query.CP ?? DBNull.Value);
                command.Parameters.AddWithValue("@estado", (object?)query.Estado ?? DBNull.Value);
                command.Parameters.AddWithValue("@ciudad", (object?)query.Ciudad ?? DBNull.Value);
                command.Parameters.AddWithValue("@pais", (object?)query.Pais ?? DBNull.Value);
                command.Parameters.AddWithValue("@calle", (object?)query.Calle ?? DBNull.Value);
                command.Parameters.AddWithValue("@nomExt", (object?)query.NomExt ?? DBNull.Value);
                command.Parameters.AddWithValue("@numInt", (object?)query.NumInt ?? DBNull.Value);
                command.Parameters.AddWithValue("@colonia", (object?)query.Colonia ?? DBNull.Value);
                command.Parameters.AddWithValue("@apoderado", (object?)query.Apoderado ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);

                await using var reader = await command.ExecuteReaderAsync();

                // Leer el resultado del stored procedure
                if (await reader.ReadAsync())
                {
                    var result = reader.GetString(reader.GetOrdinal("Result"));
                    var message = reader.GetString(reader.GetOrdinal("Message"));
                    
                    if (result == "Success")
                    {
                        var entidadId = reader.GetInt32(reader.GetOrdinal("idEntidad"));
                        var rowsAffected = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                        _logger.LogDebug("Entidad {IdEntidad} actualizada", entidadId);
                        return new { success = true, message = message, idEntidad = entidadId, rowsAffected = rowsAffected };
                    }
                    else
                    {
                        _logger.LogWarning("Error al actualizar entidad: {Message}", message);
                        return new { success = false, message = message };
                    }
                }

                _logger.LogWarning("No se recibió respuesta del stored procedure");
                return new { success = false, message = "No se recibió respuesta del servidor" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar entidad. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar entidad en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar entidad");
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva entidad usando el procedimiento almacenado sp_entidad_edit con operacion='create'
        /// </summary>
        public async Task<object> CreateEntidadAsync(EntidadQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_entidad_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "create");
                command.Parameters.AddWithValue("@idEntidad", DBNull.Value);
                command.Parameters.AddWithValue("@nombreComercial", (object?)query.NombreComercial ?? DBNull.Value);
                command.Parameters.AddWithValue("@razonSocial", (object?)query.RazonSocial ?? DBNull.Value);
                command.Parameters.AddWithValue("@RFC", (object?)query.RFC ?? DBNull.Value);
                command.Parameters.AddWithValue("@CP", (object?)query.CP ?? DBNull.Value);
                command.Parameters.AddWithValue("@estado", (object?)query.Estado ?? DBNull.Value);
                command.Parameters.AddWithValue("@ciudad", (object?)query.Ciudad ?? DBNull.Value);
                command.Parameters.AddWithValue("@pais", (object?)query.Pais ?? DBNull.Value);
                command.Parameters.AddWithValue("@calle", (object?)query.Calle ?? DBNull.Value);
                command.Parameters.AddWithValue("@nomExt", (object?)query.NomExt ?? DBNull.Value);
                command.Parameters.AddWithValue("@numInt", (object?)query.NumInt ?? DBNull.Value);
                command.Parameters.AddWithValue("@colonia", (object?)query.Colonia ?? DBNull.Value);
                command.Parameters.AddWithValue("@apoderado", (object?)query.Apoderado ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);

                await using var reader = await command.ExecuteReaderAsync();

                // Leer el resultado del stored procedure
                if (await reader.ReadAsync())
                {
                    var result = reader.GetString(reader.GetOrdinal("Result"));
                    var message = reader.GetString(reader.GetOrdinal("Message"));
                    
                    if (result == "Success")
                    {
                        var idEntidad = reader.GetInt32(reader.GetOrdinal("idEquipo"));
                        _logger.LogDebug("Entidad creada con ID {IdEntidad}", idEntidad);
                        return new { success = true, message = message, idEntidad = idEntidad };
                    }
                    else
                    {
                        _logger.LogWarning("Error al crear entidad: {Message}", message);
                        return new { success = false, message = message };
                    }
                }

                _logger.LogWarning("No se recibió respuesta del stored procedure");
                return new { success = false, message = "No se recibió respuesta del servidor" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear entidad. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear entidad en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear entidad");
                throw;
            }
        }
    }
}
