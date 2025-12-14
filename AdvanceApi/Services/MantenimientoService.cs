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
    /// Implementación del servicio de mantenimiento que usa el procedimiento almacenado sp_MatenimientoEdit
    /// </summary>
    public class MantenimientoService : IMantenimientoService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<MantenimientoService> _logger;

        public MantenimientoService(DbHelper dbHelper, ILogger<MantenimientoService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene mantenimientos usando el procedimiento almacenado sp_MatenimientoEdit
        /// </summary>
        public async Task<List<Mantenimiento>> GetMantenimientosAsync(MantenimientoQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var mantenimientos = new List<Mantenimiento>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_MatenimientoEdit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", "select");
                command.Parameters.AddWithValue("@identificador", (object?)query.Identificador ?? DBNull.Value);
                command.Parameters.AddWithValue("@idCliente", query.IdCliente);
                command.Parameters.AddWithValue("@nota", DBNull.Value);
                command.Parameters.AddWithValue("@idMantenimiento", DBNull.Value);
                command.Parameters.AddWithValue("@idEquipo", DBNull.Value);
                command.Parameters.AddWithValue("@idTipoMantenimiento", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var mantenimiento = new Mantenimiento
                    {
                        IdMantenimiento = reader.IsDBNull(0) ? null : reader.GetInt32(0),
                        TipoMantenimiento = reader.IsDBNull(1) ? null : reader.GetString(1),
                        NombreComercial = reader.IsDBNull(2) ? null : reader.GetString(2),
                        RazonSocial = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Nota = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Identificador = reader.IsDBNull(5) ? null : reader.GetString(5)
                    };

                    mantenimientos.Add(mantenimiento);
                }

                _logger.LogDebug("Se obtuvieron {Count} mantenimientos", mantenimientos.Count);

                return mantenimientos;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener mantenimientos. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener mantenimientos de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener mantenimientos");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo mantenimiento
        /// </summary>
        public async Task<object> CreateMantenimientoAsync(MantenimientoQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_MatenimientoEdit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "put");
                command.Parameters.AddWithValue("@identificador", DBNull.Value);
                command.Parameters.AddWithValue("@idCliente", query.IdCliente);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);
                command.Parameters.AddWithValue("@idMantenimiento", DBNull.Value);
                command.Parameters.AddWithValue("@idEquipo", (object?)query.IdEquipo ?? DBNull.Value);
                command.Parameters.AddWithValue("@idTipoMantenimiento", (object?)query.IdTipoMantenimiento ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Creación de mantenimiento devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Mantenimiento creado para cliente {IdCliente} y equipo {IdEquipo}", query.IdCliente, query.IdEquipo);
                return new { success = true, message = "Mantenimiento creado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear mantenimiento. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear mantenimiento en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear mantenimiento");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) un mantenimiento
        /// </summary>
        public async Task<object> DeleteMantenimientoAsync(int idMantenimiento)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_MatenimientoEdit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@identificador", DBNull.Value);
                command.Parameters.AddWithValue("@idCliente", 0);
                command.Parameters.AddWithValue("@nota", DBNull.Value);
                command.Parameters.AddWithValue("@idMantenimiento", idMantenimiento);
                command.Parameters.AddWithValue("@idEquipo", DBNull.Value);
                command.Parameters.AddWithValue("@idTipoMantenimiento", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Delete de mantenimiento devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Mantenimiento eliminado con ID {IdMantenimiento}", idMantenimiento);
                return new { success = true, message = "Mantenimiento eliminado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar mantenimiento. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar mantenimiento en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar mantenimiento");
                throw;
            }
        }

        /// <summary>
        /// Actualiza el estado de atendido de un mantenimiento
        /// </summary>
        public async Task<object> UpdateAtendidoAsync(int idMantenimiento, int idAtendio)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_MatenimientoEdit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update_atendido");
                command.Parameters.AddWithValue("@identificador", DBNull.Value);
                command.Parameters.AddWithValue("@idCliente", 0);
                command.Parameters.AddWithValue("@nota", DBNull.Value);
                command.Parameters.AddWithValue("@idMantenimiento", idMantenimiento);
                command.Parameters.AddWithValue("@idEquipo", DBNull.Value);
                command.Parameters.AddWithValue("@idTipoMantenimiento", DBNull.Value);
                command.Parameters.AddWithValue("@atendido", true);
                command.Parameters.AddWithValue("@idAtendio", idAtendio);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Update atendido de mantenimiento devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Mantenimiento {IdMantenimiento} marcado como atendido por usuario {IdAtendio}", idMantenimiento, idAtendio);
                return new { success = true, message = "Mantenimiento marcado como atendido correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar estado atendido de mantenimiento. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar estado atendido de mantenimiento en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar estado atendido de mantenimiento");
                throw;
            }
        }
    }
}
