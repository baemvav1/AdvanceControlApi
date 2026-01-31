namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para la información de usuario obtenida del procedimiento almacenado sp_contacto_usuario_select
    /// </summary>
    public class ContactoUsuarioDto
    {
        /// <summary>
        /// ID de la credencial del usuario
        /// Tipo: int
        /// </summary>
        public int CredencialId { get; set; }

        /// <summary>
        /// Nombre completo del usuario (nombre + apellido)
        /// Tipo: nvarchar(max)
        /// </summary>
        public string NombreCompleto { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del usuario
        /// Tipo: nvarchar(100)
        /// </summary>
        public string Correo { get; set; } = string.Empty;

        /// <summary>
        /// Teléfono del usuario
        /// Tipo: nvarchar(100)
        /// </summary>
        public string Telefono { get; set; } = string.Empty;

        /// <summary>
        /// Nivel del usuario
        /// Tipo: int
        /// </summary>
        public int Nivel { get; set; }

        /// <summary>
        /// Tipo de usuario
        /// Tipo: nvarchar(100)
        /// </summary>
        public string TipoUsuario { get; set; } = string.Empty;

        /// <summary>
        /// IdProveedor
        /// Tipo: nvarchar(100)
        /// </summary>
        public int IdProveedor { get; set; } = 0;
    }
}
