using AdvanceApi.DTOs;
using AdvanceApi.Helpers;
using Clases;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Implementación del servicio de TransferenciaSPEI que usa los procedimientos almacenados
    /// sp_CrearTransferenciaSPEI y sp_ConsultarTransferenciasSPEI
    /// </summary>
    public class TransferenciaSPEIService : ITransferenciaSPEIService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<TransferenciaSPEIService> _logger;

        public TransferenciaSPEIService(DbHelper dbHelper, ILogger<TransferenciaSPEIService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea una nueva transferencia SPEI usando el procedimiento almacenado sp_CrearTransferenciaSPEI
        /// </summary>
        public async Task<object> CrearTransferenciaSPEIAsync(TransferenciaSPEICreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_CrearTransferenciaSPEI", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idMovimiento", dto.IdMovimiento);
                command.Parameters.AddWithValue("@tipoTransferencia", (object?)dto.TipoTransferencia ?? DBNull.Value);
                command.Parameters.AddWithValue("@bancoClave", (object?)dto.BancoClave ?? DBNull.Value);
                command.Parameters.AddWithValue("@bancoNombre", (object?)dto.BancoNombre ?? DBNull.Value);
                command.Parameters.AddWithValue("@cuentaOrigen", (object?)dto.CuentaOrigen ?? DBNull.Value);
                command.Parameters.AddWithValue("@cuentaDestino", (object?)dto.CuentaDestino ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombreEmisor", (object?)dto.NombreEmisor ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombreDestinatario", (object?)dto.NombreDestinatario ?? DBNull.Value);
                command.Parameters.AddWithValue("@rfcEmisor", (object?)dto.RfcEmisor ?? DBNull.Value);
                command.Parameters.AddWithValue("@rfcDestinatario", (object?)dto.RfcDestinatario ?? DBNull.Value);
                command.Parameters.AddWithValue("@claveRastreo", (object?)dto.ClaveRastreo ?? DBNull.Value);
                command.Parameters.AddWithValue("@concepto", (object?)dto.Concepto ?? DBNull.Value);
                command.Parameters.AddWithValue("@hora", dto.Hora ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@monto", dto.Monto);

                // Parámetro de salida para el ID
                var idOutputParam = new SqlParameter("@idTransferencia", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(idOutputParam);

                await using var reader = await command.ExecuteReaderAsync();

                int idTransferencia = 0;
                string mensaje = "Transferencia SPEI creada exitosamente";

                if (await reader.ReadAsync())
                {
                    try
                    {
                        idTransferencia = reader.GetInt32(reader.GetOrdinal("idTransferencia"));
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
                if (idTransferencia == 0 && idOutputParam.Value != DBNull.Value)
                {
                    idTransferencia = (int)idOutputParam.Value;
                }

                _logger.LogDebug("Transferencia SPEI creada con ID: {IdTransferencia}", idTransferencia);
                return new { success = true, idTransferencia, message = mensaje };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear transferencia SPEI. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear transferencia SPEI en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear transferencia SPEI");
                throw;
            }
        }

        /// <summary>
        /// Consulta transferencias SPEI según los criterios especificados usando el procedimiento almacenado sp_ConsultarTransferenciasSPEI
        /// </summary>
        public async Task<List<TransferenciaSPEI>> ConsultarTransferenciasSPEIAsync(TransferenciaSPEIQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_ConsultarTransferenciasSPEI", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idMovimiento", query.IdMovimiento ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tipoTransferencia", (object?)query.TipoTransferencia ?? DBNull.Value);
                command.Parameters.AddWithValue("@claveRastreo", (object?)query.ClaveRastreo ?? DBNull.Value);
                command.Parameters.AddWithValue("@rfcEmisor", (object?)query.RfcEmisor ?? DBNull.Value);
                command.Parameters.AddWithValue("@rfcDestinatario", (object?)query.RfcDestinatario ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaInicio", query.FechaInicio ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@fechaFin", query.FechaFin ?? (object)DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                var transferencias = new List<TransferenciaSPEI>();

                while (await reader.ReadAsync())
                {
                    var transferencia = new TransferenciaSPEI
                    {
                        IdTransferencia = reader.GetInt32(reader.GetOrdinal("idTransferencia")),
                        IdMovimiento = reader.GetInt32(reader.GetOrdinal("idMovimiento")),
                        TipoTransferencia = reader.GetString(reader.GetOrdinal("tipoTransferencia")),
                        BancoClave = reader.IsDBNull(reader.GetOrdinal("bancoClave")) ? null : reader.GetString(reader.GetOrdinal("bancoClave")),
                        BancoNombre = reader.IsDBNull(reader.GetOrdinal("bancoNombre")) ? null : reader.GetString(reader.GetOrdinal("bancoNombre")),
                        CuentaOrigen = reader.IsDBNull(reader.GetOrdinal("cuentaOrigen")) ? null : reader.GetString(reader.GetOrdinal("cuentaOrigen")),
                        CuentaDestino = reader.IsDBNull(reader.GetOrdinal("cuentaDestino")) ? null : reader.GetString(reader.GetOrdinal("cuentaDestino")),
                        NombreEmisor = reader.IsDBNull(reader.GetOrdinal("nombreEmisor")) ? null : reader.GetString(reader.GetOrdinal("nombreEmisor")),
                        NombreDestinatario = reader.IsDBNull(reader.GetOrdinal("nombreDestinatario")) ? null : reader.GetString(reader.GetOrdinal("nombreDestinatario")),
                        RfcEmisor = reader.IsDBNull(reader.GetOrdinal("rfcEmisor")) ? null : reader.GetString(reader.GetOrdinal("rfcEmisor")),
                        RfcDestinatario = reader.IsDBNull(reader.GetOrdinal("rfcDestinatario")) ? null : reader.GetString(reader.GetOrdinal("rfcDestinatario")),
                        ClaveRastreo = reader.IsDBNull(reader.GetOrdinal("claveRastreo")) ? null : reader.GetString(reader.GetOrdinal("claveRastreo")),
                        Concepto = reader.IsDBNull(reader.GetOrdinal("concepto")) ? null : reader.GetString(reader.GetOrdinal("concepto")),
                        Hora = reader.IsDBNull(reader.GetOrdinal("hora")) ? null : reader.GetTimeSpan(reader.GetOrdinal("hora")),
                        Monto = reader.GetDecimal(reader.GetOrdinal("monto")),
                        Fecha = reader.IsDBNull(reader.GetOrdinal("fecha")) ? null : reader.GetDateTime(reader.GetOrdinal("fecha")),
                        Referencia = reader.IsDBNull(reader.GetOrdinal("referencia")) ? null : reader.GetString(reader.GetOrdinal("referencia"))
                    };

                    transferencias.Add(transferencia);
                }

                _logger.LogDebug("Consulta de transferencias SPEI completada. Total: {Count}", transferencias.Count);
                return transferencias;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al consultar transferencias SPEI. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al consultar transferencias SPEI en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar transferencias SPEI");
                throw;
            }
        }
    }
}
