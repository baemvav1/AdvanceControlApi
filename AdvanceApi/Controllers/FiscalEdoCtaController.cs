using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/fiscalEdoCta")]
    [Authorize]
    public class FiscalEdoCtaController : ControllerBase
    {
        private readonly IFiscalEdoCtaService _fiscalEdoCtaService;
        private readonly ILogger<FiscalEdoCtaController> _logger;

        public FiscalEdoCtaController(IFiscalEdoCtaService fiscalEdoCtaService, ILogger<FiscalEdoCtaController> logger)
        {
            _fiscalEdoCtaService = fiscalEdoCtaService ?? throw new ArgumentNullException(nameof(fiscalEdoCtaService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo timbre fiscal
        /// POST /api/fiscalEdoCta/timbre
        /// </summary>
        /// <param name="idEstadoCuenta">ID del estado de cuenta (obligatorio)</param>
        /// <param name="uuid">UUID del timbre fiscal (obligatorio)</param>
        /// <param name="fechaTimbrado">Fecha de timbrado (obligatorio)</param>
        /// <param name="numeroProveedor">Número de proveedor (opcional)</param>
        /// <returns>Resultado de la operación con el ID del timbre fiscal creado</returns>
        [HttpPost("timbre")]
        public async Task<IActionResult> CreateTimbreFiscal(
            [FromQuery] int idEstadoCuenta,
            [FromQuery] string uuid,
            [FromQuery] DateTime fechaTimbrado,
            [FromQuery] string? numeroProveedor = null)
        {
            try
            {
                if (idEstadoCuenta <= 0)
                {
                    return BadRequest(new { message = "El campo 'idEstadoCuenta' debe ser mayor a 0." });
                }

                if (string.IsNullOrWhiteSpace(uuid))
                {
                    return BadRequest(new { message = "El campo 'uuid' es obligatorio." });
                }

                var dto = new TimbreFiscalCreateDto
                {
                    IdEstadoCuenta = idEstadoCuenta,
                    Uuid = uuid,
                    FechaTimbrado = fechaTimbrado,
                    NumeroProveedor = numeroProveedor
                };

                var result = await _fiscalEdoCtaService.CrearTimbreFiscalAsync(dto);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear timbre fiscal");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear timbre fiscal");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Consulta timbres fiscales según los criterios de búsqueda proporcionados
        /// GET /api/fiscalEdoCta/timbres
        /// </summary>
        /// <param name="idEstadoCuenta">ID del estado de cuenta (opcional)</param>
        /// <param name="uuid">UUID del timbre fiscal (opcional)</param>
        /// <returns>Lista de timbres fiscales que cumplen con los criterios</returns>
        [HttpGet("timbres")]
        public async Task<IActionResult> GetTimbresFiscales(
            [FromQuery] int? idEstadoCuenta = null,
            [FromQuery] string? uuid = null)
        {
            try
            {
                var timbres = await _fiscalEdoCtaService.ConsultarTimbresFiscalesAsync(idEstadoCuenta, uuid);

                return Ok(timbres);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener timbres fiscales");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener timbres fiscales");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo complemento fiscal
        /// POST /api/fiscalEdoCta/complemento
        /// </summary>
        /// <param name="idEstadoCuenta">ID del estado de cuenta (obligatorio)</param>
        /// <param name="rfc">RFC del contribuyente (obligatorio)</param>
        /// <param name="formaPago">Forma de pago (opcional)</param>
        /// <param name="metodoPago">Método de pago (opcional)</param>
        /// <param name="usoCFDI">Uso de CFDI (opcional)</param>
        /// <param name="claveProducto">Clave del producto (opcional)</param>
        /// <param name="codigoPostal">Código postal (opcional)</param>
        /// <returns>Resultado de la operación con el ID del complemento fiscal creado</returns>
        [HttpPost("complemento")]
        public async Task<IActionResult> CreateComplementoFiscal(
            [FromQuery] int idEstadoCuenta,
            [FromQuery] string rfc,
            [FromQuery] string? formaPago = null,
            [FromQuery] string? metodoPago = null,
            [FromQuery] string? usoCFDI = null,
            [FromQuery] string? claveProducto = null,
            [FromQuery] string? codigoPostal = null)
        {
            try
            {
                if (idEstadoCuenta <= 0)
                {
                    return BadRequest(new { message = "El campo 'idEstadoCuenta' debe ser mayor a 0." });
                }

                if (string.IsNullOrWhiteSpace(rfc))
                {
                    return BadRequest(new { message = "El campo 'rfc' es obligatorio." });
                }

                var dto = new ComplementoFiscalCreateDto
                {
                    IdEstadoCuenta = idEstadoCuenta,
                    Rfc = rfc,
                    FormaPago = formaPago,
                    MetodoPago = metodoPago,
                    UsoCFDI = usoCFDI,
                    ClaveProducto = claveProducto,
                    CodigoPostal = codigoPostal
                };

                var result = await _fiscalEdoCtaService.CrearComplementoFiscalAsync(dto);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear complemento fiscal");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear complemento fiscal");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Consulta complementos fiscales según los criterios de búsqueda proporcionados
        /// GET /api/fiscalEdoCta/complementos
        /// </summary>
        /// <param name="idEstadoCuenta">ID del estado de cuenta (opcional)</param>
        /// <param name="rfc">RFC del contribuyente (opcional)</param>
        /// <returns>Lista de complementos fiscales que cumplen con los criterios</returns>
        [HttpGet("complementos")]
        public async Task<IActionResult> GetComplementosFiscales(
            [FromQuery] int? idEstadoCuenta = null,
            [FromQuery] string? rfc = null)
        {
            try
            {
                var complementos = await _fiscalEdoCtaService.ConsultarComplementosFiscalesAsync(idEstadoCuenta, rfc);

                return Ok(complementos);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener complementos fiscales");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener complementos fiscales");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
