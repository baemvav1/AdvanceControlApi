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
    /// Implementación del servicio de proveedores que usa el procedimiento almacenado sp_proveedor_edit
    /// </summary>
    public class ProveedorService : IProveedorService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<ProveedorService> _logger;

        public ProveedorService(DbHelper dbHelper, ILogger<ProveedorService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Ejecuta operaciones CRUD de proveedores usando el procedimiento almacenado sp_proveedor_edit
        /// </summary>
        public async Task<List<Proveedor>> ExecuteProveedorOperationAsync(ProveedorQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var proveedores = new List<Proveedor>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_proveedor_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", query.Operacion);
                command.Parameters.AddWithValue("@idProveedor", query.IdProveedor);
                command.Parameters.AddWithValue("@rfc", (object?)query.Rfc ?? DBNull.Value);
                command.Parameters.AddWithValue("@razon_social", (object?)query.RazonSocial ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombreComercial", (object?)query.NombreComercial ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var proveedor = new Proveedor
                    {
                        IdProveedor = reader.GetInt32(reader.GetOrdinal("idProveedor")),
                        Rfc = reader.IsDBNull(reader.GetOrdinal("rfc")) ? null : reader.GetString(reader.GetOrdinal("rfc")),
                        RazonSocial = reader.IsDBNull(reader.GetOrdinal("razon_social")) ? null : reader.GetString(reader.GetOrdinal("razon_social")),
                        NombreComercial = reader.IsDBNull(reader.GetOrdinal("nombre_comercial")) ? null : reader.GetString(reader.GetOrdinal("nombre_comercial")),
                        Estatus = reader.IsDBNull(reader.GetOrdinal("estatus")) ? null : reader.GetBoolean(reader.GetOrdinal("estatus")),
                        Nota = reader.IsDBNull(reader.GetOrdinal("nota")) ? null : reader.GetString(reader.GetOrdinal("nota"))
                    };

                    proveedores.Add(proveedor);
                }

                _logger.LogDebug("Operación '{Operacion}' ejecutada. Se obtuvieron {Count} proveedores", query.Operacion, proveedores.Count);

                return proveedores;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al ejecutar operación de proveedor. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al ejecutar operación de proveedor en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al ejecutar operación de proveedor");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) un proveedor por su ID
        /// </summary>
        public async Task<object> DeleteProveedorAsync(int idProveedor)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_proveedor_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@idProveedor", idProveedor);
                command.Parameters.AddWithValue("@rfc", DBNull.Value);
                command.Parameters.AddWithValue("@razon_social", DBNull.Value);
                command.Parameters.AddWithValue("@nombreComercial", DBNull.Value);
                command.Parameters.AddWithValue("@estatus", true);
                command.Parameters.AddWithValue("@nota", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Delete de proveedor devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Proveedor {IdProveedor} eliminado (soft delete)", idProveedor);
                return new { success = true, message = "Proveedor eliminado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar proveedor. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar proveedor en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar proveedor");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un proveedor por su ID
        /// </summary>
        public async Task<object> UpdateProveedorAsync(ProveedorQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_proveedor_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update");
                command.Parameters.AddWithValue("@idProveedor", query.IdProveedor);
                command.Parameters.AddWithValue("@rfc", (object?)query.Rfc ?? DBNull.Value);
                command.Parameters.AddWithValue("@razon_social", (object?)query.RazonSocial ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombreComercial", (object?)query.NombreComercial ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Update de proveedor devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Proveedor {IdProveedor} actualizado", query.IdProveedor);
                return new { success = true, message = "Proveedor actualizado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar proveedor. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar proveedor en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar proveedor");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo proveedor usando el procedimiento almacenado sp_proveedor_edit
        /// </summary>
        public async Task<object> CreateProveedorAsync(ProveedorQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_proveedor_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "create");
                command.Parameters.AddWithValue("@idProveedor", 0);
                command.Parameters.AddWithValue("@rfc", (object?)query.Rfc ?? DBNull.Value);
                command.Parameters.AddWithValue("@razon_social", (object?)query.RazonSocial ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombreComercial", (object?)query.NombreComercial ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", true);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                // Check if there's any result or error message
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Create de proveedor devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Proveedor creado correctamente");
                return new { success = true, message = "Proveedor creado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear proveedor. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear proveedor en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear proveedor");
                throw;
            }
        }
    }
}
