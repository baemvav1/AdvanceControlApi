using AdvanceApi.DTOs;
using AdvanceApi.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace AdvanceApi.Services
{
    public class EstadoCuentaProcedimientosService : IEstadoCuentaProcedimientosService
    {
        // Los procedimientos en ProcedimientosEstadoCuenta.sql declaran DECIMAL(18,2)
        private const byte PrecisionDecimalDefault = 18;
        private const byte EscalaDecimalDefault = 2;

        private static readonly HashSet<string> ProcedimientosPermitidos = new(StringComparer.OrdinalIgnoreCase)
        {
            "sp_CrearEstadoCuenta",
            "sp_EditarEstadoCuenta",
            "sp_ConsultarEstadoCuenta",
            "sp_CrearBanco",
            "sp_ConsultarBanco",
            "sp_CrearCuentaHabiente",
            "sp_ConsultarCuentaHabiente",
            "sp_CrearMovimiento",
            "sp_EditarMovimiento",
            "sp_ConsultarMovimientos",
            "sp_CrearTransferenciaSPEI",
            "sp_ConsultarTransferenciasSPEI",
            "sp_CrearImpuestoMovimiento",
            "sp_ConsultarImpuestosMovimiento",
            "sp_CrearComisionBancaria",
            "sp_ConsultarComisionesBancarias",
            "sp_CrearPagoServicio",
            "sp_ConsultarPagosServicio",
            "sp_CrearDeposito",
            "sp_ConsultarDepositos",
            "sp_CrearTimbreFiscal",
            "sp_ConsultarTimbresFiscales",
            "sp_CrearComplementoFiscal",
            "sp_ConsultarComplementosFiscales",
            "sp_ConsultarEstadoCuentaCompleto",
            "sp_ResumenMovimientosPorTipo",
            "sp_ConsultarTransferenciasPorRFC"
        };

        private readonly DbHelper _dbHelper;
        private readonly ILogger<EstadoCuentaProcedimientosService> _logger;

        public EstadoCuentaProcedimientosService(DbHelper dbHelper, ILogger<EstadoCuentaProcedimientosService> logger)
        {
            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProcedimientoEstadoCuentaResponse> EjecutarAsync(ProcedimientoEstadoCuentaRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var procedimiento = request.Procedimiento?.Trim();

            if (string.IsNullOrWhiteSpace(procedimiento))
                throw new InvalidOperationException("Debe proporcionar el nombre del procedimiento a ejecutar.");

            if (!ProcedimientosPermitidos.Contains(procedimiento))
                throw new InvalidOperationException($"El procedimiento '{procedimiento}' no está permitido.");

            try
            {
                await using var connection = await _dbHelper.GetOpenConnectionAsync();
                await using var command = new SqlCommand(procedimiento, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (request.Parametros != null)
                {
                    foreach (var parametro in request.Parametros)
                    {
                        var nombreParametro = parametro.Key.StartsWith("@") ? parametro.Key : $"@{parametro.Key}";
                        var valor = ConvertirValorParametro(parametro.Value);
                        AgregarParametro(command, nombreParametro, valor);
                    }
                }

                var respuesta = new ProcedimientoEstadoCuentaResponse
                {
                    Procedimiento = procedimiento
                };

                await using var reader = await command.ExecuteReaderAsync();
                do
                {
                    var filas = new List<Dictionary<string, object?>>();
                    while (await reader.ReadAsync())
                    {
                        var fila = new Dictionary<string, object?>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            fila[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }
                        filas.Add(fila);
                    }

                    respuesta.Resultados.Add(filas);
                }
                while (await reader.NextResultAsync());

                _logger.LogDebug("Procedimiento {Procedimiento} ejecutado con {Cantidad} conjunto(s) de resultados", procedimiento, respuesta.Resultados.Count);

                return respuesta;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error SQL al ejecutar el procedimiento {Procedimiento}", procedimiento);
                throw new InvalidOperationException($"Error al ejecutar el procedimiento almacenado '{procedimiento}'.", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al ejecutar el procedimiento {Procedimiento}", procedimiento);
                throw;
            }
        }

        private static object? ConvertirValorParametro(object? valor)
        {
            if (valor == null)
                return DBNull.Value;

            if (valor is JsonElement element)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                        return DBNull.Value;
                    case JsonValueKind.Number:
                        // Se prioriza decimal para conservar precisión en valores monetarios
                        if (element.TryGetDecimal(out var decimalValue)) return decimalValue;
                        if (element.TryGetInt64(out var longValue)) return longValue;
                        if (element.TryGetDouble(out var doubleValue)) return doubleValue;
                        return element.GetRawText();
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        return element.GetBoolean();
                    case JsonValueKind.String:
                        if (element.TryGetDateTime(out var dateValue)) return dateValue;
                        return element.GetString();
                    case JsonValueKind.Array:
                    case JsonValueKind.Object:
                        return element.GetRawText();
                }
            }

            return valor ?? DBNull.Value;
        }

        private static void AgregarParametro(SqlCommand command, string nombre, object? valor)
        {
            var safeValue = valor ?? DBNull.Value;
            var parametro = command.Parameters.Add(nombre, InferirTipoSql(safeValue));
            parametro.Value = safeValue;

            if (safeValue is decimal)
            {
                parametro.Precision = PrecisionDecimalDefault;
                parametro.Scale = EscalaDecimalDefault;
            }
        }

        private static SqlDbType InferirTipoSql(object valor)
        {
            return valor switch
            {
                DBNull => SqlDbType.Variant,
                long => SqlDbType.BigInt,
                int => SqlDbType.Int,
                decimal => SqlDbType.Decimal,
                double => SqlDbType.Float,
                float => SqlDbType.Real,
                bool => SqlDbType.Bit,
                DateTime => SqlDbType.DateTime2,
                _ => SqlDbType.NVarChar
            };
        }
    }
}
