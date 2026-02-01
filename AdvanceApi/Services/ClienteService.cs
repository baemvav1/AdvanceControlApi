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
    /// Implementación del servicio de clientes que usa el procedimiento almacenado sp_cliente_edit
    /// </summary>
    public class ClienteService : IClienteService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(DbHelper dbHelper, ILogger<ClienteService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene clientes usando el procedimiento almacenado sp_cliente_edit
        /// </summary>
        public async Task<List<Cliente>> GetClientesAsync(ClienteEditDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var clientes = new List<Cliente>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_cliente_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", "select");
                command.Parameters.AddWithValue("@id_cliente", 0);
                command.Parameters.AddWithValue("@rfc", (object?)query.Rfc ?? DBNull.Value);
                command.Parameters.AddWithValue("@razon_social", (object?)query.RazonSocial ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombre_comercial", (object?)query.NombreComercial ?? DBNull.Value);
                command.Parameters.AddWithValue("@regimen_fiscal", (object?)query.RegimenFiscal ?? DBNull.Value);
                command.Parameters.AddWithValue("@uso_cfdi", (object?)query.UsoCfdi ?? DBNull.Value);
                command.Parameters.AddWithValue("@dias_credito", (object?)query.DiasCredito ?? DBNull.Value);
                command.Parameters.AddWithValue("@limite_credito", (object?)query.LimiteCredito ?? DBNull.Value);
                command.Parameters.AddWithValue("@prioridad", (object?)query.Prioridad ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);
                command.Parameters.AddWithValue("@credencial_id", (object?)query.CredencialId ?? DBNull.Value);
                command.Parameters.AddWithValue("@notas", (object?)query.Notas ?? DBNull.Value);
                command.Parameters.AddWithValue("@id_usuario", (object?)query.IdUsuario ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var cliente = new Cliente
                    {
                        IdCliente = reader.GetInt32(reader.GetOrdinal("id_cliente")),
                        Rfc = reader.IsDBNull(reader.GetOrdinal("rfc")) ? null : reader.GetString(reader.GetOrdinal("rfc")),
                        RazonSocial = reader.IsDBNull(reader.GetOrdinal("razon_social")) ? null : reader.GetString(reader.GetOrdinal("razon_social")),
                        NombreComercial = reader.IsDBNull(reader.GetOrdinal("nombre_comercial")) ? null : reader.GetString(reader.GetOrdinal("nombre_comercial")),
                        RegimenFiscal = reader.IsDBNull(reader.GetOrdinal("regimen_fiscal")) ? null : reader.GetString(reader.GetOrdinal("regimen_fiscal")),
                        UsoCfdi = reader.IsDBNull(reader.GetOrdinal("uso_cfdi")) ? null : reader.GetString(reader.GetOrdinal("uso_cfdi")),
                        DiasCredito = reader.IsDBNull(reader.GetOrdinal("dias_credito")) ? null : reader.GetInt32(reader.GetOrdinal("dias_credito")),
                        LimiteCredito = reader.IsDBNull(reader.GetOrdinal("limite_credito")) ? null : reader.GetDecimal(reader.GetOrdinal("limite_credito")),
                        Prioridad = reader.IsDBNull(reader.GetOrdinal("prioridad")) ? null : reader.GetInt32(reader.GetOrdinal("prioridad")),
                        Estatus = reader.IsDBNull(reader.GetOrdinal("estatus")) ? null : reader.GetBoolean(reader.GetOrdinal("estatus")),
                        CredencialId = reader.IsDBNull(reader.GetOrdinal("credencial_id")) ? null : reader.GetInt32(reader.GetOrdinal("credencial_id")),
                        Notas = reader.IsDBNull(reader.GetOrdinal("notas")) ? null : reader.GetString(reader.GetOrdinal("notas")),
                        CreadoEn = reader.IsDBNull(reader.GetOrdinal("creado_en")) ? null : reader.GetDateTime(reader.GetOrdinal("creado_en")),
                        ActualizadoEn = reader.IsDBNull(reader.GetOrdinal("actualizado_en")) ? null : reader.GetDateTime(reader.GetOrdinal("actualizado_en")),
                        IdUsuarioCreador = reader.IsDBNull(reader.GetOrdinal("id_usuario_creador")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario_creador")),
                        IdUsuarioAct = reader.IsDBNull(reader.GetOrdinal("id_usuaio_act")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuaio_act"))
                    };

                    clientes.Add(cliente);
                }

                _logger.LogDebug("Se obtuvieron {Count} clientes", clientes.Count);

                return clientes;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener clientes. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener clientes de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener clientes");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo cliente usando el procedimiento almacenado sp_cliente_edit
        /// </summary>
        public async Task<object> CreateClienteAsync(ClienteEditDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_cliente_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "create");
                command.Parameters.AddWithValue("@id_cliente", 0);
                command.Parameters.AddWithValue("@rfc", (object?)query.Rfc ?? DBNull.Value);
                command.Parameters.AddWithValue("@razon_social", (object?)query.RazonSocial ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombre_comercial", (object?)query.NombreComercial ?? DBNull.Value);
                command.Parameters.AddWithValue("@regimen_fiscal", (object?)query.RegimenFiscal ?? DBNull.Value);
                command.Parameters.AddWithValue("@uso_cfdi", (object?)query.UsoCfdi ?? DBNull.Value);
                command.Parameters.AddWithValue("@dias_credito", (object?)query.DiasCredito ?? DBNull.Value);
                command.Parameters.AddWithValue("@limite_credito", (object?)query.LimiteCredito ?? DBNull.Value);
                command.Parameters.AddWithValue("@prioridad", (object?)query.Prioridad ?? 1);
                command.Parameters.AddWithValue("@estatus", true);
                command.Parameters.AddWithValue("@credencial_id", (object?)query.CredencialId ?? DBNull.Value);
                command.Parameters.AddWithValue("@notas", (object?)query.Notas ?? DBNull.Value);
                command.Parameters.AddWithValue("@id_usuario", (object?)query.IdUsuario ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        var idCliente = reader.IsDBNull(reader.GetOrdinal("id_cliente")) ? 0 : reader.GetInt32(reader.GetOrdinal("id_cliente"));
                        _logger.LogDebug("Create de cliente devolvió: {Result}, id_cliente: {IdCliente}", result, idCliente);
                        return new { success = true, message = result, id_cliente = idCliente };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Cliente creado correctamente");
                return new { success = true, message = "Cliente creado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear cliente. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear cliente en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear cliente");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un cliente por su ID
        /// </summary>
        public async Task<object> UpdateClienteAsync(ClienteEditDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_cliente_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update");
                command.Parameters.AddWithValue("@id_cliente", query.IdCliente);
                command.Parameters.AddWithValue("@rfc", (object?)query.Rfc ?? DBNull.Value);
                command.Parameters.AddWithValue("@razon_social", (object?)query.RazonSocial ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombre_comercial", (object?)query.NombreComercial ?? DBNull.Value);
                command.Parameters.AddWithValue("@regimen_fiscal", (object?)query.RegimenFiscal ?? DBNull.Value);
                command.Parameters.AddWithValue("@uso_cfdi", (object?)query.UsoCfdi ?? DBNull.Value);
                command.Parameters.AddWithValue("@dias_credito", (object?)query.DiasCredito ?? DBNull.Value);
                command.Parameters.AddWithValue("@limite_credito", (object?)query.LimiteCredito ?? DBNull.Value);
                command.Parameters.AddWithValue("@prioridad", (object?)query.Prioridad ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);
                command.Parameters.AddWithValue("@credencial_id", (object?)query.CredencialId ?? DBNull.Value);
                command.Parameters.AddWithValue("@notas", (object?)query.Notas ?? DBNull.Value);
                command.Parameters.AddWithValue("@id_usuario", (object?)query.IdUsuario ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Update de cliente devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Cliente {IdCliente} actualizado", query.IdCliente);
                return new { success = true, message = "Cliente actualizado exitosamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar cliente. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar cliente en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar cliente");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) un cliente por su ID
        /// </summary>
        public async Task<object> DeleteClienteAsync(int idCliente, int? idUsuario)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_cliente_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@id_cliente", idCliente);
                command.Parameters.AddWithValue("@rfc", DBNull.Value);
                command.Parameters.AddWithValue("@razon_social", DBNull.Value);
                command.Parameters.AddWithValue("@nombre_comercial", DBNull.Value);
                command.Parameters.AddWithValue("@regimen_fiscal", DBNull.Value);
                command.Parameters.AddWithValue("@uso_cfdi", DBNull.Value);
                command.Parameters.AddWithValue("@dias_credito", DBNull.Value);
                command.Parameters.AddWithValue("@limite_credito", DBNull.Value);
                command.Parameters.AddWithValue("@prioridad", DBNull.Value);
                command.Parameters.AddWithValue("@estatus", true);
                command.Parameters.AddWithValue("@credencial_id", DBNull.Value);
                command.Parameters.AddWithValue("@notas", DBNull.Value);
                command.Parameters.AddWithValue("@id_usuario", (object?)idUsuario ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Delete de cliente devolvió: {Result}", result);
                        
                        // Check if it's an error message
                        if (result.Contains("Invalido"))
                        {
                            return new { success = false, message = result };
                        }
                        
                        return new { success = true, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Cliente {IdCliente} eliminado (soft delete)", idCliente);
                return new { success = true, message = "Cliente eliminado exitosamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar cliente. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar cliente en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar cliente");
                throw;
            }
        }
    }
}
