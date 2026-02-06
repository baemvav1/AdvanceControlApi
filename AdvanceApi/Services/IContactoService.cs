using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de contactos
    /// </summary>
    public interface IContactoService
    {
        /// <summary>
        /// Obtiene contactos usando el procedimiento almacenado sp_contacto_edit
        /// </summary>
        /// <param name="query">Parámetros de búsqueda</param>
        /// <returns>Lista de contactos que cumplen con los criterios de búsqueda</returns>
        Task<List<Contacto>> GetContactosAsync(ContactoEditDto query);

        /// <summary>
        /// Crea un nuevo contacto usando el procedimiento almacenado sp_contacto_edit
        /// </summary>
        /// <param name="query">Datos del contacto a crear</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> CreateContactoAsync(ContactoEditDto query);

        /// <summary>
        /// Actualiza un contacto por su ID
        /// </summary>
        /// <param name="query">Datos del contacto a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> UpdateContactoAsync(ContactoEditDto query);

        /// <summary>
        /// Elimina (soft delete) un contacto por su ID
        /// </summary>
        /// <param name="contactoId">ID del contacto a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        Task<object> DeleteContactoAsync(long contactoId);
    }
}
