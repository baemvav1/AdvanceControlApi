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
        /// Obtiene clientes usando el procedimiento almacenado sp_cliente_edit
        /// </summary>
        /// <param name="query">Parámetros de búsqueda</param>
        /// <returns>Lista de clientes que cumplen con los criterios de búsqueda</returns>
        Task<List<Cliente>> GetClientesAsync(ClienteEditDto query);

        /// <summary>
        /// Crea un nuevo cliente usando el procedimiento almacenado sp_cliente_edit
        /// </summary>
        /// <param name="query">Datos del cliente a crear</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> CreateClienteAsync(ClienteEditDto query);

        /// <summary>
        /// Actualiza un cliente por su ID
        /// </summary>
        /// <param name="query">Datos del cliente a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdateClienteAsync(ClienteEditDto query);

        /// <summary>
        /// Elimina (soft delete) un cliente por su ID
        /// </summary>
        /// <param name="idCliente">ID del cliente a eliminar</param>
        /// <param name="idUsuario">ID del usuario que realiza la operación</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteClienteAsync(int idCliente, int? idUsuario);
    }
}
