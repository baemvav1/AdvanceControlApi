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
    /// Implementación del servicio de relaciones proveedor-refacción que usa el procedimiento almacenado sp_relacionProveedorRefaccion_edit
    /// </summary>
    public class RelacionProveedorRefaccionService : IRelacionProveedorRefaccionService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<RelacionProveedorRefaccionService> _logger;

        public RelacionProveedorRefaccionService(DbHelper dbHelper, ILogger<RelacionProveedorRefaccionService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene refacciones asociadas a un proveedor usando el procedimiento almacenado sp_relacionProveedorRefaccion_edit
        /// </summary>
        public async Task<List<RelacionProveedorRefaccion>> GetRefaccionesAsync(RelacionProveedorRefaccionQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var refacciones = new List<RelacionProveedorRefaccion>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionProveedorRefaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", "select");
                command.Parameters.AddWithValue("@idRelacionProveedor", query.IdRelacionProveedor);
                command.Parameters.AddWithValue("@idProveedor", query.IdProveedor);
                command.Parameters.AddWithValue("@idRefaccion", query.IdRefaccion);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);
                command.Parameters.AddWithValue("@precio", query.Precio);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var refaccion = new RelacionProveedorRefaccion
                    {
                        IdRefaccion = reader.IsDBNull(reader.GetOrdinal("idRefaccion")) ? null : reader.GetInt32(reader.GetOrdinal("idRefaccion")),
                        Marca = reader.IsDBNull(reader.GetOrdinal("marca")) ? null : reader.GetString(reader.GetOrdinal("marca")),
                        Serie = reader.IsDBNull(reader.GetOrdinal("serie")) ? null : reader.GetString(reader.GetOrdinal("serie")),
                        Costo = reader.IsDBNull(reader.GetOrdinal("costo")) ? null : reader.GetDouble(reader.GetOrdinal("costo")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion"))
                    };

                    refacciones.Add(refaccion);
                }

                _logger.LogDebug("Se obtuvieron {Count} refacciones para el proveedor {IdProveedor}", refacciones.Count, query.IdProveedor);

                return refacciones;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener refacciones por proveedor. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener refacciones por proveedor de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener refacciones por proveedor");
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva relación proveedor-refacción
        /// </summary>
        public async Task<object> CreateRelacionAsync(RelacionProveedorRefaccionQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionProveedorRefaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "put");
                command.Parameters.AddWithValue("@idRelacionProveedor", 0);
                command.Parameters.AddWithValue("@idProveedor", query.IdProveedor);
                command.Parameters.AddWithValue("@idRefaccion", query.IdRefaccion);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);
                command.Parameters.AddWithValue("@precio", query.Precio);

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

                _logger.LogDebug("Relación creada para proveedor {IdProveedor} y refacción {IdRefaccion}", query.IdProveedor, query.IdRefaccion);
                return new { success = true, message = "Relación creada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear relación proveedor-refacción. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear relación proveedor-refacción en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear relación proveedor-refacción");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) una relación proveedor-refacción
        /// </summary>
        public async Task<object> DeleteRelacionAsync(int idRelacionProveedor)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionProveedorRefaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@idRelacionProveedor", idRelacionProveedor);
                command.Parameters.AddWithValue("@idProveedor", 0);
                command.Parameters.AddWithValue("@idRefaccion", 0);
                command.Parameters.AddWithValue("@nota", DBNull.Value);
                command.Parameters.AddWithValue("@precio", 0.0);

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

                _logger.LogDebug("Relación eliminada para idRelacionProveedor {IdRelacionProveedor}", idRelacionProveedor);
                return new { success = true, message = "Relación eliminada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar relación proveedor-refacción. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar relación proveedor-refacción en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar relación proveedor-refacción");
                throw;
            }
        }

        /// <summary>
        /// Actualiza la nota de una relación proveedor-refacción
        /// </summary>
        public async Task<object> UpdateNotaAsync(RelacionProveedorRefaccionQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionProveedorRefaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update_nota");
                command.Parameters.AddWithValue("@idRelacionProveedor", query.IdRelacionProveedor);
                command.Parameters.AddWithValue("@idProveedor", 0);
                command.Parameters.AddWithValue("@idRefaccion", 0);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);
                command.Parameters.AddWithValue("@precio", 0.0);

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

                _logger.LogDebug("Nota actualizada para idRelacionProveedor {IdRelacionProveedor}", query.IdRelacionProveedor);
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

        /// <summary>
        /// Actualiza el precio de una relación proveedor-refacción
        /// </summary>
        public async Task<object> UpdatePrecioAsync(RelacionProveedorRefaccionQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionProveedorRefaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update_precio");
                command.Parameters.AddWithValue("@idRelacionProveedor", query.IdRelacionProveedor);
                command.Parameters.AddWithValue("@idProveedor", 0);
                command.Parameters.AddWithValue("@idRefaccion", 0);
                command.Parameters.AddWithValue("@nota", DBNull.Value);
                command.Parameters.AddWithValue("@precio", query.Precio);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Update de precio devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Precio actualizado para idRelacionProveedor {IdRelacionProveedor}", query.IdRelacionProveedor);
                return new { success = true, message = "Precio actualizado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar precio de relación. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar precio de relación en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar precio de relación");
                throw;
            }
        }

        /// <summary>
        /// Obtiene proveedores que tienen una refacción específica con sus precios
        /// </summary>
        public async Task<List<ProveedorPorRefaccion>> GetProveedoresByRefaccionAsync(int idRefaccion)
        {
            var proveedores = new List<ProveedorPorRefaccion>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionProveedorRefaccion_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", "select_by_refaccion");
                command.Parameters.AddWithValue("@idRelacionProveedor", 0);
                command.Parameters.AddWithValue("@idProveedor", 0);
                command.Parameters.AddWithValue("@idRefaccion", idRefaccion);
                command.Parameters.AddWithValue("@nota", DBNull.Value);
                command.Parameters.AddWithValue("@precio", 0.0);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var proveedor = new ProveedorPorRefaccion
                    {
                        IdProveedor = reader.IsDBNull(reader.GetOrdinal("idProveedor")) ? null : reader.GetInt32(reader.GetOrdinal("idProveedor")),
                        NombreComercial = reader.IsDBNull(reader.GetOrdinal("nombre_comercial")) ? null : reader.GetString(reader.GetOrdinal("nombre_comercial")),
                        Costo = reader.IsDBNull(reader.GetOrdinal("costo")) ? null : reader.GetDouble(reader.GetOrdinal("costo"))
                    };

                    proveedores.Add(proveedor);
                }

                _logger.LogDebug("Se obtuvieron {Count} proveedores para la refacción {IdRefaccion}", proveedores.Count, idRefaccion);

                return proveedores;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener proveedores por refacción. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener proveedores por refacción de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener proveedores por refacción");
                throw;
            }
        }
    }
}
