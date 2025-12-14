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
        /// <param name="search">Búsqueda en razon_social o nombre_comercial</param>
        /// <param name="rfc">Búsqueda parcial por RFC</param>
        /// <param name="notas">Búsqueda parcial en notas</param>
        /// <param name="prioridad">Coincidencia exacta de prioridad</param>
        /// <returns>Lista de clientes que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetClientes(
            [FromQuery] string? search = null,
            [FromQuery] string? rfc = null,
            [FromQuery] string? notas = null,
            [FromQuery] short? prioridad = null)
        {
            try
            {
                var query = new ClienteQueryDto
                {
                    Search = search,
                    Rfc = rfc,
                    Notas = notas,
                    Prioridad = prioridad
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
    }
}
