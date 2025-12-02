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
    /// Implementación del servicio de relaciones refacción-equipo que usa el procedimiento almacenado sp_relacionRefaccionEquipo_edit
    /// </summary>
    public class RelacionRefaccionEquipoService : IRelacionRefaccionEquipoService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<RelacionRefaccionEquipoService> _logger;

        public RelacionRefaccionEquipoService(DbHelper dbHelper, ILogger<RelacionRefaccionEquipoService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene relaciones refacción-equipo usando el procedimiento almacenado sp_relacionRefaccionEquipo_edit
        /// </summary>
        public async Task<List<RelacionRefaccionEquipo>> GetRelacionesAsync(RelacionRefaccionEquipoQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var relaciones = new List<RelacionRefaccionEquipo>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionRefaccionEquipo_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", "select");
                command.Parameters.AddWithValue("@idRelacionRefaccion", DBNull.Value);
                command.Parameters.AddWithValue("@idRefaccion", query.IdRefaccion);
                command.Parameters.AddWithValue("@nota", DBNull.Value);
                command.Parameters.AddWithValue("@idEquipo", query.IdEquipo);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var relacion = new RelacionRefaccionEquipo
                    {
                        IdRefaccion = reader.IsDBNull(reader.GetOrdinal("idRefaccion")) ? null : reader.GetInt32(reader.GetOrdinal("idRefaccion")),
                        Marca = reader.IsDBNull(reader.GetOrdinal("marca")) ? null : reader.GetString(reader.GetOrdinal("marca")),
                        Serie = reader.IsDBNull(reader.GetOrdinal("serie")) ? null : reader.GetString(reader.GetOrdinal("serie")),
                        Costo = reader.IsDBNull(reader.GetOrdinal("costo")) ? null : reader.GetDouble(reader.GetOrdinal("costo")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion"))
                    };

                    relaciones.Add(relacion);
                }

                _logger.LogDebug("Se obtuvieron {Count} relaciones refacción-equipo", relaciones.Count);

                return relaciones;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener relaciones refacción-equipo. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener relaciones refacción-equipo de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener relaciones refacción-equipo");
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva relación refacción-equipo
        /// </summary>
        public async Task<object> CreateRelacionAsync(RelacionRefaccionEquipoQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionRefaccionEquipo_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "put");
                command.Parameters.AddWithValue("@idRelacionRefaccion", DBNull.Value);
                command.Parameters.AddWithValue("@idRefaccion", query.IdRefaccion);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);
                command.Parameters.AddWithValue("@idEquipo", query.IdEquipo);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Creación de relación devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Relación creada para refacción {IdRefaccion} y equipo {IdEquipo}", query.IdRefaccion, query.IdEquipo);
                return new { success = true, message = "Relación creada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear relación refacción-equipo. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear relación refacción-equipo en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear relación refacción-equipo");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) una relación refacción-equipo
        /// </summary>
        public async Task<object> DeleteRelacionAsync(int idRelacionRefaccion)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionRefaccionEquipo_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@idRelacionRefaccion", idRelacionRefaccion);
                command.Parameters.AddWithValue("@idRefaccion", DBNull.Value);
                command.Parameters.AddWithValue("@nota", DBNull.Value);
                command.Parameters.AddWithValue("@idEquipo", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Delete de relación devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Relación eliminada para idRelacionRefaccion {IdRelacionRefaccion}", idRelacionRefaccion);
                return new { success = true, message = "Relación eliminada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar relación refacción-equipo. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar relación refacción-equipo en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar relación refacción-equipo");
                throw;
            }
        }

        /// <summary>
        /// Actualiza la nota de una relación refacción-equipo
        /// </summary>
        public async Task<object> UpdateNotaAsync(RelacionRefaccionEquipoQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionRefaccionEquipo_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update_nota");
                command.Parameters.AddWithValue("@idRelacionRefaccion", query.IdRelacionRefaccion);
                command.Parameters.AddWithValue("@idRefaccion", DBNull.Value);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);
                command.Parameters.AddWithValue("@idEquipo", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Update de nota devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Nota actualizada para idRelacionRefaccion {IdRelacionRefaccion}", query.IdRelacionRefaccion);
                return new { success = true, message = "Nota actualizada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar nota de relación. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar nota de relación en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar nota de relación");
                throw;
            }
        }
    }
}
