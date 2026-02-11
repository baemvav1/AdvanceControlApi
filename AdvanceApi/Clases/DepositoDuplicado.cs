using System;

namespace Clases
{
    /// <summary>
    /// Modelo para el resultado de búsqueda de duplicados
    /// </summary>
    public class DepositoDuplicado
    {
        public DateTime? Fecha { get; set; }
        public decimal? Monto { get; set; }
        public int Cantidad { get; set; }
        public string? Descripciones { get; set; }
        public string? Tipos { get; set; }
    }
}
