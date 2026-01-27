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
    /// Implementación del servicio de cargos que usa el procedimiento almacenado sp_cargo_edit
    /// </summary>
    public class CargoService : ICargoService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<CargoService> _logger;

        public CargoService(DbHelper dbHelper, ILogger<CargoService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene cargos usando el procedimiento almacenado sp_cargo_edit
        /// </summary>
        public async Task<List<Cargo>> GetCargosAsync(CargoEditDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var cargos = new List<Cargo>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_cargo_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", "select");
                command.Parameters.AddWithValue("@idCargo", query.IdCargo);
                command.Parameters.AddWithValue("@idTipoCargo", (object?)query.IdTipoCargo ?? DBNull.Value);
                command.Parameters.AddWithValue("@idOperacion", (object?)query.IdOperacion ?? DBNull.Value);
                command.Parameters.AddWithValue("@idRelacionCargo", (object?)query.IdRelacionCargo ?? DBNull.Value);
                command.Parameters.AddWithValue("@monto", (object?)query.Monto ?? DBNull.Value);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var cargo = new Cargo
                    {
                        IdCargo = reader.GetInt32(reader.GetOrdinal("idCargo")),
                        IdTipoCargo = reader.IsDBNull(reader.GetOrdinal("idTipoCargo")) ? null : reader.GetInt32(reader.GetOrdinal("idTipoCargo")),
                        IdOperacion = reader.IsDBNull(reader.GetOrdinal("idOperacion")) ? null : reader.GetInt32(reader.GetOrdinal("idOperacion")),
                        IdRelacionCargo = reader.IsDBNull(reader.GetOrdinal("idRelacionCargo")) ? null : reader.GetInt32(reader.GetOrdinal("idRelacionCargo")),
                        Monto = reader.IsDBNull(reader.GetOrdinal("monto")) ? null : reader.GetDouble(reader.GetOrdinal("monto")),
                        Nota = reader.IsDBNull(reader.GetOrdinal("nota")) ? null : reader.GetString(reader.GetOrdinal("nota"))
                    };

                    cargos.Add(cargo);
                }

                _logger.LogDebug("Se obtuvieron {Count} cargos", cargos.Count);

                return cargos;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener cargos. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener cargos de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener cargos");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo cargo usando el procedimiento almacenado sp_cargo_edit
        /// </summary>
        public async Task<object> CreateCargoAsync(CargoEditDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_cargo_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "create");
                command.Parameters.AddWithValue("@idCargo", 0);
                command.Parameters.AddWithValue("@idTipoCargo", (object?)query.IdTipoCargo ?? DBNull.Value);
                command.Parameters.AddWithValue("@idOperacion", (object?)query.IdOperacion ?? DBNull.Value);
                command.Parameters.AddWithValue("@idRelacionCargo", (object?)query.IdRelacionCargo ?? DBNull.Value);
                command.Parameters.AddWithValue("@monto", (object?)query.Monto ?? DBNull.Value);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        
                        // Check if it's an error message
                        if (result.Contains("Error"))
                        {
                            _logger.LogWarning("Create de cargo devolvió error: {Result}", result);
                            return new { success = false, message = result };
                        }
                        
                        var idCargo = reader.IsDBNull(reader.GetOrdinal("idCargo")) ? 0 : reader.GetInt32(reader.GetOrdinal("idCargo"));
                        _logger.LogDebug("Create de cargo devolvió: {Result}, idCargo: {IdCargo}", result, idCargo);
                        return new { success = true, message = result, idCargo = idCargo };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Cargo creado correctamente");
                return new { success = true, message = "Cargo creado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear cargo. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear cargo en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear cargo");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un cargo por su ID
        /// </summary>
        public async Task<object> UpdateCargoAsync(CargoEditDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_cargo_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update");
                command.Parameters.AddWithValue("@idCargo", query.IdCargo);
                command.Parameters.AddWithValue("@idTipoCargo", (object?)query.IdTipoCargo ?? DBNull.Value);
                command.Parameters.AddWithValue("@idOperacion", DBNull.Value);
                command.Parameters.AddWithValue("@idRelacionCargo", (object?)query.IdRelacionCargo ?? DBNull.Value);
                command.Parameters.AddWithValue("@monto", (object?)query.Monto ?? DBNull.Value);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Update de cargo devolvió: {Result}", result);
                        
                        // Check if it's an error message
                        if (result.Contains("Invalido") || result.Contains("No se encontró"))
                        {
                            return new { success = false, message = result };
                        }
                        
                        return new { success = true, message = result };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Cargo {IdCargo} actualizado", query.IdCargo);
                return new { success = true, message = "Cargo actualizado exitosamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar cargo. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar cargo en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar cargo");
                throw;
            }
        }

        /// <summary>
        /// Elimina un cargo por su ID
        /// </summary>
        public async Task<object> DeleteCargoAsync(int idCargo)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_cargo_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@idCargo", idCargo);
                command.Parameters.AddWithValue("@idTipoCargo", DBNull.Value);
                command.Parameters.AddWithValue("@idOperacion", DBNull.Value);
                command.Parameters.AddWithValue("@idRelacionCargo", DBNull.Value);
                command.Parameters.AddWithValue("@monto", DBNull.Value);
                command.Parameters.AddWithValue("@nota", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Delete de cargo devolvió: {Result}", result);
                        
                        // Check if it's an error message
                        if (result.Contains("Invalido") || result.Contains("No se encontró"))
                        {
                            return new { success = false, message = result };
                        }
                        
                        return new { success = true, message = result };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Cargo {IdCargo} eliminado", idCargo);
                return new { success = true, message = "Cargo eliminado exitosamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar cargo. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar cargo en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar cargo");
                throw;
            }
        }
    }
}
