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
    /// Implementación del servicio de ubicaciones que usa el procedimiento almacenado SP_Ubicacion_CRUD
    /// </summary>
    public class UbicacionService : IUbicacionService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<UbicacionService> _logger;

        public UbicacionService(DbHelper dbHelper, ILogger<UbicacionService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Agrega un parámetro decimal con tipo explícito para evitar errores de conversión numeric/decimal
        /// </summary>
        /// <param name="command">El comando SQL al cual agregar el parámetro</param>
        /// <param name="parameterName">Nombre del parámetro (ej: @latitud)</param>
        /// <param name="value">Valor decimal nullable a asignar</param>
        /// <param name="precision">Precisión del decimal (dígitos totales, máximo 38)</param>
        /// <param name="scale">Escala del decimal (dígitos después del punto decimal)</param>
        private static void AddDecimalParameter(SqlCommand command, string parameterName, decimal? value, byte precision = 28, byte scale = 8)
        {
            var param = command.Parameters.Add(parameterName, SqlDbType.Decimal);
            param.Precision = precision;
            param.Scale = scale;
            param.Value = value.HasValue ? (object)value.Value : DBNull.Value;
        }

        /// <summary>
        /// Helper method to map a SqlDataReader to a Ubicacion object
        /// </summary>
        private Ubicacion MapReaderToUbicacion(SqlDataReader reader)
        {
            return new Ubicacion
            {
                IdUbicacion = reader.GetInt32(reader.GetOrdinal("idUbicacion")),
                Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                Latitud = reader.IsDBNull(reader.GetOrdinal("latitud")) ? null : reader.GetDecimal(reader.GetOrdinal("latitud")),
                Longitud = reader.IsDBNull(reader.GetOrdinal("longitud")) ? null : reader.GetDecimal(reader.GetOrdinal("longitud")),
                DireccionCompleta = reader.IsDBNull(reader.GetOrdinal("direccionCompleta")) ? null : reader.GetString(reader.GetOrdinal("direccionCompleta")),
                Ciudad = reader.IsDBNull(reader.GetOrdinal("ciudad")) ? null : reader.GetString(reader.GetOrdinal("ciudad")),
                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                Pais = reader.IsDBNull(reader.GetOrdinal("pais")) ? null : reader.GetString(reader.GetOrdinal("pais")),
                PlaceId = reader.IsDBNull(reader.GetOrdinal("placeId")) ? null : reader.GetString(reader.GetOrdinal("placeId")),
                Icono = reader.IsDBNull(reader.GetOrdinal("icono")) ? null : reader.GetString(reader.GetOrdinal("icono")),
                ColorIcono = reader.IsDBNull(reader.GetOrdinal("colorIcono")) ? null : reader.GetString(reader.GetOrdinal("colorIcono")),
                NivelZoom = reader.IsDBNull(reader.GetOrdinal("nivelZoom")) ? null : reader.GetInt32(reader.GetOrdinal("nivelZoom")),
                InfoWindowHTML = reader.IsDBNull(reader.GetOrdinal("infoWindowHTML")) ? null : reader.GetString(reader.GetOrdinal("infoWindowHTML")),
                Categoria = reader.IsDBNull(reader.GetOrdinal("categoria")) ? null : reader.GetString(reader.GetOrdinal("categoria")),
                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                MetadataJSON = reader.IsDBNull(reader.GetOrdinal("metadataJSON")) ? null : reader.GetString(reader.GetOrdinal("metadataJSON")),
                Activo = reader.IsDBNull(reader.GetOrdinal("activo")) ? null : reader.GetBoolean(reader.GetOrdinal("activo")),
                FechaCreacion = reader.IsDBNull(reader.GetOrdinal("fechaCreacion")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaCreacion")),
                FechaModificacion = reader.IsDBNull(reader.GetOrdinal("fechaModificacion")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaModificacion")),
                UsuarioCreacion = reader.IsDBNull(reader.GetOrdinal("usuarioCreacion")) ? null : reader.GetString(reader.GetOrdinal("usuarioCreacion")),
                UsuarioModificacion = reader.IsDBNull(reader.GetOrdinal("usuarioModificacion")) ? null : reader.GetString(reader.GetOrdinal("usuarioModificacion"))
            };
        }

        /// <summary>
        /// Crea una nueva ubicación
        /// </summary>
        public async Task<object> CreateUbicacionAsync(UbicacionDto ubicacion)
        {
            if (ubicacion == null)
                throw new ArgumentNullException(nameof(ubicacion));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("SP_Ubicacion_CRUD", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@Operacion", "Create_Ubicacion");
                command.Parameters.AddWithValue("@idUbicacion", DBNull.Value);
                command.Parameters.AddWithValue("@nombre", (object?)ubicacion.Nombre ?? DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)ubicacion.Descripcion ?? DBNull.Value);
                AddDecimalParameter(command, "@latitud", ubicacion.Latitud);
                AddDecimalParameter(command, "@longitud", ubicacion.Longitud);
                command.Parameters.AddWithValue("@direccionCompleta", (object?)ubicacion.DireccionCompleta ?? DBNull.Value);
                command.Parameters.AddWithValue("@ciudad", (object?)ubicacion.Ciudad ?? DBNull.Value);
                command.Parameters.AddWithValue("@estado", (object?)ubicacion.Estado ?? DBNull.Value);
                command.Parameters.AddWithValue("@pais", (object?)ubicacion.Pais ?? DBNull.Value);
                command.Parameters.AddWithValue("@placeId", (object?)ubicacion.PlaceId ?? DBNull.Value);
                command.Parameters.AddWithValue("@icono", (object?)ubicacion.Icono ?? DBNull.Value);
                command.Parameters.AddWithValue("@colorIcono", (object?)ubicacion.ColorIcono ?? DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", (object?)ubicacion.NivelZoom ?? DBNull.Value);
                command.Parameters.AddWithValue("@infoWindowHTML", (object?)ubicacion.InfoWindowHTML ?? DBNull.Value);
                command.Parameters.AddWithValue("@categoria", (object?)ubicacion.Categoria ?? DBNull.Value);
                command.Parameters.AddWithValue("@telefono", (object?)ubicacion.Telefono ?? DBNull.Value);
                command.Parameters.AddWithValue("@email", (object?)ubicacion.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", (object?)ubicacion.MetadataJSON ?? DBNull.Value);
                command.Parameters.AddWithValue("@activo", (object?)ubicacion.Activo ?? DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", (object?)ubicacion.UsuarioCreacion ?? DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var idUbicacion = reader.IsDBNull(reader.GetOrdinal("idUbicacion")) ? 0 : reader.GetInt32(reader.GetOrdinal("idUbicacion"));
                    var exito = reader.IsDBNull(reader.GetOrdinal("exito")) ? 0 : reader.GetInt32(reader.GetOrdinal("exito"));
                    var mensaje = reader.IsDBNull(reader.GetOrdinal("mensaje")) ? "" : reader.GetString(reader.GetOrdinal("mensaje"));

                    _logger.LogDebug("Create ubicación devolvió: idUbicacion={IdUbicacion}, exito={Exito}, mensaje={Mensaje}", idUbicacion, exito, mensaje);

                    return new 
                    { 
                        success = exito == 1, 
                        message = mensaje, 
                        idUbicacion = idUbicacion 
                    };
                }

                return new { success = false, message = "No se recibió respuesta del procedimiento almacenado" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear ubicación. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear ubicación en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear ubicación");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una ubicación por su ID
        /// </summary>
        public async Task<Ubicacion?> GetUbicacionByIdAsync(int idUbicacion)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("SP_Ubicacion_CRUD", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operacion", "Select_Ubicacion");
                command.Parameters.AddWithValue("@idUbicacion", idUbicacion);
                command.Parameters.AddWithValue("@nombre", DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@latitud", DBNull.Value);
                command.Parameters.AddWithValue("@longitud", DBNull.Value);
                command.Parameters.AddWithValue("@direccionCompleta", DBNull.Value);
                command.Parameters.AddWithValue("@ciudad", DBNull.Value);
                command.Parameters.AddWithValue("@estado", DBNull.Value);
                command.Parameters.AddWithValue("@pais", DBNull.Value);
                command.Parameters.AddWithValue("@placeId", DBNull.Value);
                command.Parameters.AddWithValue("@icono", DBNull.Value);
                command.Parameters.AddWithValue("@colorIcono", DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", DBNull.Value);
                command.Parameters.AddWithValue("@infoWindowHTML", DBNull.Value);
                command.Parameters.AddWithValue("@categoria", DBNull.Value);
                command.Parameters.AddWithValue("@telefono", DBNull.Value);
                command.Parameters.AddWithValue("@email", DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", DBNull.Value);
                command.Parameters.AddWithValue("@activo", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // Check if result has exito column (error case)
                    try
                    {
                        var exitoOrdinal = reader.GetOrdinal("exito");
                        if (!reader.IsDBNull(exitoOrdinal))
                        {
                            var exito = reader.GetInt32(exitoOrdinal);
                            if (exito == 0)
                            {
                                _logger.LogWarning("GetUbicacionById devolvió error");
                                return null;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // Column doesn't exist, continue with normal processing
                        _logger.LogDebug("Column 'exito' not found, proceeding with normal data mapping");
                    }

                    var ubicacion = MapReaderToUbicacion(reader);
                    _logger.LogDebug("Se obtuvo ubicación con ID {IdUbicacion}", ubicacion.IdUbicacion);
                    return ubicacion;
                }

                return null;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener ubicación por ID. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener ubicación de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener ubicación por ID");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una ubicación por su nombre exacto
        /// </summary>
        public async Task<Ubicacion?> GetUbicacionByNameAsync(string nombre)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("SP_Ubicacion_CRUD", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operacion", "Select_By_Name");
                command.Parameters.AddWithValue("@idUbicacion", DBNull.Value);
                command.Parameters.AddWithValue("@nombre", nombre);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@latitud", DBNull.Value);
                command.Parameters.AddWithValue("@longitud", DBNull.Value);
                command.Parameters.AddWithValue("@direccionCompleta", DBNull.Value);
                command.Parameters.AddWithValue("@ciudad", DBNull.Value);
                command.Parameters.AddWithValue("@estado", DBNull.Value);
                command.Parameters.AddWithValue("@pais", DBNull.Value);
                command.Parameters.AddWithValue("@placeId", DBNull.Value);
                command.Parameters.AddWithValue("@icono", DBNull.Value);
                command.Parameters.AddWithValue("@colorIcono", DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", DBNull.Value);
                command.Parameters.AddWithValue("@infoWindowHTML", DBNull.Value);
                command.Parameters.AddWithValue("@categoria", DBNull.Value);
                command.Parameters.AddWithValue("@telefono", DBNull.Value);
                command.Parameters.AddWithValue("@email", DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", DBNull.Value);
                command.Parameters.AddWithValue("@activo", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // Check if result has exito column (error case)
                    try
                    {
                        var exitoOrdinal = reader.GetOrdinal("exito");
                        if (!reader.IsDBNull(exitoOrdinal))
                        {
                            var exito = reader.GetInt32(exitoOrdinal);
                            if (exito == 0)
                            {
                                _logger.LogWarning("GetUbicacionByName devolvió error");
                                return null;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // Column doesn't exist, continue with normal processing
                        _logger.LogDebug("Column 'exito' not found, proceeding with normal data mapping");
                    }

                    var ubicacion = MapReaderToUbicacion(reader);
                    _logger.LogDebug("Se obtuvo ubicación con nombre {Nombre}", ubicacion.Nombre);
                    return ubicacion;
                }

                return null;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener ubicación por nombre. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener ubicación de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener ubicación por nombre");
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las ubicaciones activas
        /// </summary>
        public async Task<List<Ubicacion>> GetAllUbicacionesAsync()
        {
            var ubicaciones = new List<Ubicacion>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("SP_Ubicacion_CRUD", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operacion", "Select_All_Ubicaciones");
                command.Parameters.AddWithValue("@idUbicacion", DBNull.Value);
                command.Parameters.AddWithValue("@nombre", DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@latitud", DBNull.Value);
                command.Parameters.AddWithValue("@longitud", DBNull.Value);
                command.Parameters.AddWithValue("@direccionCompleta", DBNull.Value);
                command.Parameters.AddWithValue("@ciudad", DBNull.Value);
                command.Parameters.AddWithValue("@estado", DBNull.Value);
                command.Parameters.AddWithValue("@pais", DBNull.Value);
                command.Parameters.AddWithValue("@placeId", DBNull.Value);
                command.Parameters.AddWithValue("@icono", DBNull.Value);
                command.Parameters.AddWithValue("@colorIcono", DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", DBNull.Value);
                command.Parameters.AddWithValue("@infoWindowHTML", DBNull.Value);
                command.Parameters.AddWithValue("@categoria", DBNull.Value);
                command.Parameters.AddWithValue("@telefono", DBNull.Value);
                command.Parameters.AddWithValue("@email", DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", DBNull.Value);
                command.Parameters.AddWithValue("@activo", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var ubicacion = MapReaderToUbicacion(reader);
                    ubicaciones.Add(ubicacion);
                }

                _logger.LogDebug("Se obtuvieron {Count} ubicaciones", ubicaciones.Count);
                return ubicaciones;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener ubicaciones. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener ubicaciones de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener ubicaciones");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una ubicación existente
        /// </summary>
        public async Task<object> UpdateUbicacionAsync(UbicacionDto ubicacion)
        {
            if (ubicacion == null)
                throw new ArgumentNullException(nameof(ubicacion));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("SP_Ubicacion_CRUD", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operacion", "Update_Ubicacion");
                command.Parameters.AddWithValue("@idUbicacion", (object?)ubicacion.IdUbicacion ?? DBNull.Value);
                command.Parameters.AddWithValue("@nombre", (object?)ubicacion.Nombre ?? DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)ubicacion.Descripcion ?? DBNull.Value);
                AddDecimalParameter(command, "@latitud", ubicacion.Latitud);
                AddDecimalParameter(command, "@longitud", ubicacion.Longitud);
                command.Parameters.AddWithValue("@direccionCompleta", (object?)ubicacion.DireccionCompleta ?? DBNull.Value);
                command.Parameters.AddWithValue("@ciudad", (object?)ubicacion.Ciudad ?? DBNull.Value);
                command.Parameters.AddWithValue("@estado", (object?)ubicacion.Estado ?? DBNull.Value);
                command.Parameters.AddWithValue("@pais", (object?)ubicacion.Pais ?? DBNull.Value);
                command.Parameters.AddWithValue("@placeId", (object?)ubicacion.PlaceId ?? DBNull.Value);
                command.Parameters.AddWithValue("@icono", (object?)ubicacion.Icono ?? DBNull.Value);
                command.Parameters.AddWithValue("@colorIcono", (object?)ubicacion.ColorIcono ?? DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", (object?)ubicacion.NivelZoom ?? DBNull.Value);
                command.Parameters.AddWithValue("@infoWindowHTML", (object?)ubicacion.InfoWindowHTML ?? DBNull.Value);
                command.Parameters.AddWithValue("@categoria", (object?)ubicacion.Categoria ?? DBNull.Value);
                command.Parameters.AddWithValue("@telefono", (object?)ubicacion.Telefono ?? DBNull.Value);
                command.Parameters.AddWithValue("@email", (object?)ubicacion.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", (object?)ubicacion.MetadataJSON ?? DBNull.Value);
                command.Parameters.AddWithValue("@activo", (object?)ubicacion.Activo ?? DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", (object?)ubicacion.UsuarioModificacion ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var exito = reader.IsDBNull(reader.GetOrdinal("exito")) ? 0 : reader.GetInt32(reader.GetOrdinal("exito"));
                    var mensaje = reader.IsDBNull(reader.GetOrdinal("mensaje")) ? "" : reader.GetString(reader.GetOrdinal("mensaje"));

                    _logger.LogDebug("Update ubicación devolvió: exito={Exito}, mensaje={Mensaje}", exito, mensaje);

                    return new 
                    { 
                        success = exito == 1, 
                        message = mensaje 
                    };
                }

                return new { success = false, message = "No se recibió respuesta del procedimiento almacenado" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar ubicación. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar ubicación en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar ubicación");
                throw;
            }
        }

        /// <summary>
        /// Elimina físicamente una ubicación
        /// </summary>
        public async Task<object> DeleteUbicacionAsync(int idUbicacion)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("SP_Ubicacion_CRUD", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Operacion", "Delete_Ubicacion");
                command.Parameters.AddWithValue("@idUbicacion", idUbicacion);
                command.Parameters.AddWithValue("@nombre", DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@latitud", DBNull.Value);
                command.Parameters.AddWithValue("@longitud", DBNull.Value);
                command.Parameters.AddWithValue("@direccionCompleta", DBNull.Value);
                command.Parameters.AddWithValue("@ciudad", DBNull.Value);
                command.Parameters.AddWithValue("@estado", DBNull.Value);
                command.Parameters.AddWithValue("@pais", DBNull.Value);
                command.Parameters.AddWithValue("@placeId", DBNull.Value);
                command.Parameters.AddWithValue("@icono", DBNull.Value);
                command.Parameters.AddWithValue("@colorIcono", DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", DBNull.Value);
                command.Parameters.AddWithValue("@infoWindowHTML", DBNull.Value);
                command.Parameters.AddWithValue("@categoria", DBNull.Value);
                command.Parameters.AddWithValue("@telefono", DBNull.Value);
                command.Parameters.AddWithValue("@email", DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", DBNull.Value);
                command.Parameters.AddWithValue("@activo", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var exito = reader.IsDBNull(reader.GetOrdinal("exito")) ? 0 : reader.GetInt32(reader.GetOrdinal("exito"));
                    var mensaje = reader.IsDBNull(reader.GetOrdinal("mensaje")) ? "" : reader.GetString(reader.GetOrdinal("mensaje"));

                    _logger.LogDebug("Delete ubicación devolvió: exito={Exito}, mensaje={Mensaje}", exito, mensaje);

                    return new 
                    { 
                        success = exito == 1, 
                        message = mensaje 
                    };
                }

                return new { success = false, message = "No se recibió respuesta del procedimiento almacenado" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar ubicación. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar ubicación de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar ubicación");
                throw;
            }
        }
    }
}
