using AdvanceApi.DTOs;
using Clases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanceApi.Services
{
    /// <summary>
    /// Interfaz para el servicio de Fiscal Estado de Cuenta
    /// Maneja operaciones de TimbreFiscal y ComplementoFiscal
    /// </summary>
    public interface IFiscalEdoCtaService
    {
        /// <summary>
        /// Crea un nuevo timbre fiscal usando el procedimiento almacenado sp_CrearTimbreFiscal
        /// </summary>
        /// <param name="dto">Datos del timbre fiscal a crear</param>
        /// <returns>Resultado de la operación con el ID del timbre fiscal creado</returns>
        Task<object> CrearTimbreFiscalAsync(TimbreFiscalCreateDto dto);

        /// <summary>
        /// Consulta timbres fiscales usando el procedimiento almacenado sp_ConsultarTimbresFiscales
        /// </summary>
        /// <param name="idEstadoCuenta">ID del estado de cuenta (opcional)</param>
        /// <param name="uuid">UUID del timbre fiscal (opcional)</param>
        /// <returns>Lista de timbres fiscales que cumplen con los criterios</returns>
        Task<List<TimbreFiscal>> ConsultarTimbresFiscalesAsync(int? idEstadoCuenta = null, string? uuid = null);

        /// <summary>
        /// Crea un nuevo complemento fiscal usando el procedimiento almacenado sp_CrearComplementoFiscal
        /// </summary>
        /// <param name="dto">Datos del complemento fiscal a crear</param>
        /// <returns>Resultado de la operación con el ID del complemento fiscal creado</returns>
        Task<object> CrearComplementoFiscalAsync(ComplementoFiscalCreateDto dto);

        /// <summary>
        /// Consulta complementos fiscales usando el procedimiento almacenado sp_ConsultarComplementosFiscales
        /// </summary>
        /// <param name="idEstadoCuenta">ID del estado de cuenta (opcional)</param>
        /// <param name="rfc">RFC del contribuyente (opcional)</param>
        /// <returns>Lista de complementos fiscales que cumplen con los criterios</returns>
        Task<List<ComplementoFiscal>> ConsultarComplementosFiscalesAsync(int? idEstadoCuenta = null, string? rfc = null);
    }
}
