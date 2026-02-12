using AdvanceApi.DTOs;
using Clases;

namespace AdvanceApi.Services
{
    public interface IBancoCtaHabienteService
    {
        // Banco methods
        Task<object> CreateBancoAsync(BancoDto banco);
        Task<List<Banco>> GetBancosAsync(BancoDto query);

        // CuentaHabiente methods
        Task<object> CreateCuentaHabienteAsync(CuentaHabienteDto cuentaHabiente);
        Task<List<CuentaHabiente>> GetCuentasHabienteAsync(CuentaHabienteDto query);
    }
}
