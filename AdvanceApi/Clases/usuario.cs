using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clases
{
    /// <summary>
    /// Representa las credenciales de inicio de sesión del usuario
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 150 caracteres")]
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "La contraseña debe tener entre 4 y 100 caracteres")]
        public string Password { get; set; } = string.Empty;
    }
}
