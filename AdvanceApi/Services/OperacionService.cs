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
    /// Implementación del servicio de operaciones que usa el procedimiento almacenado sp_OperacionEdit
    /// </summary>
    public class OperacionService : IOperacionService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<OperacionService> _logger;

        public OperacionService(DbHelper dbHelper, ILogger<OperacionService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene operaciones usando el procedimiento almacenado sp_OperacionEdit
        /// </summary>
        public async Task<List<OperacionDetalle>> GetOperacionesAsync(OperacionQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var operaciones = new List<OperacionDetalle>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_OperacionEdit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", "select");
                command.Parameters.AddWithValue("@idOperacion", 0);
                command.Parameters.AddWithValue("@idTipo", query.IdTipo);
                command.Parameters.AddWithValue("@idCliente", query.IdCliente);
                command.Parameters.AddWithValue("@idEquipo", query.IdEquipo);
                command.Parameters.AddWithValue("@idAtiende", query.IdAtiende);
                command.Parameters.AddWithValue("@monto", 0);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? "");
                command.Parameters.AddWithValue("@fechaFinal", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var operacion = new OperacionDetalle
                    {
                        IdOperacion = reader.GetInt32(reader.GetOrdinal("idOperacion")),
                        IdTipo = reader.IsDBNull(reader.GetOrdinal("idTipo")) ? null : reader.GetInt32(reader.GetOrdinal("idTipo")),
                        RazonSocial = reader.IsDBNull(reader.GetOrdinal("razon_social")) ? null : reader.GetString(reader.GetOrdinal("razon_social")),
                        Identificador = reader.IsDBNull(reader.GetOrdinal("identificador")) ? null : reader.GetString(reader.GetOrdinal("identificador")),
                        Atiende = reader.IsDBNull(reader.GetOrdinal("Atiende")) ? null : reader.GetString(reader.GetOrdinal("Atiende")),
                        IdAtiende =  reader.IsDBNull(reader.GetOrdinal("idAtiende")) ? null : reader.GetInt32(reader.GetOrdinal("idAtiende")),
                        Monto = reader.IsDBNull(reader.GetOrdinal("monto")) ? null : reader.GetDouble(reader.GetOrdinal("monto")),
                        Nota = reader.IsDBNull(reader.GetOrdinal("nota")) ? null : reader.GetString(reader.GetOrdinal("nota")),
                        FechaInicio = reader.IsDBNull(reader.GetOrdinal("fechaInicio")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaInicio")),
                        FechaFinal = reader.IsDBNull(reader.GetOrdinal("fechaFinal")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaFinal")),
                        Finalizado = reader.IsDBNull(reader.GetOrdinal("finalizado")) ? null : reader.GetBoolean(reader.GetOrdinal("finalizado"))
                    };

                    operaciones.Add(operacion);
                }

                _logger.LogDebug("Se obtuvieron {Count} operaciones", operaciones.Count);

                return operaciones;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener operaciones. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener operaciones de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener operaciones");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) una operación
        /// </summary>
        public async Task<object> DeleteOperacionAsync(int idOperacion)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_OperacionEdit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@idOperacion", idOperacion);
                command.Parameters.AddWithValue("@idTipo", 0);
                command.Parameters.AddWithValue("@idCliente", 0);
                command.Parameters.AddWithValue("@idEquipo", 0);
                command.Parameters.AddWithValue("@idAtiende", 0);
                command.Parameters.AddWithValue("@monto", 0);
                command.Parameters.AddWithValue("@nota", DBNull.Value);
                command.Parameters.AddWithValue("@fechaFinal", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Delete de operación devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Operación eliminada con ID {IdOperacion}", idOperacion);
                return new { success = true, message = "Operación eliminada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar operación. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar operación en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar operación");
                throw;
            }
        }
    }
}
