using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CargosController : ControllerBase
    {
        private readonly ICargoService _cargoService;
        private readonly ILogger<CargosController> _logger;

        public CargosController(ICargoService cargoService, ILogger<CargosController> logger)
        {
            _cargoService = cargoService ?? throw new ArgumentNullException(nameof(cargoService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene cargos según los criterios de búsqueda proporcionados
        /// GET /api/Cargos
        /// </summary>
        /// <param name="idCargo">Búsqueda por ID de cargo específico</param>
        /// <param name="idTipoCargo">Filtro por tipo de cargo</param>
        /// <param name="idRelacionCargo">Filtro por relación de cargo</param>
        /// <param name="monto">Filtro por monto exacto</param>
        /// <param name="nota">Búsqueda parcial en notas</param>
        /// <returns>Lista de cargos que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetCargos(
            [FromQuery] int? idCargo = null,
            [FromQuery] int? idTipoCargo = null,
            [FromQuery] int? idRelacionCargo = null,
            [FromQuery] double? monto = null,
            [FromQuery] string? nota = null)
        {
            try
            {
                var query = new CargoEditDto
                {
                    IdCargo = idCargo ?? 0,
                    IdTipoCargo = idTipoCargo,
                    IdRelacionCargo = idRelacionCargo,
                    Monto = monto,
                    Nota = nota
                };

                var cargos = await _cargoService.GetCargosAsync(query);

                return Ok(cargos);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener cargos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener cargos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo cargo
        /// POST /api/Cargos
        /// </summary>
        /// <param name="idTipoCargo">ID del tipo de cargo (obligatorio)</param>
        /// <param name="idRelacionCargo">ID de la relación del cargo (obligatorio)</param>
        /// <param name="monto">Monto del cargo (obligatorio)</param>
        /// <param name="nota">Nota del cargo (opcional)</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCargo(
            [FromQuery] int idTipoCargo,
            [FromQuery] int idRelacionCargo,
            [FromQuery] double monto,
            [FromQuery] string? nota = null)
        {
            try
            {
                var query = new CargoEditDto
                {
                    IdTipoCargo = idTipoCargo,
                    IdRelacionCargo = idRelacionCargo,
                    Monto = monto,
                    Nota = nota
                };

                var result = await _cargoService.CreateCargoAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear cargo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear cargo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza un cargo existente
        /// PUT /api/Cargos/{id}
        /// </summary>
        /// <param name="id">ID del cargo a actualizar</param>
        /// <param name="idTipoCargo">Nuevo ID del tipo de cargo</param>
        /// <param name="idRelacionCargo">Nuevo ID de la relación del cargo</param>
        /// <param name="monto">Nuevo monto del cargo</param>
        /// <param name="nota">Nueva nota del cargo</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCargo(
            int id,
            [FromQuery] int? idTipoCargo = null,
            [FromQuery] int? idRelacionCargo = null,
            [FromQuery] double? monto = null,
            [FromQuery] string? nota = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var query = new CargoEditDto
                {
                    IdCargo = id,
                    IdTipoCargo = idTipoCargo,
                    IdRelacionCargo = idRelacionCargo,
                    Monto = monto,
                    Nota = nota
                };

                var result = await _cargoService.UpdateCargoAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar cargo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar cargo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina un cargo por su ID
        /// DELETE /api/Cargos/{id}
        /// </summary>
        /// <param name="id">ID del cargo a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCargo(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var result = await _cargoService.DeleteCargoAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar cargo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar cargo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
