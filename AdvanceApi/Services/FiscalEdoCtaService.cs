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
    /// Implementación del servicio de Fiscal Estado de Cuenta que usa los procedimientos almacenados
    /// sp_CrearTimbreFiscal, sp_ConsultarTimbresFiscales, sp_CrearComplementoFiscal y sp_ConsultarComplementosFiscales
    /// </summary>
    public class FiscalEdoCtaService : IFiscalEdoCtaService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<FiscalEdoCtaService> _logger;

        public FiscalEdoCtaService(DbHelper dbHelper, ILogger<FiscalEdoCtaService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo timbre fiscal usando el procedimiento almacenado sp_CrearTimbreFiscal
        /// </summary>
        public async Task<object> CrearTimbreFiscalAsync(TimbreFiscalCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_CrearTimbreFiscal", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idEstadoCuenta", dto.IdEstadoCuenta);
                command.Parameters.AddWithValue("@uuid", (object?)dto.Uuid ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaTimbrado", dto.FechaTimbrado);
                command.Parameters.AddWithValue("@numeroProveedor", (object?)dto.NumeroProveedor ?? DBNull.Value);

                // Parámetro de salida para el ID
                var idOutputParam = new SqlParameter("@idTimbre", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(idOutputParam);

                await using var reader = await command.ExecuteReaderAsync();

                int idTimbre = 0;
                string mensaje = "Timbre fiscal creado exitosamente";

                if (await reader.ReadAsync())
                {
                    try
                    {
                        idTimbre = reader.GetInt32(reader.GetOrdinal("idTimbre"));
                        mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));
                    }
                    catch
                    {
                        // Si no se pueden leer los campos, usar el parámetro de salida
                    }
                }

                await reader.CloseAsync();

                // Si no se obtuvo del reader, intentar del parámetro de salida
                if (idTimbre == 0 && idOutputParam.Value != DBNull.Value)
                {
                    idTimbre = (int)idOutputParam.Value;
                }

                _logger.LogDebug("Timbre fiscal creado con ID: {IdTimbre}", idTimbre);
                return new { success = true, idTimbre, message = mensaje };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear timbre fiscal. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear timbre fiscal en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear timbre fiscal");
                throw;
            }
        }

        /// <summary>
        /// Consulta timbres fiscales usando el procedimiento almacenado sp_ConsultarTimbresFiscales
        /// </summary>
        public async Task<List<TimbreFiscal>> ConsultarTimbresFiscalesAsync(int? idEstadoCuenta = null, string? uuid = null)
        {
            var timbres = new List<TimbreFiscal>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_ConsultarTimbresFiscales", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idEstadoCuenta", (object?)idEstadoCuenta ?? DBNull.Value);
                command.Parameters.AddWithValue("@uuid", (object?)uuid ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var timbre = new TimbreFiscal
                    {
                        IdTimbre = reader.GetInt32(reader.GetOrdinal("idTimbre")),
                        IdEstadoCuenta = reader.GetInt32(reader.GetOrdinal("idEstadoCuenta")),
                        Uuid = reader.IsDBNull(reader.GetOrdinal("uuid")) ? null : reader.GetString(reader.GetOrdinal("uuid")),
                        FechaTimbrado = reader.IsDBNull(reader.GetOrdinal("fechaTimbrado")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaTimbrado")),
                        NumeroProveedor = reader.IsDBNull(reader.GetOrdinal("numeroProveedor")) ? null : reader.GetString(reader.GetOrdinal("numeroProveedor")),
                        NumeroCuenta = reader.IsDBNull(reader.GetOrdinal("numeroCuenta")) ? null : reader.GetString(reader.GetOrdinal("numeroCuenta")),
                        FechaCorte = reader.IsDBNull(reader.GetOrdinal("fechaCorte")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaCorte"))
                    };

                    timbres.Add(timbre);
                }

                _logger.LogDebug("Consulta de timbres fiscales ejecutada. Se obtuvieron {Count} registros", timbres.Count);

                return timbres;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al consultar timbres fiscales. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al consultar timbres fiscales en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar timbres fiscales");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo complemento fiscal usando el procedimiento almacenado sp_CrearComplementoFiscal
        /// </summary>
        public async Task<object> CrearComplementoFiscalAsync(ComplementoFiscalCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_CrearComplementoFiscal", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idEstadoCuenta", dto.IdEstadoCuenta);
                command.Parameters.AddWithValue("@rfc", (object?)dto.Rfc ?? DBNull.Value);
                command.Parameters.AddWithValue("@formaPago", (object?)dto.FormaPago ?? DBNull.Value);
                command.Parameters.AddWithValue("@metodoPago", (object?)dto.MetodoPago ?? DBNull.Value);
                command.Parameters.AddWithValue("@usoCFDI", (object?)dto.UsoCFDI ?? DBNull.Value);
                command.Parameters.AddWithValue("@claveProducto", (object?)dto.ClaveProducto ?? DBNull.Value);
                command.Parameters.AddWithValue("@codigoPostal", (object?)dto.CodigoPostal ?? DBNull.Value);

                // Parámetro de salida para el ID
                var idOutputParam = new SqlParameter("@idComplemento", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(idOutputParam);

                await using var reader = await command.ExecuteReaderAsync();

                int idComplemento = 0;
                string mensaje = "Complemento fiscal creado exitosamente";

                if (await reader.ReadAsync())
                {
                    try
                    {
                        idComplemento = reader.GetInt32(reader.GetOrdinal("idComplemento"));
                        mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));
                    }
                    catch
                    {
                        // Si no se pueden leer los campos, usar el parámetro de salida
                    }
                }

                await reader.CloseAsync();

                // Si no se obtuvo del reader, intentar del parámetro de salida
                if (idComplemento == 0 && idOutputParam.Value != DBNull.Value)
                {
                    idComplemento = (int)idOutputParam.Value;
                }

                _logger.LogDebug("Complemento fiscal creado con ID: {IdComplemento}", idComplemento);
                return new { success = true, idComplemento, message = mensaje };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear complemento fiscal. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear complemento fiscal en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear complemento fiscal");
                throw;
            }
        }

        /// <summary>
        /// Consulta complementos fiscales usando el procedimiento almacenado sp_ConsultarComplementosFiscales
        /// </summary>
        public async Task<List<ComplementoFiscal>> ConsultarComplementosFiscalesAsync(int? idEstadoCuenta = null, string? rfc = null)
        {
            var complementos = new List<ComplementoFiscal>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_ConsultarComplementosFiscales", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idEstadoCuenta", (object?)idEstadoCuenta ?? DBNull.Value);
                command.Parameters.AddWithValue("@rfc", (object?)rfc ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var complemento = new ComplementoFiscal
                    {
                        IdComplemento = reader.GetInt32(reader.GetOrdinal("idComplemento")),
                        IdEstadoCuenta = reader.GetInt32(reader.GetOrdinal("idEstadoCuenta")),
                        Rfc = reader.IsDBNull(reader.GetOrdinal("rfc")) ? null : reader.GetString(reader.GetOrdinal("rfc")),
                        FormaPago = reader.IsDBNull(reader.GetOrdinal("formaPago")) ? null : reader.GetString(reader.GetOrdinal("formaPago")),
                        MetodoPago = reader.IsDBNull(reader.GetOrdinal("metodoPago")) ? null : reader.GetString(reader.GetOrdinal("metodoPago")),
                        UsoCFDI = reader.IsDBNull(reader.GetOrdinal("usoCFDI")) ? null : reader.GetString(reader.GetOrdinal("usoCFDI")),
                        ClaveProducto = reader.IsDBNull(reader.GetOrdinal("claveProducto")) ? null : reader.GetString(reader.GetOrdinal("claveProducto")),
                        CodigoPostal = reader.IsDBNull(reader.GetOrdinal("codigoPostal")) ? null : reader.GetString(reader.GetOrdinal("codigoPostal")),
                        NumeroCuenta = reader.IsDBNull(reader.GetOrdinal("numeroCuenta")) ? null : reader.GetString(reader.GetOrdinal("numeroCuenta")),
                        FechaCorte = reader.IsDBNull(reader.GetOrdinal("fechaCorte")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaCorte"))
                    };

                    complementos.Add(complemento);
                }

                _logger.LogDebug("Consulta de complementos fiscales ejecutada. Se obtuvieron {Count} registros", complementos.Count);

                return complementos;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al consultar complementos fiscales. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al consultar complementos fiscales en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar complementos fiscales");
                throw;
            }
        }
    }
}
