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
    /// Implementación del servicio de EstadoCuenta que usa los procedimientos almacenados
    /// sp_CrearEstadoCuenta, sp_EditarEstadoCuenta y sp_ConsultarEstadoCuenta
    /// </summary>
    public class EstadoCuentaService : IEstadoCuentaService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<EstadoCuentaService> _logger;

        public EstadoCuentaService(DbHelper dbHelper, ILogger<EstadoCuentaService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo estado de cuenta usando el procedimiento almacenado sp_CrearEstadoCuenta
        /// </summary>
        public async Task<object> CrearEstadoCuentaAsync(EstadoCuentaQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_CrearEstadoCuenta", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@numeroCuenta", (object?)query.NumeroCuenta ?? DBNull.Value);
                command.Parameters.AddWithValue("@clabe", (object?)query.Clabe ?? DBNull.Value);
                command.Parameters.AddWithValue("@tipoCuenta", (object?)query.TipoCuenta ?? DBNull.Value);
                command.Parameters.AddWithValue("@tipoMoneda", (object?)query.TipoMoneda ?? "MXN");
                command.Parameters.AddWithValue("@fechaInicio", (object?)query.FechaInicio ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaFin", (object?)query.FechaFin ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaCorte", (object?)query.FechaCorte ?? DBNull.Value);
                command.Parameters.AddWithValue("@saldoInicial", (object?)query.SaldoInicial ?? 0);
                command.Parameters.AddWithValue("@totalCargos", (object?)query.TotalCargos ?? 0);
                command.Parameters.AddWithValue("@totalAbonos", (object?)query.TotalAbonos ?? 0);
                command.Parameters.AddWithValue("@saldoFinal", (object?)query.SaldoFinal ?? 0);
                command.Parameters.AddWithValue("@totalComisiones", (object?)query.TotalComisiones ?? 0);
                command.Parameters.AddWithValue("@totalISR", (object?)query.TotalISR ?? 0);
                command.Parameters.AddWithValue("@totalIVA", (object?)query.TotalIVA ?? 0);

                // Parámetro de salida para el ID
                var idOutputParam = new SqlParameter("@idEstadoCuenta", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(idOutputParam);

                await using var reader = await command.ExecuteReaderAsync();

                int idEstadoCuenta = 0;
                string mensaje = "Estado de cuenta creado exitosamente";

                if (await reader.ReadAsync())
                {
                    try
                    {
                        idEstadoCuenta = reader.GetInt32(reader.GetOrdinal("idEstadoCuenta"));
                        mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));
                    }
                    catch
                    {
                        // Si no se pueden leer los campos, usar el parámetro de salida
                    }
                }

                await reader.CloseAsync();

                // Si no se obtuvo del reader, intentar del parámetro de salida
                if (idEstadoCuenta == 0 && idOutputParam.Value != DBNull.Value)
                {
                    idEstadoCuenta = (int)idOutputParam.Value;
                }

                _logger.LogDebug("Estado de cuenta creado con ID: {IdEstadoCuenta}", idEstadoCuenta);
                return new { success = true, idEstadoCuenta, message = mensaje };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear estado de cuenta. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear estado de cuenta en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear estado de cuenta");
                throw;
            }
        }

        /// <summary>
        /// Edita un estado de cuenta existente usando el procedimiento almacenado sp_EditarEstadoCuenta
        /// </summary>
        public async Task<object> EditarEstadoCuentaAsync(EstadoCuentaQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_EditarEstadoCuenta", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idEstadoCuenta", query.IdEstadoCuenta);
                command.Parameters.AddWithValue("@numeroCuenta", (object?)query.NumeroCuenta ?? DBNull.Value);
                command.Parameters.AddWithValue("@clabe", (object?)query.Clabe ?? DBNull.Value);
                command.Parameters.AddWithValue("@tipoCuenta", (object?)query.TipoCuenta ?? DBNull.Value);
                command.Parameters.AddWithValue("@tipoMoneda", (object?)query.TipoMoneda ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaInicio", (object?)query.FechaInicio ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaFin", (object?)query.FechaFin ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaCorte", (object?)query.FechaCorte ?? DBNull.Value);
                command.Parameters.AddWithValue("@saldoInicial", (object?)query.SaldoInicial ?? DBNull.Value);
                command.Parameters.AddWithValue("@totalCargos", (object?)query.TotalCargos ?? DBNull.Value);
                command.Parameters.AddWithValue("@totalAbonos", (object?)query.TotalAbonos ?? DBNull.Value);
                command.Parameters.AddWithValue("@saldoFinal", (object?)query.SaldoFinal ?? DBNull.Value);
                command.Parameters.AddWithValue("@totalComisiones", (object?)query.TotalComisiones ?? DBNull.Value);
                command.Parameters.AddWithValue("@totalISR", (object?)query.TotalISR ?? DBNull.Value);
                command.Parameters.AddWithValue("@totalIVA", (object?)query.TotalIVA ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                string mensaje = "Estado de cuenta actualizado exitosamente";

                if (await reader.ReadAsync())
                {
                    try
                    {
                        mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));
                    }
                    catch
                    {
                        // Si no se puede leer el mensaje, usar el valor por defecto
                    }
                }

                _logger.LogDebug("Estado de cuenta {IdEstadoCuenta} actualizado", query.IdEstadoCuenta);
                return new { success = true, message = mensaje };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al editar estado de cuenta. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al editar estado de cuenta en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al editar estado de cuenta");
                throw;
            }
        }

        /// <summary>
        /// Consulta estados de cuenta usando el procedimiento almacenado sp_ConsultarEstadoCuenta
        /// </summary>
        public async Task<List<EstadoCuenta>> ConsultarEstadoCuentaAsync(int? idEstadoCuenta = null, string? numeroCuenta = null, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var estadosCuenta = new List<EstadoCuenta>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_ConsultarEstadoCuenta", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@idEstadoCuenta", (object?)idEstadoCuenta ?? DBNull.Value);
                command.Parameters.AddWithValue("@numeroCuenta", (object?)numeroCuenta ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaInicio", (object?)fechaInicio ?? DBNull.Value);
                command.Parameters.AddWithValue("@fechaFin", (object?)fechaFin ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var estadoCuenta = new EstadoCuenta
                    {
                        IdEstadoCuenta = reader.GetInt32(reader.GetOrdinal("idEstadoCuenta")),
                        NumeroCuenta = reader.IsDBNull(reader.GetOrdinal("numeroCuenta")) ? null : reader.GetString(reader.GetOrdinal("numeroCuenta")),
                        Clabe = reader.IsDBNull(reader.GetOrdinal("clabe")) ? null : reader.GetString(reader.GetOrdinal("clabe")),
                        TipoCuenta = reader.IsDBNull(reader.GetOrdinal("tipoCuenta")) ? null : reader.GetString(reader.GetOrdinal("tipoCuenta")),
                        TipoMoneda = reader.IsDBNull(reader.GetOrdinal("tipoMoneda")) ? null : reader.GetString(reader.GetOrdinal("tipoMoneda")),
                        FechaInicio = reader.IsDBNull(reader.GetOrdinal("fechaInicio")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaInicio")),
                        FechaFin = reader.IsDBNull(reader.GetOrdinal("fechaFin")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaFin")),
                        FechaCorte = reader.IsDBNull(reader.GetOrdinal("fechaCorte")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaCorte")),
                        SaldoInicial = reader.IsDBNull(reader.GetOrdinal("saldoInicial")) ? null : reader.GetDecimal(reader.GetOrdinal("saldoInicial")),
                        TotalCargos = reader.IsDBNull(reader.GetOrdinal("totalCargos")) ? null : reader.GetDecimal(reader.GetOrdinal("totalCargos")),
                        TotalAbonos = reader.IsDBNull(reader.GetOrdinal("totalAbonos")) ? null : reader.GetDecimal(reader.GetOrdinal("totalAbonos")),
                        SaldoFinal = reader.IsDBNull(reader.GetOrdinal("saldoFinal")) ? null : reader.GetDecimal(reader.GetOrdinal("saldoFinal")),
                        TotalComisiones = reader.IsDBNull(reader.GetOrdinal("totalComisiones")) ? null : reader.GetDecimal(reader.GetOrdinal("totalComisiones")),
                        TotalISR = reader.IsDBNull(reader.GetOrdinal("totalISR")) ? null : reader.GetDecimal(reader.GetOrdinal("totalISR")),
                        TotalIVA = reader.IsDBNull(reader.GetOrdinal("totalIVA")) ? null : reader.GetDecimal(reader.GetOrdinal("totalIVA")),
                        FechaCarga = reader.IsDBNull(reader.GetOrdinal("fechaCarga")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaCarga"))
                    };

                    estadosCuenta.Add(estadoCuenta);
                }

                _logger.LogDebug("Consulta de estados de cuenta ejecutada. Se obtuvieron {Count} registros", estadosCuenta.Count);

                return estadosCuenta;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al consultar estados de cuenta. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al consultar estados de cuenta en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar estados de cuenta");
                throw;
            }
        }

        /// <summary>
        /// Consulta el estado de cuenta completo con todos sus datos relacionados usando sp_ConsultarEstadoCuentaCompleto
        /// </summary>
        public async Task<EstadoCuentaCompletoDto> ConsultarEstadoCuentaCompletoAsync(int idEstadoCuenta)
        {
            var resultado = new EstadoCuentaCompletoDto();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_ConsultarEstadoCuentaCompleto", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetro del procedimiento almacenado
                command.Parameters.AddWithValue("@idEstadoCuenta", idEstadoCuenta);

                await using var reader = await command.ExecuteReaderAsync();

                // Primer conjunto de resultados: Estado de Cuenta
                if (await reader.ReadAsync())
                {
                    resultado.EstadoCuenta = new EstadoCuenta
                    {
                        IdEstadoCuenta = reader.GetInt32(reader.GetOrdinal("idEstadoCuenta")),
                        NumeroCuenta = reader.IsDBNull(reader.GetOrdinal("numeroCuenta")) ? null : reader.GetString(reader.GetOrdinal("numeroCuenta")),
                        Clabe = reader.IsDBNull(reader.GetOrdinal("clabe")) ? null : reader.GetString(reader.GetOrdinal("clabe")),
                        TipoCuenta = reader.IsDBNull(reader.GetOrdinal("tipoCuenta")) ? null : reader.GetString(reader.GetOrdinal("tipoCuenta")),
                        TipoMoneda = reader.IsDBNull(reader.GetOrdinal("tipoMoneda")) ? null : reader.GetString(reader.GetOrdinal("tipoMoneda")),
                        FechaInicio = reader.IsDBNull(reader.GetOrdinal("fechaInicio")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaInicio")),
                        FechaFin = reader.IsDBNull(reader.GetOrdinal("fechaFin")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaFin")),
                        FechaCorte = reader.IsDBNull(reader.GetOrdinal("fechaCorte")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaCorte")),
                        SaldoInicial = reader.IsDBNull(reader.GetOrdinal("saldoInicial")) ? null : reader.GetDecimal(reader.GetOrdinal("saldoInicial")),
                        TotalCargos = reader.IsDBNull(reader.GetOrdinal("totalCargos")) ? null : reader.GetDecimal(reader.GetOrdinal("totalCargos")),
                        TotalAbonos = reader.IsDBNull(reader.GetOrdinal("totalAbonos")) ? null : reader.GetDecimal(reader.GetOrdinal("totalAbonos")),
                        SaldoFinal = reader.IsDBNull(reader.GetOrdinal("saldoFinal")) ? null : reader.GetDecimal(reader.GetOrdinal("saldoFinal")),
                        TotalComisiones = reader.IsDBNull(reader.GetOrdinal("totalComisiones")) ? null : reader.GetDecimal(reader.GetOrdinal("totalComisiones")),
                        TotalISR = reader.IsDBNull(reader.GetOrdinal("totalISR")) ? null : reader.GetDecimal(reader.GetOrdinal("totalISR")),
                        TotalIVA = reader.IsDBNull(reader.GetOrdinal("totalIVA")) ? null : reader.GetDecimal(reader.GetOrdinal("totalIVA")),
                        FechaCarga = reader.IsDBNull(reader.GetOrdinal("fechaCarga")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaCarga"))
                    };
                }

                // Segundo conjunto de resultados: Movimientos
                if (await reader.NextResultAsync())
                {
                    resultado.Movimientos = new List<Movimiento>();
                    while (await reader.ReadAsync())
                    {
                        var movimiento = new Movimiento
                        {
                            IdMovimiento = reader.GetInt32(reader.GetOrdinal("idMovimiento")),
                            Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                            Referencia = reader.IsDBNull(reader.GetOrdinal("referencia")) ? null : reader.GetString(reader.GetOrdinal("referencia")),
                            Cargo = reader.IsDBNull(reader.GetOrdinal("cargo")) ? null : reader.GetDecimal(reader.GetOrdinal("cargo")),
                            Abono = reader.IsDBNull(reader.GetOrdinal("abono")) ? null : reader.GetDecimal(reader.GetOrdinal("abono")),
                            Saldo = reader.GetDecimal(reader.GetOrdinal("saldo")),
                            TipoOperacion = reader.IsDBNull(reader.GetOrdinal("tipoOperacion")) ? null : reader.GetString(reader.GetOrdinal("tipoOperacion"))
                        };

                        resultado.Movimientos.Add(movimiento);
                    }
                }

                // Tercer conjunto de resultados: Transferencias SPEI
                if (await reader.NextResultAsync())
                {
                    resultado.TransferenciasSPEI = new List<TransferenciaSPEI>();
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
                            Monto = reader.GetDecimal(reader.GetOrdinal("monto"))
                        };

                        resultado.TransferenciasSPEI.Add(transferencia);
                    }
                }

                // Cuarto conjunto de resultados: Comisiones
                if (await reader.NextResultAsync())
                {
                    resultado.Comisiones = new List<ComisionBancaria>();
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

                        resultado.Comisiones.Add(comision);
                    }
                }

                // Quinto conjunto de resultados: Impuestos
                if (await reader.NextResultAsync())
                {
                    resultado.Impuestos = new List<ImpuestoMovimiento>();
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

                        resultado.Impuestos.Add(impuesto);
                    }
                }

                // Validar que se encontró un estado de cuenta
                if (resultado.EstadoCuenta == null)
                {
                    _logger.LogWarning("No se encontró estado de cuenta con ID: {IdEstadoCuenta}", idEstadoCuenta);
                    return resultado;
                }

                _logger.LogDebug("Estado de cuenta completo consultado. ID: {IdEstadoCuenta}, Movimientos: {CantidadMovimientos}, Transferencias SPEI: {CantidadSPEI}, Comisiones: {CantidadComisiones}, Impuestos: {CantidadImpuestos}", 
                    idEstadoCuenta, resultado.Movimientos?.Count ?? 0, resultado.TransferenciasSPEI?.Count ?? 0, resultado.Comisiones?.Count ?? 0, resultado.Impuestos?.Count ?? 0);

                return resultado;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al consultar estado de cuenta completo. IdEstadoCuenta: {IdEstadoCuenta}, SqlError: {Message}", idEstadoCuenta, sqlEx.Message);
                throw new InvalidOperationException("Error al consultar estado de cuenta completo en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar estado de cuenta completo. IdEstadoCuenta: {IdEstadoCuenta}", idEstadoCuenta);
                throw;
            }
        }
    }
}
