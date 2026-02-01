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
    /// Implementación del servicio de equipos que usa el procedimiento almacenado sp_equipo_edit
    /// </summary>
    public class EquipoService : IEquipoService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<EquipoService> _logger;

        public EquipoService(DbHelper dbHelper, ILogger<EquipoService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Ejecuta operaciones CRUD de equipos usando el procedimiento almacenado sp_equipo_edit
        /// </summary>
        public async Task<List<Equipo>> ExecuteEquipoOperationAsync(EquipoQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var equipos = new List<Equipo>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_equipo_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", query.Operacion);
                command.Parameters.AddWithValue("@idEquipo", query.IdEquipo);
                command.Parameters.AddWithValue("@marca", (object?)query.Marca ?? DBNull.Value);
                command.Parameters.AddWithValue("@creado", (object?)query.Creado ?? DBNull.Value);
                command.Parameters.AddWithValue("@paradas", (object?)query.Paradas ?? DBNull.Value);
                command.Parameters.AddWithValue("@kilogramos", (object?)query.Kilogramos ?? DBNull.Value);
                command.Parameters.AddWithValue("@personas", (object?)query.Personas ?? DBNull.Value);
                command.Parameters.AddWithValue("@descricpion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@identificador", (object?)query.Identificador ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var equipo = new Equipo
                    {
                        IdEquipo = reader.GetInt32(reader.GetOrdinal("idEquipo")),
                        Marca = reader.IsDBNull(reader.GetOrdinal("marca")) ? null : reader.GetString(reader.GetOrdinal("marca")),
                        Creado = reader.IsDBNull(reader.GetOrdinal("creado")) ? null : reader.GetInt32(reader.GetOrdinal("creado")),
                        Paradas = reader.IsDBNull(reader.GetOrdinal("paradas")) ? null : reader.GetInt32(reader.GetOrdinal("paradas")),
                        Kilogramos = reader.IsDBNull(reader.GetOrdinal("kilogramos")) ? null : reader.GetInt32(reader.GetOrdinal("kilogramos")),
                        Personas = reader.IsDBNull(reader.GetOrdinal("personas")) ? null : reader.GetInt32(reader.GetOrdinal("personas")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                        Identificador = reader.IsDBNull(reader.GetOrdinal("identificador")) ? null : reader.GetString(reader.GetOrdinal("identificador")),
                        Estatus = reader.IsDBNull(reader.GetOrdinal("estatus")) ? null : reader.GetBoolean(reader.GetOrdinal("estatus"))
                    };

                    equipos.Add(equipo);
                }

                _logger.LogDebug("Operación '{Operacion}' ejecutada. Se obtuvieron {Count} equipos", query.Operacion, equipos.Count);

                return equipos;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al ejecutar operación de equipo. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al ejecutar operación de equipo en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al ejecutar operación de equipo");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) un equipo por su ID
        /// </summary>
        public async Task<object> DeleteEquipoAsync(int idEquipo)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_equipo_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@idEquipo", idEquipo);
                command.Parameters.AddWithValue("@marca", DBNull.Value);
                command.Parameters.AddWithValue("@creado", DBNull.Value);
                command.Parameters.AddWithValue("@paradas", DBNull.Value);
                command.Parameters.AddWithValue("@kilogramos", DBNull.Value);
                command.Parameters.AddWithValue("@personas", DBNull.Value);
                command.Parameters.AddWithValue("@descricpion", DBNull.Value);
                command.Parameters.AddWithValue("@identificador", DBNull.Value);
                command.Parameters.AddWithValue("@estatus", true);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Delete de equipo devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Equipo {IdEquipo} eliminado (soft delete)", idEquipo);
                return new { success = true, message = "Equipo eliminado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar equipo. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar equipo en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar equipo");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un equipo por su ID
        /// </summary>
        public async Task<object> UpdateEquipoAsync(EquipoQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_equipo_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update");
                command.Parameters.AddWithValue("@idEquipo", query.IdEquipo);
                command.Parameters.AddWithValue("@marca", (object?)query.Marca ?? DBNull.Value);
                command.Parameters.AddWithValue("@creado", (object?)query.Creado ?? DBNull.Value);
                command.Parameters.AddWithValue("@paradas", (object?)query.Paradas ?? DBNull.Value);
                command.Parameters.AddWithValue("@kilogramos", (object?)query.Kilogramos ?? DBNull.Value);
                command.Parameters.AddWithValue("@personas", (object?)query.Personas ?? DBNull.Value);
                command.Parameters.AddWithValue("@descricpion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@identificador", (object?)query.Identificador ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);

                await using var reader = await command.ExecuteReaderAsync();

                // Si hay un resultado, puede ser un mensaje de error
                if (await reader.ReadAsync())
                {
                    try
                    {
                        var result = reader.GetString(reader.GetOrdinal("Result"));
                        _logger.LogWarning("Update de equipo devolvió: {Result}", result);
                        return new { success = false, message = result };
                    }
                    catch (Exception ex)
                    {
                        // No es un mensaje de resultado, operación exitosa
                        _logger.LogDebug(ex, "No se pudo leer columna Result del stored procedure, asumiendo operación exitosa");
                    }
                }

                _logger.LogDebug("Equipo {IdEquipo} actualizado", query.IdEquipo);
                return new { success = true, message = "Equipo actualizado correctamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar equipo. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar equipo en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar equipo");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo equipo usando el procedimiento almacenado sp_equipo_create
        /// </summary>
        public async Task<object> CreateEquipoAsync(EquipoQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_equipo_create", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@marca", (object?)query.Marca ?? DBNull.Value);
                command.Parameters.AddWithValue("@creado", (object?)query.Creado ?? DBNull.Value);
                command.Parameters.AddWithValue("@paradas", (object?)query.Paradas ?? DBNull.Value);
                command.Parameters.AddWithValue("@kilogramos", (object?)query.Kilogramos ?? DBNull.Value);
                command.Parameters.AddWithValue("@personas", (object?)query.Personas ?? DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@identificador", (object?)query.Identificador ?? DBNull.Value);
                command.Parameters.AddWithValue("@estatus", query.Estatus);

                await using var reader = await command.ExecuteReaderAsync();

                int? newId = null;
                Equipo? createdEquipo = null;

                // Primer result set: idEquipo
                if (await reader.ReadAsync())
                {
                    newId = reader.GetInt32(reader.GetOrdinal("idEquipo"));
                }

                // Segundo result set: fila creada
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    createdEquipo = new Equipo
                    {
                        IdEquipo = reader.GetInt32(reader.GetOrdinal("idEquipo")),
                        Marca = reader.IsDBNull(reader.GetOrdinal("marca")) ? null : reader.GetString(reader.GetOrdinal("marca")),
                        Creado = reader.IsDBNull(reader.GetOrdinal("creado")) ? null : reader.GetInt32(reader.GetOrdinal("creado")),
                        Paradas = reader.IsDBNull(reader.GetOrdinal("paradas")) ? null : reader.GetInt32(reader.GetOrdinal("paradas")),
                        Kilogramos = reader.IsDBNull(reader.GetOrdinal("kilogramos")) ? null : reader.GetInt32(reader.GetOrdinal("kilogramos")),
                        Personas = reader.IsDBNull(reader.GetOrdinal("personas")) ? null : reader.GetInt32(reader.GetOrdinal("personas")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                        Identificador = reader.IsDBNull(reader.GetOrdinal("identificador")) ? null : reader.GetString(reader.GetOrdinal("identificador")),
                        Estatus = reader.IsDBNull(reader.GetOrdinal("estatus")) ? null : reader.GetBoolean(reader.GetOrdinal("estatus"))
                    };
                }

                _logger.LogDebug("Equipo creado con ID {IdEquipo}", newId);
                return new { success = true, message = "Equipo creado correctamente", idEquipo = newId, equipo = createdEquipo };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear equipo. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear equipo en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear equipo");
                throw;
            }
        }
    }
}
