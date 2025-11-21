using AdvanceApi.DTOs;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de información de usuario
    /// </summary>
    public interface IContactoUsuarioService
    {
        /// <summary>
        /// Obtiene la información del usuario usando el procedimiento almacenado sp_contacto_usuario_select
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <returns>Información del contacto/usuario o null si no se encuentra</returns>
        Task<ContactoUsuarioDto?> GetContactoUsuarioAsync(string usuario);
    }
}
