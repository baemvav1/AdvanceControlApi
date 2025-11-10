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
    /// Implementación del servicio de clientes que usa el procedimiento almacenado sp_cliente_select
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
        /// Obtiene clientes usando el procedimiento almacenado sp_cliente_select
        /// </summary>
        public async Task<List<Cliente>> GetClientesAsync(ClienteQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var clientes = new List<Cliente>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_cliente_select", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@search", (object?)query.Search ?? DBNull.Value);
                command.Parameters.AddWithValue("@rfc", (object?)query.Rfc ?? DBNull.Value);
                command.Parameters.AddWithValue("@curp", (object?)query.Curp ?? DBNull.Value);
                command.Parameters.AddWithValue("@notas", (object?)query.Notas ?? DBNull.Value);
                command.Parameters.AddWithValue("@prioridad", (object?)query.Prioridad ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var cliente = new Cliente
                    {
                        IdCliente = reader.GetInt32(reader.GetOrdinal("id_cliente")),
                        TipoPersona = reader.IsDBNull(reader.GetOrdinal("tipo_persona")) ? null : reader.GetInt32(reader.GetOrdinal("tipo_persona")),
                        Rfc = reader.IsDBNull(reader.GetOrdinal("rfc")) ? null : reader.GetString(reader.GetOrdinal("rfc")),
                        RazonSocial = reader.IsDBNull(reader.GetOrdinal("razon_social")) ? null : reader.GetString(reader.GetOrdinal("razon_social")),
                        NombreComercial = reader.IsDBNull(reader.GetOrdinal("nombre_comercial")) ? null : reader.GetString(reader.GetOrdinal("nombre_comercial")),
                        Curp = reader.IsDBNull(reader.GetOrdinal("curp")) ? null : reader.GetString(reader.GetOrdinal("curp")),
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
    }
}
