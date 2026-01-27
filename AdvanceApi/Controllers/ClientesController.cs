using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IClienteService clienteService, ILogger<ClientesController> logger)
        {
            _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene clientes según los criterios de búsqueda proporcionados
        /// GET /api/Clientes
        /// </summary>
        /// <param name="rfc">Búsqueda parcial por RFC</param>
        /// <param name="razonSocial">Búsqueda parcial por razón social</param>
        /// <param name="nombreComercial">Búsqueda parcial por nombre comercial</param>
        /// <param name="regimenFiscal">Búsqueda parcial por régimen fiscal</param>
        /// <param name="usoCfdi">Búsqueda parcial por uso CFDI</param>
        /// <param name="notas">Búsqueda parcial en notas</param>
        /// <param name="prioridad">Coincidencia exacta de prioridad</param>
        /// <param name="credencialId">Coincidencia exacta de credencial_id</param>
        /// <returns>Lista de clientes que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetClientes(
            [FromQuery] string? rfc = null,
            [FromQuery] string? razonSocial = null,
            [FromQuery] string? nombreComercial = null,
            [FromQuery] string? regimenFiscal = null,
            [FromQuery] string? usoCfdi = null,
            [FromQuery] string? notas = null,
            [FromQuery] int? prioridad = null,
            [FromQuery] int? credencialId = null)
        {
            try
            {
                var query = new ClienteEditDto
                {
                    Rfc = rfc,
                    RazonSocial = razonSocial,
                    NombreComercial = nombreComercial,
                    RegimenFiscal = regimenFiscal,
                    UsoCfdi = usoCfdi,
                    Notas = notas,
                    Prioridad = prioridad,
                    CredencialId = credencialId,
                    Estatus = true
                };

                var clientes = await _clienteService.GetClientesAsync(query);

                return Ok(clientes);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener clientes");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener clientes");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo cliente
        /// POST /api/Clientes
        /// </summary>
        /// <param name="rfc">RFC del cliente (obligatorio)</param>
        /// <param name="razonSocial">Razón social del cliente</param>
        /// <param name="nombreComercial">Nombre comercial</param>
        /// <param name="regimenFiscal">Régimen fiscal</param>
        /// <param name="usoCfdi">Uso de CFDI</param>
        /// <param name="diasCredito">Días de crédito</param>
        /// <param name="limiteCredito">Límite de crédito</param>
        /// <param name="prioridad">Prioridad del cliente (por defecto 1)</param>
        /// <param name="credencialId">ID de credencial asociada</param>
        /// <param name="notas">Notas adicionales</param>
        /// <param name="idUsuario">ID del usuario que crea el cliente</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCliente(
            [FromQuery] string rfc,
            [FromQuery] string? razonSocial = null,
            [FromQuery] string? nombreComercial = null,
            [FromQuery] string? regimenFiscal = null,
            [FromQuery] string? usoCfdi = null,
            [FromQuery] int? diasCredito = null,
            [FromQuery] decimal? limiteCredito = null,
            [FromQuery] int? prioridad = 1,
            [FromQuery] int? credencialId = null,
            [FromQuery] string? notas = null,
            [FromQuery] int? idUsuario = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rfc))
                {
                    return BadRequest(new { message = "El campo 'rfc' es obligatorio." });
                }

                var query = new ClienteEditDto
                {
                    Rfc = rfc,
                    RazonSocial = razonSocial,
                    NombreComercial = nombreComercial,
                    RegimenFiscal = regimenFiscal,
                    UsoCfdi = usoCfdi,
                    DiasCredito = diasCredito,
                    LimiteCredito = limiteCredito,
                    Prioridad = prioridad,
                    CredencialId = credencialId,
                    Notas = notas,
                    IdUsuario = idUsuario
                };

                var result = await _clienteService.CreateClienteAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza un cliente existente
        /// PUT /api/Clientes/{id}
        /// </summary>
        /// <param name="id">ID del cliente a actualizar</param>
        /// <param name="rfc">Nuevo RFC del cliente</param>
        /// <param name="razonSocial">Nueva razón social</param>
        /// <param name="nombreComercial">Nuevo nombre comercial</param>
        /// <param name="regimenFiscal">Nuevo régimen fiscal</param>
        /// <param name="usoCfdi">Nuevo uso de CFDI</param>
        /// <param name="diasCredito">Nuevos días de crédito</param>
        /// <param name="limiteCredito">Nuevo límite de crédito</param>
        /// <param name="prioridad">Nueva prioridad</param>
        /// <param name="credencialId">Nueva credencial ID</param>
        /// <param name="notas">Nuevas notas</param>
        /// <param name="idUsuario">ID del usuario que actualiza</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(
            int id,
            [FromQuery] string? rfc = null,
            [FromQuery] string? razonSocial = null,
            [FromQuery] string? nombreComercial = null,
            [FromQuery] string? regimenFiscal = null,
            [FromQuery] string? usoCfdi = null,
            [FromQuery] int? diasCredito = null,
            [FromQuery] decimal? limiteCredito = null,
            [FromQuery] int? prioridad = null,
            [FromQuery] int? credencialId = null,
            [FromQuery] string? notas = null,
            [FromQuery] int? idUsuario = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var query = new ClienteEditDto
                {
                    IdCliente = id,
                    Rfc = rfc,
                    RazonSocial = razonSocial,
                    NombreComercial = nombreComercial,
                    RegimenFiscal = regimenFiscal,
                    UsoCfdi = usoCfdi,
                    DiasCredito = diasCredito,
                    LimiteCredito = limiteCredito,
                    Prioridad = prioridad,
                    CredencialId = credencialId,
                    Notas = notas,
                    IdUsuario = idUsuario
                };

                var result = await _clienteService.UpdateClienteAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) un cliente por su ID
        /// DELETE /api/Clientes/{id}
        /// </summary>
        /// <param name="id">ID del cliente a eliminar</param>
        /// <param name="idUsuario">ID del usuario que realiza la eliminación</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id, [FromQuery] int? idUsuario = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var result = await _clienteService.DeleteClienteAsync(id, idUsuario);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar cliente");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
