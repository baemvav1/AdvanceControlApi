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
    /// Implementación del servicio de servicios que usa el procedimiento almacenado sp_servicio_edit
    /// </summary>
    public class ServicioService : IServicioService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<ServicioService> _logger;

        public ServicioService(DbHelper dbHelper, ILogger<ServicioService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Ejecuta operaciones CRUD de servicios usando el procedimiento almacenado sp_servicio_edit
        /// </summary>
        public async Task<List<Servicio>> ExecuteServicioOperationAsync(ServicioQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var servicios = new List<Servicio>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_servicio_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", query.Operacion);
                command.Parameters.AddWithValue("@idServicio", query.IdServicio);
                command.Parameters.AddWithValue("@concepto", (object?)query.Concepto ?? DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@costo", (object?)query.Costo ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var servicio = new Servicio
                    {
                        IdServicio = reader.GetInt32(reader.GetOrdinal("idServicio")),
                        Concepto = reader.IsDBNull(reader.GetOrdinal("concepto")) ? null : reader.GetString(reader.GetOrdinal("concepto")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                        Costo = reader.IsDBNull(reader.GetOrdinal("costo")) ? null : reader.GetDouble(reader.GetOrdinal("costo")),
                        Estatus = reader.IsDBNull(reader.GetOrdinal("estatus")) ? null : reader.GetBoolean(reader.GetOrdinal("estatus"))
                    };

                    servicios.Add(servicio);
                }

                _logger.LogDebug("Operación '{Operacion}' ejecutada. Se obtuvieron {Count} servicios", query.Operacion, servicios.Count);

                return servicios;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al ejecutar operación de servicio. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al ejecutar operación de servicio en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al ejecutar operación de servicio");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) un servicio por su ID
        /// </summary>
        public async Task<object> DeleteServicioAsync(int idServicio)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_servicio_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@idServicio", idServicio);
                command.Parameters.AddWithValue("@concepto", DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@costo", DBNull.Value);
                command.Parameters.AddWithValue("@estatus", true);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Delete de servicio devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Servicio {IdServicio} eliminado (soft delete)", idServicio);
                return new { success = true, message = "Servicio eliminado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar servicio. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar servicio en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar servicio");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un servicio por su ID
        /// </summary>
        public async Task<object> UpdateServicioAsync(ServicioQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_servicio_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update");
                command.Parameters.AddWithValue("@idServicio", query.IdServicio);
                command.Parameters.AddWithValue("@concepto", (object?)query.Concepto ?? DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@costo", (object?)query.Costo ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Update de servicio devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Servicio {IdServicio} actualizado", query.IdServicio);
                return new { success = true, message = "Servicio actualizado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar servicio. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar servicio en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar servicio");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo servicio usando el procedimiento almacenado sp_servicio_edit con operación 'put'
        /// </summary>
        public async Task<object> CreateServicioAsync(ServicioQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_servicio_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "put");
                command.Parameters.AddWithValue("@idServicio", 0);
                command.Parameters.AddWithValue("@concepto", (object?)query.Concepto ?? DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@costo", (object?)query.Costo ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Create de servicio devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Servicio creado correctamente");
                return new { success = true, message = "Servicio creado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear servicio. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear servicio en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear servicio");
                throw;
            }
        }
    }
}
