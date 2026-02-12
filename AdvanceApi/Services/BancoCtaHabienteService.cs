using AdvanceApi.DTOs;
using AdvanceApi.Helpers;
using Clases;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdvanceApi.Services
{
    public class BancoCtaHabienteService : IBancoCtaHabienteService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<BancoCtaHabienteService> _logger;

        public BancoCtaHabienteService(DbHelper dbHelper, ILogger<BancoCtaHabienteService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // =============================================
        // BANCO METHODS
        // =============================================

        public async Task<object> CreateBancoAsync(BancoDto banco)
        {
            if (banco == null)
                throw new ArgumentNullException(nameof(banco));

            if (string.IsNullOrWhiteSpace(banco.NombreBanco))
                throw new ArgumentException("El nombre del banco es obligatorio", nameof(banco));

            if (string.IsNullOrWhiteSpace(banco.Rfc))
                throw new ArgumentException("El RFC es obligatorio", nameof(banco));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_CrearBanco", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@nombreBanco", banco.NombreBanco);
                command.Parameters.AddWithValue("@rfc", banco.Rfc);
                command.Parameters.AddWithValue("@nombreSucursal", (object?)banco.NombreSucursal ?? DBNull.Value);
                command.Parameters.AddWithValue("@direccion", (object?)banco.Direccion ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var idBanco = reader.GetInt32(reader.GetOrdinal("idBanco"));
                    var mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));
                    
                    _logger.LogDebug("Banco creado con ID: {IdBanco}", idBanco);
                    return new { success = true, idBanco = idBanco, message = mensaje };
                }

                _logger.LogWarning("El procedimiento no retornó resultados");
                return new { success = false, message = "No se pudo crear el banco" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear banco. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear banco en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear banco");
                throw;
            }
        }

        public async Task<List<Banco>> GetBancosAsync(BancoDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var bancos = new List<Banco>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_ConsultarBanco", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@idBanco", (object?)query.IdBanco ?? DBNull.Value);
                command.Parameters.AddWithValue("@rfc", (object?)query.Rfc ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var banco = new Banco
                    {
                        IdBanco = reader.GetInt32(reader.GetOrdinal("idBanco")),
                        NombreBanco = reader.IsDBNull(reader.GetOrdinal("nombreBanco")) ? null : reader.GetString(reader.GetOrdinal("nombreBanco")),
                        Rfc = reader.IsDBNull(reader.GetOrdinal("rfc")) ? null : reader.GetString(reader.GetOrdinal("rfc")),
                        NombreSucursal = reader.IsDBNull(reader.GetOrdinal("nombreSucursal")) ? null : reader.GetString(reader.GetOrdinal("nombreSucursal")),
                        Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString(reader.GetOrdinal("direccion"))
                    };

                    bancos.Add(banco);
                }

                _logger.LogDebug("Se obtuvieron {Count} bancos", bancos.Count);
                return bancos;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener bancos. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener bancos de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener bancos");
                throw;
            }
        }

        // =============================================
        // CUENTA HABIENTE METHODS
        // =============================================

        public async Task<object> CreateCuentaHabienteAsync(CuentaHabienteDto cuentaHabiente)
        {
            if (cuentaHabiente == null)
                throw new ArgumentNullException(nameof(cuentaHabiente));

            if (string.IsNullOrWhiteSpace(cuentaHabiente.Nombre))
                throw new ArgumentException("El nombre es obligatorio", nameof(cuentaHabiente));

            if (string.IsNullOrWhiteSpace(cuentaHabiente.Rfc))
                throw new ArgumentException("El RFC es obligatorio", nameof(cuentaHabiente));

            if (string.IsNullOrWhiteSpace(cuentaHabiente.NumeroCuenta))
                throw new ArgumentException("El número de cuenta es obligatorio", nameof(cuentaHabiente));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_CrearCuentaHabiente", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@nombre", cuentaHabiente.Nombre);
                command.Parameters.AddWithValue("@rfc", cuentaHabiente.Rfc);
                command.Parameters.AddWithValue("@numeroCuenta", cuentaHabiente.NumeroCuenta);
                command.Parameters.AddWithValue("@direccion", (object?)cuentaHabiente.Direccion ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var idCuentaHabiente = reader.GetInt32(reader.GetOrdinal("idCuentaHabiente"));
                    var mensaje = reader.GetString(reader.GetOrdinal("Mensaje"));
                    
                    _logger.LogDebug("Cuenta habiente creada con ID: {IdCuentaHabiente}", idCuentaHabiente);
                    return new { success = true, idCuentaHabiente = idCuentaHabiente, message = mensaje };
                }

                _logger.LogWarning("El procedimiento no retornó resultados");
                return new { success = false, message = "No se pudo crear la cuenta habiente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear cuenta habiente. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear cuenta habiente en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear cuenta habiente");
                throw;
            }
        }

        public async Task<List<CuentaHabiente>> GetCuentasHabienteAsync(CuentaHabienteDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var cuentasHabiente = new List<CuentaHabiente>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_ConsultarCuentaHabiente", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@idCuentaHabiente", (object?)query.IdCuentaHabiente ?? DBNull.Value);
                command.Parameters.AddWithValue("@numeroCuenta", (object?)query.NumeroCuenta ?? DBNull.Value);
                command.Parameters.AddWithValue("@rfc", (object?)query.Rfc ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var cuentaHabiente = new CuentaHabiente
                    {
                        IdCuentaHabiente = reader.GetInt32(reader.GetOrdinal("idCuentaHabiente")),
                        Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                        Rfc = reader.IsDBNull(reader.GetOrdinal("rfc")) ? null : reader.GetString(reader.GetOrdinal("rfc")),
                        NumeroCuenta = reader.IsDBNull(reader.GetOrdinal("numeroCuenta")) ? null : reader.GetString(reader.GetOrdinal("numeroCuenta")),
                        Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString(reader.GetOrdinal("direccion"))
                    };

                    cuentasHabiente.Add(cuentaHabiente);
                }

                _logger.LogDebug("Se obtuvieron {Count} cuentas habiente", cuentasHabiente.Count);
                return cuentasHabiente;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener cuentas habiente. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener cuentas habiente de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener cuentas habiente");
                throw;
            }
        }
    }
}
