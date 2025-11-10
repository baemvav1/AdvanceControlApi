using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de clientes
    /// </summary>
    public interface IClienteService
    {
        /// <summary>
        /// Obtiene clientes usando el procedimiento almacenado sp_cliente_select
        /// </summary>
        /// <param name="query">Parámetros de búsqueda</param>
        /// <returns>Lista de clientes que cumplen con los criterios de búsqueda</returns>
        Task<List<Cliente>> GetClientesAsync(ClienteQueryDto query);
    }
}
