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
    /// Implementación del servicio de refacciones que usa el procedimiento almacenado sp_refaccion_edit
    /// </summary>
    public class RefaccionService : IRefaccionService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<RefaccionService> _logger;

        public RefaccionService(DbHelper dbHelper, ILogger<RefaccionService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Ejecuta operaciones CRUD de refacciones usando el procedimiento almacenado sp_refaccion_edit
        /// </summary>
        public async Task<List<Refaccion>> ExecuteRefaccionOperationAsync(RefaccionQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var refacciones = new List<Refaccion>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_refaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", query.Operacion);
                command.Parameters.AddWithValue("@idRefaccion", query.IdRefaccion);
                command.Parameters.AddWithValue("@marca", (object?)query.Marca ?? DBNull.Value);
                command.Parameters.AddWithValue("@serie", (object?)query.Serie ?? DBNull.Value);
                command.Parameters.AddWithValue("@costo", (object?)query.Costo ?? DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@idProveedor", query.IdProveedor);
                command.Parameters.AddWithValue("@estatus", query.Estatus);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var refaccion = new Refaccion
                    {
                        IdRefaccion = reader.GetInt32(reader.GetOrdinal("idRefaccion")),
                        Marca = reader.IsDBNull(reader.GetOrdinal("marca")) ? null : reader.GetString(reader.GetOrdinal("marca")),
                        Serie = reader.IsDBNull(reader.GetOrdinal("serie")) ? null : reader.GetString(reader.GetOrdinal("serie")),
                        Costo = reader.IsDBNull(reader.GetOrdinal("costo")) ? null : reader.GetDouble(reader.GetOrdinal("costo")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                        Estatus = reader.IsDBNull(reader.GetOrdinal("estatus")) ? null : reader.GetBoolean(reader.GetOrdinal("estatus"))
                    };

                    refacciones.Add(refaccion);
                }

                _logger.LogDebug("Operación '{Operacion}' ejecutada. Se obtuvieron {Count} refacciones", query.Operacion, refacciones.Count);

                return refacciones;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al ejecutar operación de refacción. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al ejecutar operación de refacción en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al ejecutar operación de refacción");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) una refacción por su ID
        /// </summary>
        public async Task<object> DeleteRefaccionAsync(int idRefaccion)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_refaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@idRefaccion", idRefaccion);
                command.Parameters.AddWithValue("@marca", DBNull.Value);
                command.Parameters.AddWithValue("@serie", DBNull.Value);
                command.Parameters.AddWithValue("@costo", DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@idProveedor", 0);
                command.Parameters.AddWithValue("@estatus", true);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Delete de refacción devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Refacción {IdRefaccion} eliminada (soft delete)", idRefaccion);
                return new { success = true, message = "Refacción eliminada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar refacción. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar refacción en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar refacción");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una refacción por su ID
        /// </summary>
        public async Task<object> UpdateRefaccionAsync(RefaccionQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_refaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update");
                command.Parameters.AddWithValue("@idRefaccion", query.IdRefaccion);
                command.Parameters.AddWithValue("@marca", (object?)query.Marca ?? DBNull.Value);
                command.Parameters.AddWithValue("@serie", (object?)query.Serie ?? DBNull.Value);
                command.Parameters.AddWithValue("@costo", (object?)query.Costo ?? DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@idProveedor", query.IdProveedor);
                command.Parameters.AddWithValue("@estatus", query.Estatus);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Update de refacción devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Refacción {IdRefaccion} actualizada", query.IdRefaccion);
                return new { success = true, message = "Refacción actualizada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar refacción. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar refacción en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar refacción");
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva refacción usando el procedimiento almacenado sp_refaccion_edit con operación 'put'
        /// </summary>
        public async Task<object> CreateRefaccionAsync(RefaccionQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_refaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "put");
                command.Parameters.AddWithValue("@idRefaccion", 0);
                command.Parameters.AddWithValue("@marca", (object?)query.Marca ?? DBNull.Value);
                command.Parameters.AddWithValue("@serie", (object?)query.Serie ?? DBNull.Value);
                command.Parameters.AddWithValue("@costo", (object?)query.Costo ?? DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@idProveedor", query.IdProveedor);
                command.Parameters.AddWithValue("@estatus", query.Estatus);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Create de refacción devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Refacción creada correctamente");
                return new { success = true, message = "Refacción creada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear refacción. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear refacción en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear refacción");
                throw;
            }
        }

        /// <summary>
        /// Verifica si una refacción tiene proveedores relacionados
        /// </summary>
        public async Task<object> CheckProveedorExistsAsync(int idRefaccion)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_refaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "select_exists_proveedor");
                command.Parameters.AddWithValue("@idRefaccion", idRefaccion);
                command.Parameters.AddWithValue("@marca", DBNull.Value);
                command.Parameters.AddWithValue("@serie", DBNull.Value);
                command.Parameters.AddWithValue("@costo", DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@idProveedor", 0);
                command.Parameters.AddWithValue("@estatus", true);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var result = reader.GetInt32(reader.GetOrdinal("Result"));
                    _logger.LogDebug("Check proveedor exists para refacción {IdRefaccion}: {Result}", idRefaccion, result);
                    return new { exists = result == 1, result = result };
                }

                return new { exists = false, result = 0 };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al verificar proveedores de refacción. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al verificar proveedores de refacción en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al verificar proveedores de refacción");
                throw;
            }
        }
    }
}
