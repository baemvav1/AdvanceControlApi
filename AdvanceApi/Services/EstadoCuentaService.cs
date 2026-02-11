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
    /// Implementación del servicio de estados de cuenta que usa el procedimiento almacenado sp_GestionEstadoCuenta
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
        /// Obtiene todos los estados de cuenta con resumen de depósitos
        /// </summary>
        public async Task<List<EstadoCuenta>> GetEstadosCuentaAsync()
        {
            var estadosCuenta = new List<EstadoCuenta>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_GestionEstadoCuenta", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operacion", "Select");

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var estadoCuenta = new EstadoCuenta
                    {
                        EstadoCuentaID = reader.GetInt32(reader.GetOrdinal("EstadoCuentaID")),
                        FechaCorte = reader.IsDBNull(reader.GetOrdinal("FechaCorte")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaCorte")),
                        PeriodoDesde = reader.IsDBNull(reader.GetOrdinal("PeriodoDesde")) ? null : reader.GetDateTime(reader.GetOrdinal("PeriodoDesde")),
                        PeriodoHasta = reader.IsDBNull(reader.GetOrdinal("PeriodoHasta")) ? null : reader.GetDateTime(reader.GetOrdinal("PeriodoHasta")),
                        SaldoInicial = reader.IsDBNull(reader.GetOrdinal("SaldoInicial")) ? null : reader.GetDecimal(reader.GetOrdinal("SaldoInicial")),
                        SaldoCorte = reader.IsDBNull(reader.GetOrdinal("SaldoCorte")) ? null : reader.GetDecimal(reader.GetOrdinal("SaldoCorte")),
                        TotalDepositos = reader.IsDBNull(reader.GetOrdinal("TotalDepositos")) ? null : reader.GetDecimal(reader.GetOrdinal("TotalDepositos")),
                        TotalRetiros = reader.IsDBNull(reader.GetOrdinal("TotalRetiros")) ? null : reader.GetDecimal(reader.GetOrdinal("TotalRetiros")),
                        Comisiones = reader.IsDBNull(reader.GetOrdinal("Comisiones")) ? null : reader.GetDecimal(reader.GetOrdinal("Comisiones")),
                        NombreArchivo = reader.IsDBNull(reader.GetOrdinal("NombreArchivo")) ? null : reader.GetString(reader.GetOrdinal("NombreArchivo")),
                        FechaProcesamiento = reader.IsDBNull(reader.GetOrdinal("FechaProcesamiento")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaProcesamiento")),
                        CantidadDepositos = reader.IsDBNull(reader.GetOrdinal("CantidadDepositos")) ? null : reader.GetInt32(reader.GetOrdinal("CantidadDepositos")),
                        TotalDepositosRegistrados = reader.IsDBNull(reader.GetOrdinal("TotalDepositosRegistrados")) ? null : reader.GetDecimal(reader.GetOrdinal("TotalDepositosRegistrados"))
                    };

                    estadosCuenta.Add(estadoCuenta);
                }

                _logger.LogDebug("Se obtuvieron {Count} estados de cuenta", estadosCuenta.Count);

                return estadosCuenta;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener estados de cuenta. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener estados de cuenta de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener estados de cuenta");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo estado de cuenta
        /// </summary>
        public async Task<object> CreateEstadoCuentaAsync(EstadoCuentaQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_GestionEstadoCuenta", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operacion", "Create_Estado");
                command.Parameters.AddWithValue("@FechaCorte", (object?)query.FechaCorte ?? DBNull.Value);
                command.Parameters.AddWithValue("@PeriodoDesde", (object?)query.PeriodoDesde ?? DBNull.Value);
                command.Parameters.AddWithValue("@PeriodoHasta", (object?)query.PeriodoHasta ?? DBNull.Value);
                command.Parameters.AddWithValue("@SaldoInicial", (object?)query.SaldoInicial ?? DBNull.Value);
                command.Parameters.AddWithValue("@SaldoCorte", (object?)query.SaldoCorte ?? DBNull.Value);
                command.Parameters.AddWithValue("@TotalDepositos", (object?)query.TotalDepositos ?? DBNull.Value);
                command.Parameters.AddWithValue("@TotalRetiros", (object?)query.TotalRetiros ?? DBNull.Value);
                command.Parameters.AddWithValue("@Comisiones", (object?)query.Comisiones ?? DBNull.Value);
                command.Parameters.AddWithValue("@NombreArchivo", (object?)query.NombreArchivo ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // Try to read the result
                    try
                    {
                        var mensaje = reader.IsDBNull(reader.GetOrdinal("Mensaje")) ? null : reader.GetString(reader.GetOrdinal("Mensaje"));
                        
                        // Check if EstadoCuentaID column exists
                        try
                        {
                            var estadoCuentaId = reader.GetDecimal(reader.GetOrdinal("EstadoCuentaID"));
                            _logger.LogDebug("Estado de cuenta creado con ID: {EstadoCuentaID}", estadoCuentaId);
                            return new { success = true, estadoCuentaId = (int)estadoCuentaId, message = mensaje };
                        }
                        catch
                        {
                            // Might be a warning message with existing ID
                            return new { success = false, message = mensaje };
                        }
                    }
                    catch
                    {
                        return new { success = false, message = "Error al procesar respuesta" };
                    }
                }

                return new { success = true, message = "Estado de cuenta procesado" };
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
        /// Agrega un depósito a un estado de cuenta
        /// </summary>
        public async Task<object> CreateDepositoAsync(EstadoCuentaQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_GestionEstadoCuenta", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operacion", "Create_Deposito");
                command.Parameters.AddWithValue("@EstadoCuentaID", (object?)query.EstadoCuentaID ?? DBNull.Value);
                command.Parameters.AddWithValue("@FechaDeposito", (object?)query.FechaDeposito ?? DBNull.Value);
                command.Parameters.AddWithValue("@DescripcionDeposito", (object?)query.DescripcionDeposito ?? DBNull.Value);
                command.Parameters.AddWithValue("@MontoDeposito", (object?)query.MontoDeposito ?? DBNull.Value);
                command.Parameters.AddWithValue("@TipoDeposito", (object?)query.TipoDeposito ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    try
                    {
                        var mensaje = reader.IsDBNull(reader.GetOrdinal("Mensaje")) ? null : reader.GetString(reader.GetOrdinal("Mensaje"));
                        
                        // Check if DepositoID column exists
                        try
                        {
                            var depositoId = reader.GetDecimal(reader.GetOrdinal("DepositoID"));
                            _logger.LogDebug("Depósito creado con ID: {DepositoID}", depositoId);
                            return new { success = true, depositoId = (int)depositoId, message = mensaje };
                        }
                        catch
                        {
                            // Might be a duplicate warning or error message
                            try
                            {
                                // Check if DescripcionExistente exists (warning case)
                                var descripcionExistente = reader.IsDBNull(reader.GetOrdinal("DescripcionExistente")) ? null : reader.GetString(reader.GetOrdinal("DescripcionExistente"));
                                var depositoIdExistente = reader.GetInt32(reader.GetOrdinal("DepositoID"));
                                return new { success = false, depositoId = depositoIdExistente, message = mensaje, descripcionExistente };
                            }
                            catch
                            {
                                return new { success = false, message = mensaje };
                            }
                        }
                    }
                    catch
                    {
                        return new { success = false, message = "Error al procesar respuesta" };
                    }
                }

                return new { success = true, message = "Depósito procesado" };
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
        /// Obtiene los depósitos de un estado de cuenta específico
        /// </summary>
        public async Task<List<Deposito>> GetDepositosAsync(int estadoCuentaId)
        {
            var depositos = new List<Deposito>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_GestionEstadoCuenta", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operacion", "Select_Depositos");
                command.Parameters.AddWithValue("@EstadoCuentaID", estadoCuentaId);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var deposito = new Deposito
                    {
                        DepositoID = reader.GetInt32(reader.GetOrdinal("DepositoID")),
                        Fecha = reader.IsDBNull(reader.GetOrdinal("Fecha")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                        Monto = reader.IsDBNull(reader.GetOrdinal("Monto")) ? null : reader.GetDecimal(reader.GetOrdinal("Monto")),
                        TipoDeposito = reader.IsDBNull(reader.GetOrdinal("TipoDeposito")) ? null : reader.GetString(reader.GetOrdinal("TipoDeposito"))
                    };

                    depositos.Add(deposito);
                }

                _logger.LogDebug("Se obtuvieron {Count} depósitos para EstadoCuentaID: {EstadoCuentaID}", depositos.Count, estadoCuentaId);

                return depositos;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener depósitos. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener depósitos de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener depósitos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene el resumen de depósitos por tipo de un estado de cuenta
        /// </summary>
        public async Task<List<DepositoResumen>> GetResumenDepositosAsync(int estadoCuentaId)
        {
            var resumen = new List<DepositoResumen>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_GestionEstadoCuenta", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operacion", "Select_Resumen");
                command.Parameters.AddWithValue("@EstadoCuentaID", estadoCuentaId);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var item = new DepositoResumen
                    {
                        TipoDeposito = reader.IsDBNull(reader.GetOrdinal("TipoDeposito")) ? null : reader.GetString(reader.GetOrdinal("TipoDeposito")),
                        Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                        Total = reader.IsDBNull(reader.GetOrdinal("Total")) ? null : reader.GetDecimal(reader.GetOrdinal("Total")),
                        Promedio = reader.IsDBNull(reader.GetOrdinal("Promedio")) ? null : reader.GetDecimal(reader.GetOrdinal("Promedio"))
                    };

                    resumen.Add(item);
                }

                _logger.LogDebug("Se obtuvo resumen de {Count} tipos de depósito para EstadoCuentaID: {EstadoCuentaID}", resumen.Count, estadoCuentaId);

                return resumen;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener resumen de depósitos. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener resumen de depósitos de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener resumen de depósitos");
                throw;
            }
        }

        /// <summary>
        /// Verifica si un depósito específico ya existe
        /// </summary>
        public async Task<List<DepositoVerificacion>> VerificarDepositoAsync(EstadoCuentaQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var resultados = new List<DepositoVerificacion>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_GestionEstadoCuenta", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operacion", "Verificar_Deposito");
                command.Parameters.AddWithValue("@EstadoCuentaID", (object?)query.EstadoCuentaID ?? DBNull.Value);
                command.Parameters.AddWithValue("@FechaDeposito", (object?)query.FechaDeposito ?? DBNull.Value);
                command.Parameters.AddWithValue("@DescripcionDeposito", (object?)query.DescripcionDeposito ?? DBNull.Value);
                command.Parameters.AddWithValue("@MontoDeposito", (object?)query.MontoDeposito ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var item = new DepositoVerificacion
                    {
                        Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? null : reader.GetString(reader.GetOrdinal("Estado")),
                        DepositoID = reader.IsDBNull(reader.GetOrdinal("DepositoID")) ? null : reader.GetInt32(reader.GetOrdinal("DepositoID")),
                        Fecha = reader.IsDBNull(reader.GetOrdinal("Fecha")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                        Monto = reader.IsDBNull(reader.GetOrdinal("Monto")) ? null : reader.GetDecimal(reader.GetOrdinal("Monto")),
                        TipoDeposito = reader.IsDBNull(reader.GetOrdinal("TipoDeposito")) ? null : reader.GetString(reader.GetOrdinal("TipoDeposito"))
                    };

                    resultados.Add(item);
                }

                _logger.LogDebug("Verificación de depósito completada, {Count} resultados", resultados.Count);

                return resultados;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al verificar depósito. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al verificar depósito en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al verificar depósito");
                throw;
            }
        }

        /// <summary>
        /// Busca posibles depósitos duplicados en un estado de cuenta
        /// </summary>
        public async Task<List<DepositoDuplicado>> BuscarDuplicadosAsync(int estadoCuentaId)
        {
            var duplicados = new List<DepositoDuplicado>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_GestionEstadoCuenta", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operacion", "Buscar_Duplicados");
                command.Parameters.AddWithValue("@EstadoCuentaID", estadoCuentaId);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var item = new DepositoDuplicado
                    {
                        Fecha = reader.IsDBNull(reader.GetOrdinal("Fecha")) ? null : reader.GetDateTime(reader.GetOrdinal("Fecha")),
                        Monto = reader.IsDBNull(reader.GetOrdinal("Monto")) ? null : reader.GetDecimal(reader.GetOrdinal("Monto")),
                        Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                        Descripciones = reader.IsDBNull(reader.GetOrdinal("Descripciones")) ? null : reader.GetString(reader.GetOrdinal("Descripciones")),
                        Tipos = reader.IsDBNull(reader.GetOrdinal("Tipos")) ? null : reader.GetString(reader.GetOrdinal("Tipos"))
                    };

                    duplicados.Add(item);
                }

                _logger.LogDebug("Se encontraron {Count} posibles duplicados para EstadoCuentaID: {EstadoCuentaID}", duplicados.Count, estadoCuentaId);

                return duplicados;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al buscar duplicados. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al buscar duplicados en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al buscar duplicados");
                throw;
            }
        }
    }
}
