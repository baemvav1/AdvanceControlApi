namespace AdvanceApi.DTOs
{
    public class ProcedimientoEstadoCuentaRequest
    {
        public string Procedimiento { get; set; } = string.Empty;

        public Dictionary<string, object?>? Parametros { get; set; }
    }

    public class ProcedimientoEstadoCuentaResponse
    {
        public string Procedimiento { get; set; } = string.Empty;

        public List<List<Dictionary<string, object?>>> Resultados { get; set; } = new();
    }
}
