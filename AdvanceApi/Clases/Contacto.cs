using System;

namespace Clases
{
    /// <summary>
    /// Modelo de entidad para la tabla contacto
    /// </summary>
    public class Contacto
    {
        /// <summary>
        /// ID del contacto
        /// </summary>
        public long ContactoId { get; set; }

        /// <summary>
        /// ID de credencial asociada
        /// </summary>
        public long? CredencialId { get; set; }

        /// <summary>
        /// Nombre del contacto
        /// </summary>
        public string? Nombre { get; set; }

        /// <summary>
        /// Apellido del contacto
        /// </summary>
        public string? Apellido { get; set; }

        /// <summary>
        /// Correo electrónico del contacto
        /// </summary>
        public string? Correo { get; set; }

        /// <summary>
        /// Teléfono del contacto
        /// </summary>
        public string? Telefono { get; set; }

        /// <summary>
        /// Departamento del contacto
        /// </summary>
        public string? Departamento { get; set; }

        /// <summary>
        /// Código interno del contacto
        /// </summary>
        public string? CodigoInterno { get; set; }

        /// <summary>
        /// Indica si el contacto está activo
        /// </summary>
        public bool? Activo { get; set; }

        /// <summary>
        /// Notas adicionales
        /// </summary>
        public string? Notas { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime? CreadoEn { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? ActualizadoEn { get; set; }

        /// <summary>
        /// ID del proveedor asociado
        /// </summary>
        public int? IdProveedor { get; set; }

        /// <summary>
        /// Cargo del contacto
        /// </summary>
        public string? Cargo { get; set; }

        /// <summary>
        /// ID del cliente asociado
        /// </summary>
        public int? IdCliente { get; set; }

        /// <summary>
        /// Estatus del contacto
        /// </summary>
        public bool? Estatus { get; set; }
    }
}
