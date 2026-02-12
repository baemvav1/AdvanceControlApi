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
    /// Implementación del servicio de ImpuestoComision que usa los procedimientos almacenados
    /// sp_CrearImpuestoMovimiento, sp_ConsultarImpuestosMovimiento, sp_CrearComisionBancaria y sp_ConsultarComisionesBancarias
    /// </summary>
    public class ImpuestoComisionService : IImpuestoComisionService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<ImpuestoComisionService> _logger;

        public ImpuestoComisionService(DbHelper dbHelper, ILogger<ImpuestoComisionService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo impuesto de movimiento usando el procedimiento almacenado sp_CrearImpuestoMovimiento
        /// </summary>
        public async Task<object> CrearImpuestoMovimientoAsync(ImpuestoMovimientoDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_CrearImpuestoMovimiento", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idMovimiento", dto.IdMovimiento ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tipoImpuesto", (object?)dto.TipoImpuesto ?? DBNull.Value);
                command.Parameters.AddWithValue("@rfc", (object?)dto.Rfc ?? DBNull.Value);
                command.Parameters.AddWithValue("@monto", dto.Monto ?? (object)DBNull.Value);

                // Parámetro de salida para el ID
                var idOutputParam = new SqlParameter("@idImpuesto", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(idOutputParam);

                await using var reader = await command.ExecuteReaderAsync();

                int idImpuesto = 0;
                string mensaje = "Impuesto creado exitosamente";

                if (await reader.ReadAsync())
                {
                    try
                    {
                        idImpuesto = reader.GetInt32(reader.GetOrdinal("idImpuesto"));
                        mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "No se pudieron leer los campos del resultado, usando parámetro de salida");
                    }
                }

                await reader.CloseAsync();

                // Si no se obtuvo del reader, intentar del parámetro de salida
                if (idImpuesto == 0 && idOutputParam.Value != DBNull.Value)
                {
                    idImpuesto = (int)idOutputParam.Value;
                }

                _logger.LogDebug("Impuesto de movimiento creado con ID: {IdImpuesto}", idImpuesto);
                return new { success = true, idImpuesto, message = mensaje };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear impuesto de movimiento. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear impuesto de movimiento en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear impuesto de movimiento");
                throw;
            }
        }

        /// <summary>
        /// Consulta impuestos de movimiento usando el procedimiento almacenado sp_ConsultarImpuestosMovimiento
        /// </summary>
        public async Task<List<ImpuestoMovimiento>> ConsultarImpuestosMovimientoAsync(int? idMovimiento, string? tipoImpuesto)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_ConsultarImpuestosMovimiento", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idMovimiento", idMovimiento ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tipoImpuesto", (object?)tipoImpuesto ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                var impuestos = new List<ImpuestoMovimiento>();

                while (await reader.ReadAsync())
                {
                    var impuesto = new ImpuestoMovimiento
                    {
                        IdImpuesto = reader.GetInt32(reader.GetOrdinal("idImpuesto")),
                        IdMovimiento = reader.GetInt32(reader.GetOrdinal("idMovimiento")),
                        TipoImpuesto = reader.IsDBNull(reader.GetOrdinal("tipoImpuesto")) ? null : reader.GetString(reader.GetOrdinal("tipoImpuesto")),
                        Rfc = reader.IsDBNull(reader.GetOrdinal("rfc")) ? null : reader.GetString(reader.GetOrdinal("rfc")),
                        Monto = reader.GetDecimal(reader.GetOrdinal("monto")),
                        Fecha = reader.IsDBNull(reader.GetOrdinal("fecha")) ? null : reader.GetDateTime(reader.GetOrdinal("fecha")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion"))
                    };

                    impuestos.Add(impuesto);
                }

                _logger.LogDebug("Consulta de impuestos de movimiento completada. Total: {Count}", impuestos.Count);
                return impuestos;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al consultar impuestos de movimiento. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al consultar impuestos de movimiento en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar impuestos de movimiento");
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva comisión bancaria usando el procedimiento almacenado sp_CrearComisionBancaria
        /// </summary>
        public async Task<object> CrearComisionBancariaAsync(ComisionBancariaDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_CrearComisionBancaria", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idMovimiento", dto.IdMovimiento ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tipoComision", (object?)dto.TipoComision ?? DBNull.Value);
                command.Parameters.AddWithValue("@monto", dto.Monto ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@iva", dto.Iva ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@referencia", (object?)dto.Referencia ?? DBNull.Value);

                // Parámetro de salida para el ID
                var idOutputParam = new SqlParameter("@idComision", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(idOutputParam);

                await using var reader = await command.ExecuteReaderAsync();

                int idComision = 0;
                string mensaje = "Comisión bancaria creada exitosamente";

                if (await reader.ReadAsync())
                {
                    try
                    {
                        idComision = reader.GetInt32(reader.GetOrdinal("idComision"));
                        mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "No se pudieron leer los campos del resultado, usando parámetro de salida");
                    }
                }

                await reader.CloseAsync();

                // Si no se obtuvo del reader, intentar del parámetro de salida
                if (idComision == 0 && idOutputParam.Value != DBNull.Value)
                {
                    idComision = (int)idOutputParam.Value;
                }

                _logger.LogDebug("Comisión bancaria creada con ID: {IdComision}", idComision);
                return new { success = true, idComision, message = mensaje };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear comisión bancaria. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear comisión bancaria en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear comisión bancaria");
                throw;
            }
        }

        /// <summary>
        /// Consulta comisiones bancarias usando el procedimiento almacenado sp_ConsultarComisionesBancarias
        /// </summary>
        public async Task<List<ComisionBancaria>> ConsultarComisionesBancariasAsync(int? idMovimiento, string? tipoComision, DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_ConsultarComisionesBancarias", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idMovimiento", idMovimiento ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@tipoComision", (object?)tipoComision ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaInicio", fechaInicio ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@fechaFin", fechaFin ?? (object)DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                var comisiones = new List<ComisionBancaria>();

                while (await reader.ReadAsync())
                {
                    var comision = new ComisionBancaria
                    {
                        IdComision = reader.GetInt32(reader.GetOrdinal("idComision")),
                        IdMovimiento = reader.GetInt32(reader.GetOrdinal("idMovimiento")),
                        TipoComision = reader.IsDBNull(reader.GetOrdinal("tipoComision")) ? null : reader.GetString(reader.GetOrdinal("tipoComision")),
                        Monto = reader.GetDecimal(reader.GetOrdinal("monto")),
                        Iva = reader.IsDBNull(reader.GetOrdinal("iva")) ? null : reader.GetDecimal(reader.GetOrdinal("iva")),
                        Referencia = reader.IsDBNull(reader.GetOrdinal("referencia")) ? null : reader.GetString(reader.GetOrdinal("referencia")),
                        Fecha = reader.IsDBNull(reader.GetOrdinal("fecha")) ? null : reader.GetDateTime(reader.GetOrdinal("fecha")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion"))
                    };

                    comisiones.Add(comision);
                }

                _logger.LogDebug("Consulta de comisiones bancarias completada. Total: {Count}", comisiones.Count);
                return comisiones;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al consultar comisiones bancarias. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al consultar comisiones bancarias en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar comisiones bancarias");
                throw;
            }
        }
    }
}
