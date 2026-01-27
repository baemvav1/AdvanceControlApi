using AdvanceApi.DTOs;
using Clases;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de cargos
    /// </summary>
    public interface ICargoService
    {
        /// <summary>
        /// Obtiene cargos según los criterios especificados
        /// </summary>
        Task<List<Cargo>> GetCargosAsync(CargoEditDto query);

        /// <summary>
        /// Crea un nuevo cargo
        /// </summary>
        Task<object> CreateCargoAsync(CargoEditDto query);

        /// <summary>
        /// Actualiza un cargo existente
        /// </summary>
        Task<object> UpdateCargoAsync(CargoEditDto query);

        /// <summary>
        /// Elimina un cargo
        /// </summary>
        Task<object> DeleteCargoAsync(int idCargo);
    }
}
