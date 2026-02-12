using AdvanceApi.DTOs;

namespace AdvanceApi.Services
{
    public interface IEstadoCuentaProcedimientosService
    {
        Task<ProcedimientoEstadoCuentaResponse> EjecutarAsync(ProcedimientoEstadoCuentaRequest request);
    }
}
