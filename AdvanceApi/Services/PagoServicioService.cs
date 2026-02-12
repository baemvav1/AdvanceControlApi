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
    /// Implementación del servicio de PagoServicio que usa los procedimientos almacenados
    /// sp_CrearPagoServicio y sp_ConsultarPagosServicio
    /// </summary>
    public class PagoServicioService : IPagoServicioService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<PagoServicioService> _logger;

        public PagoServicioService(DbHelper dbHelper, ILogger<PagoServicioService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo pago de servicio usando el procedimiento almacenado sp_CrearPagoServicio
        /// </summary>
        public async Task<object> CrearPagoServicioAsync(PagoServicioQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_CrearPagoServicio", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idMovimiento", query.IdMovimiento ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tipoServicio", (object?)query.TipoServicio ?? DBNull.Value);
                command.Parameters.AddWithValue("@referencia", (object?)query.Referencia ?? DBNull.Value);
                command.Parameters.AddWithValue("@monto", query.Monto ?? (object)DBNull.Value);

                // Parámetro de salida para el ID
                var idOutputParam = new SqlParameter("@idPago", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(idOutputParam);

                await using var reader = await command.ExecuteReaderAsync();

                int idPago = 0;
                string mensaje = "Pago de servicio creado exitosamente";

                if (await reader.ReadAsync())
                {
                    try
                    {
                        idPago = reader.GetInt32(reader.GetOrdinal("idPago"));
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
                if (idPago == 0 && idOutputParam.Value != DBNull.Value)
                {
                    idPago = (int)idOutputParam.Value;
                }

                _logger.LogDebug("Pago de servicio creado con ID: {IdPago}", idPago);
                return new { success = true, idPago, message = mensaje };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear pago de servicio. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear pago de servicio en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear pago de servicio");
                throw;
            }
        }

        /// <summary>
        /// Consulta pagos de servicio según los criterios especificados usando el procedimiento almacenado sp_ConsultarPagosServicio
        /// </summary>
        public async Task<List<PagoServicio>> ConsultarPagosServicioAsync(int? idMovimiento, string? tipoServicio, DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_ConsultarPagosServicio", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idMovimiento", idMovimiento ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tipoServicio", (object?)tipoServicio ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaInicio", fechaInicio ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@fechaFin", fechaFin ?? (object)DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                var pagosServicio = new List<PagoServicio>();

                while (await reader.ReadAsync())
                {
                    var pagoServicio = new PagoServicio
                    {
                        IdPago = reader.GetInt32(reader.GetOrdinal("idPago")),
                        IdMovimiento = reader.GetInt32(reader.GetOrdinal("idMovimiento")),
                        TipoServicio = reader.GetString(reader.GetOrdinal("tipoServicio")),
                        Referencia = reader.IsDBNull(reader.GetOrdinal("referencia")) ? null : reader.GetString(reader.GetOrdinal("referencia")),
                        Monto = reader.GetDecimal(reader.GetOrdinal("monto")),
                        Fecha = reader.IsDBNull(reader.GetOrdinal("fecha")) ? null : reader.GetDateTime(reader.GetOrdinal("fecha")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion"))
                    };

                    pagosServicio.Add(pagoServicio);
                }

                _logger.LogDebug("Consulta de pagos de servicio completada. Total: {Count}", pagosServicio.Count);
                return pagosServicio;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al consultar pagos de servicio. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al consultar pagos de servicio en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar pagos de servicio");
                throw;
            }
        }
    }
}
