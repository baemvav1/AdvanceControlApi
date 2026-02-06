namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros del procedimiento almacenado sp_entidad_edit
    /// </summary>
    public class EntidadQueryDto
    {
        /// <summary>
        /// Tipo de operación: 'select', 'delete', 'update', 'create'
        /// </summary>
        public string Operacion { get; set; } = string.Empty;

        /// <summary>
        /// ID de la entidad (requerido para delete y update)
        /// </summary>
        public int? IdEntidad { get; set; }

        /// <summary>
        /// Nombre comercial de la entidad
        /// </summary>
        public string? NombreComercial { get; set; }

        /// <summary>
        /// Razón social de la entidad
        /// </summary>
        public string? RazonSocial { get; set; }

        /// <summary>
        /// RFC de la entidad
        /// </summary>
        public string? RFC { get; set; }

        /// <summary>
        /// Código postal de la entidad
        /// </summary>
        public string? CP { get; set; }

        /// <summary>
        /// Estado de la entidad
        /// </summary>
        public string? Estado { get; set; }

        /// <summary>
        /// Ciudad de la entidad
        /// </summary>
        public string? Ciudad { get; set; }

        /// <summary>
        /// País de la entidad
        /// </summary>
        public string? Pais { get; set; }

        /// <summary>
        /// Calle de la entidad
        /// </summary>
        public string? Calle { get; set; }

        /// <summary>
        /// Número exterior de la entidad
        /// </summary>
        public string? NomExt { get; set; }

        /// <summary>
        /// Número interior de la entidad
        /// </summary>
        public string? NumInt { get; set; }

        /// <summary>
        /// Colonia de la entidad
        /// </summary>
        public string? Colonia { get; set; }

        /// <summary>
        /// Apoderado de la entidad
        /// </summary>
        public string? Apoderado { get; set; }

        /// <summary>
        /// Estatus de la entidad (por defecto true)
        /// </summary>
        public bool Estatus { get; set; } = true;
    }
}
