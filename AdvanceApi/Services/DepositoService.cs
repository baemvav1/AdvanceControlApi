using AdvanceApi.DTOs;
using AdvanceApi.Helpers;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Implementación del servicio de Deposito que usa los procedimientos almacenados
    /// sp_CrearDeposito y sp_ConsultarDepositos
    /// </summary>
    public class DepositoService : IDepositoService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<DepositoService> _logger;

        public DepositoService(DbHelper dbHelper, ILogger<DepositoService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo depósito usando el procedimiento almacenado sp_CrearDeposito
        /// </summary>
        public async Task<object> CrearDepositoAsync(DepositoQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_CrearDeposito", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idMovimiento", query.IdMovimiento ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tipoDeposito", (object?)query.TipoDeposito ?? DBNull.Value);
                command.Parameters.AddWithValue("@referencia", (object?)query.Referencia ?? DBNull.Value);
                command.Parameters.AddWithValue("@monto", query.Monto ?? (object)DBNull.Value);

                // Parámetro de salida para el ID
                var idOutputParam = new SqlParameter("@idDeposito", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(idOutputParam);

                await using var reader = await command.ExecuteReaderAsync();

                int idDeposito = 0;
                string mensaje = "Depósito creado exitosamente";

                if (await reader.ReadAsync())
                {
                    try
                    {
                        idDeposito = reader.GetInt32(reader.GetOrdinal("idDeposito"));
                        mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));
                    }
                    catch (Exception ex)
                    {
                        // Si no se pueden leer los campos, usar el parámetro de salida
                        _logger.LogDebug(ex, "No se pudieron leer los campos del resultado, usando parámetro de salida");
                    }
                }

                await reader.CloseAsync();

                // Si no se obtuvo del reader, intentar del parámetro de salida
                if (idDeposito == 0 && idOutputParam.Value != DBNull.Value)
                {
                    idDeposito = (int)idOutputParam.Value;
                }

                _logger.LogDebug("Depósito creado con ID: {IdDeposito}", idDeposito);
                return new { success = true, idDeposito, message = mensaje };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear depósito. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear depósito en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear depósito");
                throw;
            }
        }

        /// <summary>
        /// Consulta depósitos según los criterios especificados usando el procedimiento almacenado sp_ConsultarDepositos
        /// </summary>
        public async Task<List<object>> ConsultarDepositosAsync(int? idMovimiento, string? tipoDeposito, DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_ConsultarDepositos", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idMovimiento", idMovimiento ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tipoDeposito", (object?)tipoDeposito ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaInicio", fechaInicio ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@fechaFin", fechaFin ?? (object)DBNull.Value);

                var depositos = new List<object>();

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var deposito = new
                    {
                        idDeposito = reader.GetInt32(reader.GetOrdinal("idDeposito")),
                        idMovimiento = reader.GetInt32(reader.GetOrdinal("idMovimiento")),
                        tipoDeposito = reader.IsDBNull(reader.GetOrdinal("tipoDeposito")) ? null : reader.GetString(reader.GetOrdinal("tipoDeposito")),
                        referencia = reader.IsDBNull(reader.GetOrdinal("referencia")) ? null : reader.GetString(reader.GetOrdinal("referencia")),
                        monto = reader.GetDecimal(reader.GetOrdinal("monto")),
                        fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                        descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion"))
                    };
                    depositos.Add(deposito);
                }

                _logger.LogDebug("Se consultaron {Count} depósitos", depositos.Count);
                return depositos;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al consultar depósitos. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al consultar depósitos en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar depósitos");
                throw;
            }
        }
    }
}
