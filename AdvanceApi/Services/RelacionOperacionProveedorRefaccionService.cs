using AdvanceApi.DTOs;
using AdvanceApi.Helpers;
using Clases;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Implementación del servicio de relaciones operación-proveedor-refacción que usa el procedimiento almacenado sp_relacionOperacion_ProveedorRefaccion_edit
    /// </summary>
    public class RelacionOperacionProveedorRefaccionService : IRelacionOperacionProveedorRefaccionService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<RelacionOperacionProveedorRefaccionService> _logger;

        public RelacionOperacionProveedorRefaccionService(DbHelper dbHelper, ILogger<RelacionOperacionProveedorRefaccionService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene relaciones operación-proveedor-refacción usando el procedimiento almacenado sp_relacionOperacion_ProveedorRefaccion_edit
        /// </summary>
        public async Task<List<RelacionOperacionProveedorRefaccion>> GetRelacionesAsync(RelacionOperacionProveedorRefaccionQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var relaciones = new List<RelacionOperacionProveedorRefaccion>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionOperacion_ProveedorRefaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", "select");
                command.Parameters.AddWithValue("@idRelacionOperacion_ProveedorRefaccion", 0);
                command.Parameters.AddWithValue("@idOperacion", query.IdOperacion);
                command.Parameters.AddWithValue("@idProveedorRefaccion", 0);
                command.Parameters.AddWithValue("@precio", 0.0);
                command.Parameters.AddWithValue("@nota", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var relacion = new RelacionOperacionProveedorRefaccion
                    {
                        IdRelacionOperacionProveedorRefaccion = reader.IsDBNull(reader.GetOrdinal("idRelacionOperacion_ProveedorRefaccion")) ? null : reader.GetInt32(reader.GetOrdinal("idRelacionOperacion_ProveedorRefaccion")),
                        IdProveedorRefaccion = reader.IsDBNull(reader.GetOrdinal("idProveedorRefaccion")) ? null : reader.GetInt32(reader.GetOrdinal("idProveedorRefaccion")),
                        Precio = reader.IsDBNull(reader.GetOrdinal("precio")) ? null : reader.GetDouble(reader.GetOrdinal("precio")),
                        Nota = reader.IsDBNull(reader.GetOrdinal("nota")) ? null : reader.GetString(reader.GetOrdinal("nota"))
                    };

                    relaciones.Add(relacion);
                }

                _logger.LogDebug("Se obtuvieron {Count} relaciones para la operación {IdOperacion}", relaciones.Count, query.IdOperacion);

                return relaciones;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener relaciones operación-proveedor-refacción. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener relaciones operación-proveedor-refacción de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener relaciones operación-proveedor-refacción");
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva relación operación-proveedor-refacción
        /// </summary>
        public async Task<object> CreateRelacionAsync(RelacionOperacionProveedorRefaccionQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionOperacion_ProveedorRefaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "put");
                command.Parameters.AddWithValue("@idRelacionOperacion_ProveedorRefaccion", 0);
                command.Parameters.AddWithValue("@idOperacion", query.IdOperacion);
                command.Parameters.AddWithValue("@idProveedorRefaccion", query.IdProveedorRefaccion);
                command.Parameters.AddWithValue("@precio", query.Precio);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);

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

                _logger.LogDebug("Relación creada para operación {IdOperacion} y proveedor refacción {IdProveedorRefaccion}", query.IdOperacion, query.IdProveedorRefaccion);
                return new { success = true, message = "Relación creada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear relación operación-proveedor-refacción. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear relación operación-proveedor-refacción en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear relación operación-proveedor-refacción");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) una relación operación-proveedor-refacción
        /// </summary>
        public async Task<object> DeleteRelacionAsync(int idRelacionOperacionProveedorRefaccion)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionOperacion_ProveedorRefaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@idRelacionOperacion_ProveedorRefaccion", idRelacionOperacionProveedorRefaccion);
                command.Parameters.AddWithValue("@idOperacion", 0);
                command.Parameters.AddWithValue("@idProveedorRefaccion", 0);
                command.Parameters.AddWithValue("@precio", 0.0);
                command.Parameters.AddWithValue("@nota", DBNull.Value);

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

                _logger.LogDebug("Relación eliminada con ID {IdRelacionOperacionProveedorRefaccion}", idRelacionOperacionProveedorRefaccion);
                return new { success = true, message = "Relación eliminada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar relación operación-proveedor-refacción. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar relación operación-proveedor-refacción en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar relación operación-proveedor-refacción");
                throw;
            }
        }

        /// <summary>
        /// Actualiza la nota de una relación operación-proveedor-refacción
        /// </summary>
        public async Task<object> UpdateNotaAsync(RelacionOperacionProveedorRefaccionQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionOperacion_ProveedorRefaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update_nota");
                command.Parameters.AddWithValue("@idRelacionOperacion_ProveedorRefaccion", query.IdRelacionOperacionProveedorRefaccion);
                command.Parameters.AddWithValue("@idOperacion", 0);
                command.Parameters.AddWithValue("@idProveedorRefaccion", 0);
                command.Parameters.AddWithValue("@precio", 0.0);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);

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

                _logger.LogDebug("Nota actualizada para idRelacionOperacion_ProveedorRefaccion {IdRelacionOperacionProveedorRefaccion}", query.IdRelacionOperacionProveedorRefaccion);
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
