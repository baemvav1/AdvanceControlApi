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
    /// Implementación del servicio de Movimiento que usa los procedimientos almacenados
    /// sp_CrearMovimiento, sp_EditarMovimiento y sp_ConsultarMovimientos
    /// </summary>
    public class MovimientoService : IMovimientoService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<MovimientoService> _logger;

        public MovimientoService(DbHelper dbHelper, ILogger<MovimientoService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo movimiento usando el procedimiento almacenado sp_CrearMovimiento
        /// </summary>
        public async Task<object> CrearMovimientoAsync(MovimientoQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_CrearMovimiento", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idEstadoCuenta", query.IdEstadoCuenta ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@fecha", query.Fecha ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@referencia", (object?)query.Referencia ?? DBNull.Value);
                command.Parameters.AddWithValue("@cargo", query.Cargo ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@abono", query.Abono ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@saldo", query.Saldo ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tipoOperacion", (object?)query.TipoOperacion ?? DBNull.Value);

                // Parámetro de salida para el ID
                var idOutputParam = new SqlParameter("@idMovimiento", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(idOutputParam);

                await using var reader = await command.ExecuteReaderAsync();

                int idMovimiento = 0;
                string mensaje = "Movimiento creado exitosamente";

                if (await reader.ReadAsync())
                {
                    try
                    {
                        idMovimiento = reader.GetInt32(reader.GetOrdinal("idMovimiento"));
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
                if (idMovimiento == 0 && idOutputParam.Value != DBNull.Value)
                {
                    idMovimiento = (int)idOutputParam.Value;
                }

                _logger.LogDebug("Movimiento creado con ID: {IdMovimiento}", idMovimiento);
                return new { success = true, idMovimiento, message = mensaje };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear movimiento. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear movimiento en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear movimiento");
                throw;
            }
        }

        /// <summary>
        /// Edita un movimiento existente usando el procedimiento almacenado sp_EditarMovimiento
        /// </summary>
        public async Task<object> EditarMovimientoAsync(MovimientoQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (!query.IdMovimiento.HasValue || query.IdMovimiento.Value <= 0)
                throw new ArgumentException("IdMovimiento es requerido para editar", nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_EditarMovimiento", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idMovimiento", query.IdMovimiento.Value);
                command.Parameters.AddWithValue("@fecha", query.Fecha ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@referencia", (object?)query.Referencia ?? DBNull.Value);
                command.Parameters.AddWithValue("@cargo", query.Cargo ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@abono", query.Abono ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@saldo", query.Saldo ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tipoOperacion", (object?)query.TipoOperacion ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                string mensaje = "Movimiento actualizado exitosamente";

                if (await reader.ReadAsync())
                {
                    try
                    {
                        mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));
                    }
                    catch (Exception ex)
                    {
                        // Si no se puede leer el mensaje, usar el mensaje por defecto
                        _logger.LogDebug(ex, "No se pudo leer el mensaje del resultado, usando mensaje por defecto");
                    }
                }

                _logger.LogDebug("Movimiento actualizado con ID: {IdMovimiento}", query.IdMovimiento.Value);
                return new { success = true, message = mensaje };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar movimiento. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar movimiento en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar movimiento");
                throw;
            }
        }

        /// <summary>
        /// Consulta movimientos según los criterios especificados usando el procedimiento almacenado sp_ConsultarMovimientos
        /// </summary>
        public async Task<List<Movimiento>> ConsultarMovimientosAsync(int? idEstadoCuenta, DateTime? fechaInicio, DateTime? fechaFin, string? tipoOperacion)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_ConsultarMovimientos", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idEstadoCuenta", idEstadoCuenta ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@fechaInicio", fechaInicio ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@fechaFin", fechaFin ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tipoOperacion", (object?)tipoOperacion ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                var movimientos = new List<Movimiento>();

                while (await reader.ReadAsync())
                {
                    var movimiento = new Movimiento
                    {
                        IdMovimiento = reader.GetInt32(reader.GetOrdinal("idMovimiento")),
                        IdEstadoCuenta = reader.GetInt32(reader.GetOrdinal("idEstadoCuenta")),
                        Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                        Referencia = reader.IsDBNull(reader.GetOrdinal("referencia")) ? null : reader.GetString(reader.GetOrdinal("referencia")),
                        Cargo = reader.IsDBNull(reader.GetOrdinal("cargo")) ? null : reader.GetDecimal(reader.GetOrdinal("cargo")),
                        Abono = reader.IsDBNull(reader.GetOrdinal("abono")) ? null : reader.GetDecimal(reader.GetOrdinal("abono")),
                        Saldo = reader.GetDecimal(reader.GetOrdinal("saldo")),
                        TipoOperacion = reader.IsDBNull(reader.GetOrdinal("tipoOperacion")) ? null : reader.GetString(reader.GetOrdinal("tipoOperacion")),
                        FechaCarga = reader.IsDBNull(reader.GetOrdinal("fechaCarga")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaCarga")),
                        NumeroCuenta = reader.IsDBNull(reader.GetOrdinal("numeroCuenta")) ? null : reader.GetString(reader.GetOrdinal("numeroCuenta")),
                        Clabe = reader.IsDBNull(reader.GetOrdinal("clabe")) ? null : reader.GetString(reader.GetOrdinal("clabe"))
                    };

                    movimientos.Add(movimiento);
                }

                _logger.LogDebug("Consulta de movimientos completada. Total: {Count}", movimientos.Count);
                return movimientos;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al consultar movimientos. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al consultar movimientos en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar movimientos");
                throw;
            }
        }
    }
}
