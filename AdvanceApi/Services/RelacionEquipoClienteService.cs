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
    /// Implementación del servicio de relaciones equipo-cliente que usa el procedimiento almacenado sp_relacionEquipoCliente_edit
    /// </summary>
    public class RelacionEquipoClienteService : IRelacionEquipoClienteService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<RelacionEquipoClienteService> _logger;

        public RelacionEquipoClienteService(DbHelper dbHelper, ILogger<RelacionEquipoClienteService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene relaciones equipo-cliente usando el procedimiento almacenado sp_relacionEquipoCliente_edit
        /// </summary>
        public async Task<List<RelacionEquipoCliente>> GetRelacionesAsync(RelacionEquipoClienteQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var relaciones = new List<RelacionEquipoCliente>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionEquipoCliente_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", "select");
                command.Parameters.AddWithValue("@identificador", (object?)query.Identificador ?? DBNull.Value);
                command.Parameters.AddWithValue("@idCliente", query.IdCliente);
                command.Parameters.AddWithValue("@nota", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var relacion = new RelacionEquipoCliente
                    {
                        IdRelacion = reader.IsDBNull(reader.GetOrdinal("idRelacion")) ? null : reader.GetInt32(reader.GetOrdinal("idRelacion")),
                        IdCliente = reader.IsDBNull(reader.GetOrdinal("id_cliente")) ? null : reader.GetInt32(reader.GetOrdinal("id_cliente")),
                        RazonSocial = reader.IsDBNull(reader.GetOrdinal("razon_social")) ? null : reader.GetString(reader.GetOrdinal("razon_social")),
                        NombreComercial = reader.IsDBNull(reader.GetOrdinal("nombre_comercial")) ? null : reader.GetString(reader.GetOrdinal("nombre_comercial"))
                    };

                    relaciones.Add(relacion);
                }

                _logger.LogDebug("Se obtuvieron {Count} relaciones equipo-cliente", relaciones.Count);

                return relaciones;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener relaciones equipo-cliente. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener relaciones equipo-cliente de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener relaciones equipo-cliente");
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva relación equipo-cliente
        /// </summary>
        public async Task<object> CreateRelacionAsync(RelacionEquipoClienteQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionEquipoCliente_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "put");
                command.Parameters.AddWithValue("@identificador", (object?)query.Identificador ?? DBNull.Value);
                command.Parameters.AddWithValue("@idCliente", query.IdCliente);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Creación de relación devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Relación creada para identificador {Identificador} y cliente {IdCliente}", query.Identificador, query.IdCliente);
                return new { success = true, message = "Relación creada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear relación equipo-cliente. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear relación equipo-cliente en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear relación equipo-cliente");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) una relación equipo-cliente
        /// </summary>
        public async Task<object> DeleteRelacionAsync(string identificador, int idCliente)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionEquipoCliente_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@identificador", (object?)identificador ?? DBNull.Value);
                command.Parameters.AddWithValue("@idCliente", idCliente);
                command.Parameters.AddWithValue("@nota", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Delete de relación devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Relación eliminada para identificador {Identificador} y cliente {IdCliente}", identificador, idCliente);
                return new { success = true, message = "Relación eliminada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar relación equipo-cliente. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar relación equipo-cliente en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar relación equipo-cliente");
                throw;
            }
        }

        /// <summary>
        /// Actualiza la nota de una relación equipo-cliente
        /// </summary>
        public async Task<object> UpdateNotaAsync(RelacionEquipoClienteQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_relacionEquipoCliente_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update_nota");
                command.Parameters.AddWithValue("@identificador", (object?)query.Identificador ?? DBNull.Value);
                command.Parameters.AddWithValue("@idCliente", query.IdCliente);
                command.Parameters.AddWithValue("@nota", (object?)query.Nota ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Update de nota devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch
                    {
                        // No es un mensaje de resultado, operación exitosa
                    }
                }

                _logger.LogDebug("Nota actualizada para identificador {Identificador} y cliente {IdCliente}", query.Identificador, query.IdCliente);
                return new { success = true, message = "Nota actualizada correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar nota de relación. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar nota de relación en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar nota de relación");
                throw;
            }
        }
    }
}
