namespace AdvanceApi.DTOs
{
    /// <summary>
    /// DTO para los parámetros de búsqueda del procedimiento almacenado sp_operacion_select
    /// </summary>
    public class OperacionQueryDto
    {
        /// <summary>
        /// Filtro exacto por idTipo
        /// </summary>
        public int? IdTipo { get; set; }

        /// <summary>
        /// Filtro exacto por idCliente
        /// </summary>
        public int? IdCliente { get; set; }

        /// <summary>
        /// Filtro exacto por estatus
        /// </summary>
        public bool? Estatus { get; set; }
    }
}
