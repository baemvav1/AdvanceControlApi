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
    /// Implementación del servicio de operaciones que usa el procedimiento almacenado sp_operacion_select
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
        /// Obtiene operaciones usando el procedimiento almacenado sp_operacion_select
        /// </summary>
        public async Task<List<Operacion>> GetOperacionesAsync(OperacionQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var operaciones = new List<Operacion>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_operacion_select", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idtipo", (object?)query.IdTipo ?? DBNull.Value);
                command.Parameters.AddWithValue("@idcliente", (object?)query.IdCliente ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", (object?)query.Estatus ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var operacion = new Operacion
                    {
                        IdOperacion = reader.GetInt32(reader.GetOrdinal("idOperacion")),
                        Concepto = reader.IsDBNull(reader.GetOrdinal("concepto")) ? null : reader.GetString(reader.GetOrdinal("concepto")),
                        IdCliente = reader.IsDBNull(reader.GetOrdinal("idCliente")) ? null : reader.GetInt32(reader.GetOrdinal("idCliente")),
                        Monto = reader.IsDBNull(reader.GetOrdinal("monto")) ? null : reader.GetDouble(reader.GetOrdinal("monto")),
                        Abono = reader.IsDBNull(reader.GetOrdinal("abono")) ? null : reader.GetDouble(reader.GetOrdinal("abono")),
                        Restante = reader.IsDBNull(reader.GetOrdinal("restante")) ? null : reader.GetDouble(reader.GetOrdinal("restante")),
                        Nota = reader.IsDBNull(reader.GetOrdinal("nota")) ? null : reader.GetString(reader.GetOrdinal("nota")),
                        Estatus = reader.IsDBNull(reader.GetOrdinal("estatus")) ? null : reader.GetBoolean(reader.GetOrdinal("estatus")),
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
    }
}
