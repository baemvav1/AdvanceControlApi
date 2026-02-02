using AdvanceApi.DTOs;
using AdvanceApi.Helpers;
using Clases;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Implementación del servicio de áreas que usa el procedimiento almacenado sp_area_edit
    /// </summary>
    public class AreaService : IAreaService
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<AreaService> _logger;

        public AreaService(DbHelper dbHelper, ILogger<AreaService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Agrega un parámetro decimal con tipo explícito para evitar errores de conversión numeric/decimal
        /// </summary>
        /// <param name="command">El comando SQL al cual agregar el parámetro</param>
        /// <param name="parameterName">Nombre del parámetro (ej: @centroLatitud)</param>
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
        /// Obtiene áreas usando el procedimiento almacenado sp_area_edit
        /// </summary>
        public async Task<List<Area>> GetAreasAsync(AreaEditDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var areas = new List<Area>();

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_area_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@operacion", "select");
                command.Parameters.AddWithValue("@idArea", query.IdArea);
                command.Parameters.AddWithValue("@nombre", (object?)query.Nombre ?? DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@colorMapa", DBNull.Value);
                command.Parameters.AddWithValue("@opacidad", DBNull.Value);
                command.Parameters.AddWithValue("@colorBorde", DBNull.Value);
                command.Parameters.AddWithValue("@anchoBorde", DBNull.Value);
                command.Parameters.AddWithValue("@activo", (object?)query.Activo ?? DBNull.Value);
                command.Parameters.AddWithValue("@tipoGeometria", (object?)query.TipoGeometria ?? DBNull.Value);
                command.Parameters.AddWithValue("@centroLatitud", DBNull.Value);
                command.Parameters.AddWithValue("@centroLongitud", DBNull.Value);
                command.Parameters.AddWithValue("@radio", DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaMostrar", DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaTexto", DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", DBNull.Value);
                command.Parameters.AddWithValue("@coordenadas", DBNull.Value);
                command.Parameters.AddWithValue("@autoCalcularCentro", 1);
                command.Parameters.AddWithValue("@validarPoligonoLargo", 1);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var area = new Area
                    {
                        IdArea = reader.GetInt32(reader.GetOrdinal("idArea")),
                        Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion")),
                        ColorMapa = reader.IsDBNull(reader.GetOrdinal("colorMapa")) ? null : reader.GetString(reader.GetOrdinal("colorMapa")),
                        Opacidad = reader.IsDBNull(reader.GetOrdinal("opacidad")) ? null : reader.GetDecimal(reader.GetOrdinal("opacidad")),
                        ColorBorde = reader.IsDBNull(reader.GetOrdinal("colorBorde")) ? null : reader.GetString(reader.GetOrdinal("colorBorde")),
                        AnchoBorde = reader.IsDBNull(reader.GetOrdinal("anchoBorde")) ? null : reader.GetInt32(reader.GetOrdinal("anchoBorde")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("activo")) ? null : reader.GetBoolean(reader.GetOrdinal("activo")),
                        TipoGeometria = reader.IsDBNull(reader.GetOrdinal("tipoGeometria")) ? null : reader.GetString(reader.GetOrdinal("tipoGeometria")),
                        CentroLatitud = reader.IsDBNull(reader.GetOrdinal("centroLatitud")) ? null : reader.GetDecimal(reader.GetOrdinal("centroLatitud")),
                        CentroLongitud = reader.IsDBNull(reader.GetOrdinal("centroLongitud")) ? null : reader.GetDecimal(reader.GetOrdinal("centroLongitud")),
                        Radio = reader.IsDBNull(reader.GetOrdinal("radio")) ? null : reader.GetDecimal(reader.GetOrdinal("radio")),
                        BoundingBoxNE_Lat = reader.IsDBNull(reader.GetOrdinal("boundingBoxNE_Lat")) ? null : reader.GetDecimal(reader.GetOrdinal("boundingBoxNE_Lat")),
                        BoundingBoxNE_Lng = reader.IsDBNull(reader.GetOrdinal("boundingBoxNE_Lng")) ? null : reader.GetDecimal(reader.GetOrdinal("boundingBoxNE_Lng")),
                        BoundingBoxSW_Lat = reader.IsDBNull(reader.GetOrdinal("boundingBoxSW_Lat")) ? null : reader.GetDecimal(reader.GetOrdinal("boundingBoxSW_Lat")),
                        BoundingBoxSW_Lng = reader.IsDBNull(reader.GetOrdinal("boundingBoxSW_Lng")) ? null : reader.GetDecimal(reader.GetOrdinal("boundingBoxSW_Lng")),
                        EtiquetaMostrar = reader.IsDBNull(reader.GetOrdinal("etiquetaMostrar")) ? null : reader.GetBoolean(reader.GetOrdinal("etiquetaMostrar")),
                        EtiquetaTexto = reader.IsDBNull(reader.GetOrdinal("etiquetaTexto")) ? null : reader.GetString(reader.GetOrdinal("etiquetaTexto")),
                        NivelZoom = reader.IsDBNull(reader.GetOrdinal("nivelZoom")) ? null : reader.GetInt32(reader.GetOrdinal("nivelZoom")),
                        MetadataJSON = reader.IsDBNull(reader.GetOrdinal("metadataJSON")) ? null : reader.GetString(reader.GetOrdinal("metadataJSON")),
                        FechaCreacion = reader.IsDBNull(reader.GetOrdinal("fechaCreacion")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaCreacion")),
                        FechaModificacion = reader.IsDBNull(reader.GetOrdinal("fechaModificacion")) ? null : reader.GetDateTime(reader.GetOrdinal("fechaModificacion")),
                        TotalCoordenadas = reader.IsDBNull(reader.GetOrdinal("totalCoordenadas")) ? null : reader.GetInt32(reader.GetOrdinal("totalCoordenadas")),
                        TotalMarcadores = reader.IsDBNull(reader.GetOrdinal("totalMarcadores")) ? null : reader.GetInt32(reader.GetOrdinal("totalMarcadores"))
                    };

                    areas.Add(area);
                }

                _logger.LogDebug("Se obtuvieron {Count} áreas", areas.Count);

                return areas;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener áreas. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener áreas de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener áreas");
                throw;
            }
        }

        /// <summary>
        /// Obtiene áreas en formato optimizado para Google Maps
        /// </summary>
        public async Task<object> GetAreasGoogleMapsAsync(int idArea, bool? activo)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_area_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "select_googlemaps");
                command.Parameters.AddWithValue("@idArea", idArea);
                command.Parameters.AddWithValue("@nombre", DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@colorMapa", DBNull.Value);
                command.Parameters.AddWithValue("@opacidad", DBNull.Value);
                command.Parameters.AddWithValue("@colorBorde", DBNull.Value);
                command.Parameters.AddWithValue("@anchoBorde", DBNull.Value);
                command.Parameters.AddWithValue("@activo", (object?)activo ?? DBNull.Value);
                command.Parameters.AddWithValue("@tipoGeometria", DBNull.Value);
                command.Parameters.AddWithValue("@centroLatitud", DBNull.Value);
                command.Parameters.AddWithValue("@centroLongitud", DBNull.Value);
                command.Parameters.AddWithValue("@radio", DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaMostrar", DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaTexto", DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", DBNull.Value);
                command.Parameters.AddWithValue("@coordenadas", DBNull.Value);
                command.Parameters.AddWithValue("@autoCalcularCentro", 1);
                command.Parameters.AddWithValue("@validarPoligonoLargo", 1);

                await using var reader = await command.ExecuteReaderAsync();

                var results = new List<Dictionary<string, object?>>();
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object?>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        row[reader.GetName(i)] = value;
                    }
                    results.Add(row);
                }

                return results;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener áreas Google Maps. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener áreas de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener áreas Google Maps");
                throw;
            }
        }

        /// <summary>
        /// Obtiene áreas en formato GeoJSON estándar
        /// </summary>
        public async Task<object> GetAreasGeoJsonAsync(int idArea, bool? activo)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_area_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "select_geojson");
                command.Parameters.AddWithValue("@idArea", idArea);
                command.Parameters.AddWithValue("@nombre", DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@colorMapa", DBNull.Value);
                command.Parameters.AddWithValue("@opacidad", DBNull.Value);
                command.Parameters.AddWithValue("@colorBorde", DBNull.Value);
                command.Parameters.AddWithValue("@anchoBorde", DBNull.Value);
                command.Parameters.AddWithValue("@activo", (object?)activo ?? DBNull.Value);
                command.Parameters.AddWithValue("@tipoGeometria", DBNull.Value);
                command.Parameters.AddWithValue("@centroLatitud", DBNull.Value);
                command.Parameters.AddWithValue("@centroLongitud", DBNull.Value);
                command.Parameters.AddWithValue("@radio", DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaMostrar", DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaTexto", DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", DBNull.Value);
                command.Parameters.AddWithValue("@coordenadas", DBNull.Value);
                command.Parameters.AddWithValue("@autoCalcularCentro", 1);
                command.Parameters.AddWithValue("@validarPoligonoLargo", 1);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var jsonString = reader.GetString(0);
                    return JsonSerializer.Deserialize<object>(jsonString) ?? new { };
                }

                return new { };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al obtener áreas GeoJSON. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al obtener áreas de la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener áreas GeoJSON");
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva área usando el procedimiento almacenado sp_area_edit
        /// </summary>
        public async Task<object> CreateAreaAsync(AreaEditDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_area_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "create");
                command.Parameters.AddWithValue("@idArea", 0);
                command.Parameters.AddWithValue("@nombre", (object?)query.Nombre ?? DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@colorMapa", (object?)query.ColorMapa ?? DBNull.Value);
                AddDecimalParameter(command, "@opacidad", query.Opacidad, 5, 2);
                command.Parameters.AddWithValue("@colorBorde", (object?)query.ColorBorde ?? DBNull.Value);
                command.Parameters.AddWithValue("@anchoBorde", (object?)query.AnchoBorde ?? DBNull.Value);
                command.Parameters.AddWithValue("@activo", (object?)query.Activo ?? DBNull.Value);
                command.Parameters.AddWithValue("@tipoGeometria", (object?)query.TipoGeometria ?? DBNull.Value);
                AddDecimalParameter(command, "@centroLatitud", query.CentroLatitud, 28, 8);
                AddDecimalParameter(command, "@centroLongitud", query.CentroLongitud, 28, 8);
                AddDecimalParameter(command, "@radio", query.Radio, 28, 8);
                command.Parameters.AddWithValue("@etiquetaMostrar", (object?)query.EtiquetaMostrar ?? DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaTexto", (object?)query.EtiquetaTexto ?? DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", (object?)query.NivelZoom ?? DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", (object?)query.MetadataJSON ?? DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", (object?)query.UsuarioCreacion ?? DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", DBNull.Value);
                command.Parameters.AddWithValue("@coordenadas", (object?)query.Coordenadas ?? DBNull.Value);
                command.Parameters.AddWithValue("@autoCalcularCentro", (object?)query.AutoCalcularCentro ?? 1);
                command.Parameters.AddWithValue("@validarPoligonoLargo", (object?)query.ValidarPoligonoLargo ?? 1);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var status = reader.GetString(reader.GetOrdinal("Status"));
                    var message = reader.GetString(reader.GetOrdinal("Message"));
                    
                    if (status == "Success")
                    {
                        var idArea = reader.IsDBNull(reader.GetOrdinal("idArea")) ? 0 : reader.GetInt32(reader.GetOrdinal("idArea"));
                        var poligonoCerrado = reader.IsDBNull(reader.GetOrdinal("poligonoCerrado")) ? false : reader.GetBoolean(reader.GetOrdinal("poligonoCerrado"));
                        
                        _logger.LogDebug("Área creada exitosamente: {IdArea}", idArea);
                        return new { success = true, message, idArea, poligonoCerrado };
                    }
                    else
                    {
                        _logger.LogWarning("Error al crear área: {Message}", message);
                        return new { success = false, message };
                    }
                }

                return new { success = true, message = "Área creada exitosamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al crear área. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al crear área en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear área");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un área por su ID
        /// </summary>
        public async Task<object> UpdateAreaAsync(AreaEditDto query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_area_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "update");
                command.Parameters.AddWithValue("@idArea", query.IdArea);
                command.Parameters.AddWithValue("@nombre", (object?)query.Nombre ?? DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", (object?)query.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@colorMapa", (object?)query.ColorMapa ?? DBNull.Value);
                AddDecimalParameter(command, "@opacidad", query.Opacidad, 5, 2);
                command.Parameters.AddWithValue("@colorBorde", (object?)query.ColorBorde ?? DBNull.Value);
                command.Parameters.AddWithValue("@anchoBorde", (object?)query.AnchoBorde ?? DBNull.Value);
                command.Parameters.AddWithValue("@activo", (object?)query.Activo ?? DBNull.Value);
                command.Parameters.AddWithValue("@tipoGeometria", (object?)query.TipoGeometria ?? DBNull.Value);
                AddDecimalParameter(command, "@centroLatitud", query.CentroLatitud, 28, 8);
                AddDecimalParameter(command, "@centroLongitud", query.CentroLongitud, 28, 8);
                AddDecimalParameter(command, "@radio", query.Radio, 28, 8);
                command.Parameters.AddWithValue("@etiquetaMostrar", (object?)query.EtiquetaMostrar ?? DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaTexto", (object?)query.EtiquetaTexto ?? DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", (object?)query.NivelZoom ?? DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", (object?)query.MetadataJSON ?? DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", (object?)query.UsuarioModificacion ?? DBNull.Value);
                command.Parameters.AddWithValue("@coordenadas", (object?)query.Coordenadas ?? DBNull.Value);
                command.Parameters.AddWithValue("@autoCalcularCentro", (object?)query.AutoCalcularCentro ?? 1);
                command.Parameters.AddWithValue("@validarPoligonoLargo", (object?)query.ValidarPoligonoLargo ?? 1);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var status = reader.GetString(reader.GetOrdinal("Status"));
                    var message = reader.GetString(reader.GetOrdinal("Message"));
                    
                    if (status == "Success")
                    {
                        _logger.LogDebug("Área {IdArea} actualizada exitosamente", query.IdArea);
                        return new { success = true, message };
                    }
                    else
                    {
                        _logger.LogWarning("Error al actualizar área: {Message}", message);
                        return new { success = false, message };
                    }
                }

                return new { success = true, message = "Área actualizada exitosamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al actualizar área. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al actualizar área en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar área");
                throw;
            }
        }

        /// <summary>
        /// Elimina (soft delete) un área por su ID
        /// </summary>
        public async Task<object> DeleteAreaAsync(int idArea)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_area_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete");
                command.Parameters.AddWithValue("@idArea", idArea);
                command.Parameters.AddWithValue("@nombre", DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@colorMapa", DBNull.Value);
                command.Parameters.AddWithValue("@opacidad", DBNull.Value);
                command.Parameters.AddWithValue("@colorBorde", DBNull.Value);
                command.Parameters.AddWithValue("@anchoBorde", DBNull.Value);
                command.Parameters.AddWithValue("@activo", DBNull.Value);
                command.Parameters.AddWithValue("@tipoGeometria", DBNull.Value);
                command.Parameters.AddWithValue("@centroLatitud", DBNull.Value);
                command.Parameters.AddWithValue("@centroLongitud", DBNull.Value);
                command.Parameters.AddWithValue("@radio", DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaMostrar", DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaTexto", DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", DBNull.Value);
                command.Parameters.AddWithValue("@coordenadas", DBNull.Value);
                command.Parameters.AddWithValue("@autoCalcularCentro", 1);
                command.Parameters.AddWithValue("@validarPoligonoLargo", 1);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var status = reader.GetString(reader.GetOrdinal("Status"));
                    var message = reader.GetString(reader.GetOrdinal("Message"));
                    
                    if (status == "Success")
                    {
                        _logger.LogDebug("Área {IdArea} eliminada (soft delete)", idArea);
                        return new { success = true, message };
                    }
                    else
                    {
                        _logger.LogWarning("Error al eliminar área: {Message}", message);
                        return new { success = false, message };
                    }
                }

                return new { success = true, message = "Área eliminada exitosamente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar área. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar área en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar área");
                throw;
            }
        }

        /// <summary>
        /// Elimina físicamente un área por su ID
        /// </summary>
        public async Task<object> DeleteAreaPhysicalAsync(int idArea)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_area_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "delete_physical");
                command.Parameters.AddWithValue("@idArea", idArea);
                command.Parameters.AddWithValue("@nombre", DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@colorMapa", DBNull.Value);
                command.Parameters.AddWithValue("@opacidad", DBNull.Value);
                command.Parameters.AddWithValue("@colorBorde", DBNull.Value);
                command.Parameters.AddWithValue("@anchoBorde", DBNull.Value);
                command.Parameters.AddWithValue("@activo", DBNull.Value);
                command.Parameters.AddWithValue("@tipoGeometria", DBNull.Value);
                command.Parameters.AddWithValue("@centroLatitud", DBNull.Value);
                command.Parameters.AddWithValue("@centroLongitud", DBNull.Value);
                command.Parameters.AddWithValue("@radio", DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaMostrar", DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaTexto", DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", DBNull.Value);
                command.Parameters.AddWithValue("@coordenadas", DBNull.Value);
                command.Parameters.AddWithValue("@autoCalcularCentro", 1);
                command.Parameters.AddWithValue("@validarPoligonoLargo", 1);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var status = reader.GetString(reader.GetOrdinal("Status"));
                    var message = reader.GetString(reader.GetOrdinal("Message"));
                    
                    if (status == "Success")
                    {
                        _logger.LogDebug("Área {IdArea} eliminada permanentemente", idArea);
                        return new { success = true, message };
                    }
                    else
                    {
                        _logger.LogWarning("Error al eliminar área físicamente: {Message}", message);
                        return new { success = false, message };
                    }
                }

                return new { success = true, message = "Área eliminada permanentemente" };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al eliminar área físicamente. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al eliminar área en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar área físicamente");
                throw;
            }
        }

        /// <summary>
        /// Valida si un punto está dentro de un polígono
        /// </summary>
        public async Task<object> ValidatePointInPolygonAsync(int idArea, decimal latitud, decimal longitud)
        {
            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand("sp_area_edit", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@operacion", "validate_point_in_polygon");
                command.Parameters.AddWithValue("@idArea", idArea);
                command.Parameters.AddWithValue("@nombre", DBNull.Value);
                command.Parameters.AddWithValue("@descripcion", DBNull.Value);
                command.Parameters.AddWithValue("@colorMapa", DBNull.Value);
                command.Parameters.AddWithValue("@opacidad", DBNull.Value);
                command.Parameters.AddWithValue("@colorBorde", DBNull.Value);
                command.Parameters.AddWithValue("@anchoBorde", DBNull.Value);
                command.Parameters.AddWithValue("@activo", DBNull.Value);
                command.Parameters.AddWithValue("@tipoGeometria", DBNull.Value);
                AddDecimalParameter(command, "@centroLatitud", latitud, 28, 8);
                AddDecimalParameter(command, "@centroLongitud", longitud, 28, 8);
                command.Parameters.AddWithValue("@radio", DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaMostrar", DBNull.Value);
                command.Parameters.AddWithValue("@etiquetaTexto", DBNull.Value);
                command.Parameters.AddWithValue("@nivelZoom", DBNull.Value);
                command.Parameters.AddWithValue("@metadataJSON", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioCreacion", DBNull.Value);
                command.Parameters.AddWithValue("@usuarioModificacion", DBNull.Value);
                command.Parameters.AddWithValue("@coordenadas", DBNull.Value);
                command.Parameters.AddWithValue("@autoCalcularCentro", 1);
                command.Parameters.AddWithValue("@validarPoligonoLargo", 1);

                await using var reader = await command.ExecuteReaderAsync();

                var results = new List<Dictionary<string, object?>>();
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object?>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        row[reader.GetName(i)] = value;
                    }
                    results.Add(row);
                }

                return results;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al validar punto en polígono. SqlError: {Message}", sqlEx.Message);
                throw new InvalidOperationException("Error al validar punto en la base de datos", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al validar punto en polígono");
                throw;
            }
        }
    }
}
