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
        public async Task<CrearTransferenciaSPEIResult> CrearTransferenciaSPEIAsync(TransferenciaSPEICreateDto dto)
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
                command.Parameters.AddWithValue("@hora", (object?)dto.Hora ?? DBNull.Value);
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
                return new CrearTransferenciaSPEIResult 
                { 
                    Success = true, 
                    IdTransferencia = idTransferencia, 
                    Message = mensaje 
                };
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
                command.Parameters.AddWithValue("@idMovimiento", (object?)query.IdMovimiento ?? DBNull.Value);
                command.Parameters.AddWithValue("@tipoTransferencia", (object?)query.TipoTransferencia ?? DBNull.Value);
                command.Parameters.AddWithValue("@claveRastreo", (object?)query.ClaveRastreo ?? DBNull.Value);
                command.Parameters.AddWithValue("@rfcEmisor", (object?)query.RfcEmisor ?? DBNull.Value);
                command.Parameters.AddWithValue("@rfcDestinatario", (object?)query.RfcDestinatario ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaInicio", (object?)query.FechaInicio ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaFin", (object?)query.FechaFin ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                var transferencias = new List<TransferenciaSPEI>();

                // Cache ordinal positions for better performance
                int idTransferenciaOrdinal = -1;
                int idMovimientoOrdinal = -1;
                int tipoTransferenciaOrdinal = -1;
                int bancoClaveOrdinal = -1;
                int bancoNombreOrdinal = -1;
                int cuentaOrigenOrdinal = -1;
                int cuentaDestinoOrdinal = -1;
                int nombreEmisorOrdinal = -1;
                int nombreDestinatarioOrdinal = -1;
                int rfcEmisorOrdinal = -1;
                int rfcDestinatarioOrdinal = -1;
                int claveRastreoOrdinal = -1;
                int conceptoOrdinal = -1;
                int horaOrdinal = -1;
                int montoOrdinal = -1;
                int fechaOrdinal = -1;
                int referenciaOrdinal = -1;

                bool ordinalsInitialized = false;

                while (await reader.ReadAsync())
                {
                    // Initialize ordinals on first row
                    if (!ordinalsInitialized)
                    {
                        idTransferenciaOrdinal = reader.GetOrdinal("idTransferencia");
                        idMovimientoOrdinal = reader.GetOrdinal("idMovimiento");
                        tipoTransferenciaOrdinal = reader.GetOrdinal("tipoTransferencia");
                        bancoClaveOrdinal = reader.GetOrdinal("bancoClave");
                        bancoNombreOrdinal = reader.GetOrdinal("bancoNombre");
                        cuentaOrigenOrdinal = reader.GetOrdinal("cuentaOrigen");
                        cuentaDestinoOrdinal = reader.GetOrdinal("cuentaDestino");
                        nombreEmisorOrdinal = reader.GetOrdinal("nombreEmisor");
                        nombreDestinatarioOrdinal = reader.GetOrdinal("nombreDestinatario");
                        rfcEmisorOrdinal = reader.GetOrdinal("rfcEmisor");
                        rfcDestinatarioOrdinal = reader.GetOrdinal("rfcDestinatario");
                        claveRastreoOrdinal = reader.GetOrdinal("claveRastreo");
                        conceptoOrdinal = reader.GetOrdinal("concepto");
                        horaOrdinal = reader.GetOrdinal("hora");
                        montoOrdinal = reader.GetOrdinal("monto");
                        fechaOrdinal = reader.GetOrdinal("fecha");
                        referenciaOrdinal = reader.GetOrdinal("referencia");
                        ordinalsInitialized = true;
                    }

                    var transferencia = new TransferenciaSPEI
                    {
                        IdTransferencia = reader.GetInt32(idTransferenciaOrdinal),
                        IdMovimiento = reader.GetInt32(idMovimientoOrdinal),
                        TipoTransferencia = reader.IsDBNull(tipoTransferenciaOrdinal) ? string.Empty : reader.GetString(tipoTransferenciaOrdinal),
                        BancoClave = reader.IsDBNull(bancoClaveOrdinal) ? null : reader.GetString(bancoClaveOrdinal),
                        BancoNombre = reader.IsDBNull(bancoNombreOrdinal) ? null : reader.GetString(bancoNombreOrdinal),
                        CuentaOrigen = reader.IsDBNull(cuentaOrigenOrdinal) ? null : reader.GetString(cuentaOrigenOrdinal),
                        CuentaDestino = reader.IsDBNull(cuentaDestinoOrdinal) ? null : reader.GetString(cuentaDestinoOrdinal),
                        NombreEmisor = reader.IsDBNull(nombreEmisorOrdinal) ? null : reader.GetString(nombreEmisorOrdinal),
                        NombreDestinatario = reader.IsDBNull(nombreDestinatarioOrdinal) ? null : reader.GetString(nombreDestinatarioOrdinal),
                        RfcEmisor = reader.IsDBNull(rfcEmisorOrdinal) ? null : reader.GetString(rfcEmisorOrdinal),
                        RfcDestinatario = reader.IsDBNull(rfcDestinatarioOrdinal) ? null : reader.GetString(rfcDestinatarioOrdinal),
                        ClaveRastreo = reader.IsDBNull(claveRastreoOrdinal) ? null : reader.GetString(claveRastreoOrdinal),
                        Concepto = reader.IsDBNull(conceptoOrdinal) ? null : reader.GetString(conceptoOrdinal),
                        Hora = reader.IsDBNull(horaOrdinal) ? null : reader.GetTimeSpan(horaOrdinal),
                        Monto = reader.GetDecimal(montoOrdinal),
                        Fecha = reader.IsDBNull(fechaOrdinal) ? null : reader.GetDateTime(fechaOrdinal),
                        Referencia = reader.IsDBNull(referenciaOrdinal) ? null : reader.GetString(referenciaOrdinal)
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
