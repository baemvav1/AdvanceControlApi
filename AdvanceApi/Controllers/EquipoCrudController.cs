using AdvanceApi.DTOs;
using AdvanceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvanceApi.Controllers
{
    [ApiController]
    [Route("api/equipo_crud")]
    [Authorize]
    public class EquipoCrudController : ControllerBase
    {
        private readonly IEquipoService _equipoService;
        private readonly ILogger<EquipoCrudController> _logger;

        public EquipoCrudController(IEquipoService equipoService, ILogger<EquipoCrudController> logger)
        {
            _equipoService = equipoService ?? throw new ArgumentNullException(nameof(equipoService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene equipos según los criterios de búsqueda proporcionados
        /// GET /api/equipo_crud
        /// </summary>
        /// <param name="marca">Búsqueda parcial por marca</param>
        /// <param name="creado">Filtro exacto por año de creación</param>
        /// <param name="paradas">Filtro exacto por número de paradas</param>
        /// <param name="kilogramos">Filtro exacto por capacidad en kilogramos</param>
        /// <param name="personas">Filtro exacto por capacidad de personas</param>
        /// <param name="descripcion">Búsqueda parcial en descripción</param>
        /// <param name="identificador">Búsqueda parcial por identificador</param>
        /// <returns>Lista de equipos que cumplen con los criterios</returns>
        [HttpGet]
        public async Task<IActionResult> GetEquipos(
            [FromQuery] string? marca = null,
            [FromQuery] int? creado = null,
            [FromQuery] int? paradas = null,
            [FromQuery] int? kilogramos = null,
            [FromQuery] int? personas = null,
            [FromQuery] string? descripcion = null,
            [FromQuery] string? identificador = null)
        {
            try
            {
                var query = new EquipoQueryDto
                {
                    Operacion = "select",
                    IdEquipo = 0,
                    Marca = marca,
                    Creado = creado,
                    Paradas = paradas,
                    Kilogramos = kilogramos,
                    Personas = personas,
                    Descripcion = descripcion,
                    Identificador = identificador,
                    Estatus = true
                };

                var equipos = await _equipoService.ExecuteEquipoOperationAsync(query);

                return Ok(equipos);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al obtener equipos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener equipos");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Elimina (soft delete) un equipo por su ID
        /// DELETE /api/equipo_crud/{id}
        /// </summary>
        /// <param name="id">ID del equipo a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var result = await _equipoService.DeleteEquipoAsync(id);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al eliminar equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Actualiza un equipo existente
        /// PUT /api/equipo_crud/{id}
        /// </summary>
        /// <param name="id">ID del equipo a actualizar</param>
        /// <param name="marca">Nueva marca del equipo</param>
        /// <param name="creado">Nuevo año de creación</param>
        /// <param name="paradas">Nuevo número de paradas</param>
        /// <param name="kilogramos">Nueva capacidad en kilogramos</param>
        /// <param name="personas">Nueva capacidad de personas</param>
        /// <param name="descripcion">Nueva descripción</param>
        /// <param name="identificador">Nuevo identificador</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEquipo(
            int id,
            [FromQuery] string? marca = null,
            [FromQuery] int? creado = null,
            [FromQuery] int? paradas = null,
            [FromQuery] int? kilogramos = null,
            [FromQuery] int? personas = null,
            [FromQuery] string? descripcion = null,
            [FromQuery] string? identificador = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id Invalido" });
                }

                var query = new EquipoQueryDto
                {
                    Operacion = "update",
                    IdEquipo = id,
                    Marca = marca,
                    Creado = creado,
                    Paradas = paradas,
                    Kilogramos = kilogramos,
                    Personas = personas,
                    Descripcion = descripcion,
                    Identificador = identificador,
                    Estatus = true
                };

                var result = await _equipoService.UpdateEquipoAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al actualizar equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }

        /// <summary>
        /// Crea un nuevo equipo
        /// POST /api/equipo_crud
        /// </summary>
        /// <param name="marca">Marca del equipo (obligatorio)</param>
        /// <param name="creado">Año de creación (opcional, default 0)</param>
        /// <param name="paradas">Número de paradas (opcional, default 0)</param>
        /// <param name="kilogramos">Capacidad en kilogramos (opcional, default 0)</param>
        /// <param name="personas">Capacidad de personas (opcional, default 0)</param>
        /// <param name="descripcion">Descripción del equipo (opcional)</param>
        /// <param name="identificador">Identificador único del equipo (obligatorio)</param>
        /// <param name="estatus">Estatus del equipo (opcional, default true)</param>
        /// <returns>Resultado de la operación con el equipo creado</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEquipo(
            [FromQuery] string marca,
            [FromQuery] int creado = 0,
            [FromQuery] int paradas = 0,
            [FromQuery] int kilogramos = 0,
            [FromQuery] int personas = 0,
            [FromQuery] string? descripcion = null,
            [FromQuery] string identificador = "",
            [FromQuery] bool estatus = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(marca))
                {
                    return BadRequest(new { message = "El campo 'marca' es obligatorio." });
                }

                if (string.IsNullOrWhiteSpace(identificador))
                {
                    return BadRequest(new { message = "El campo 'identificador' es obligatorio." });
                }

                var query = new EquipoQueryDto
                {
                    Marca = marca,
                    Creado = creado,
                    Paradas = paradas,
                    Kilogramos = kilogramos,
                    Personas = personas,
                    Descripcion = descripcion,
                    Identificador = identificador,
                    Estatus = estatus
                };

                var result = await _equipoService.CreateEquipoAsync(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error al crear equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message, innerMessage = ex.InnerException?.Message });
#else
                return StatusCode(500, new { message = "Error al acceder a la base de datos." });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear equipo");
#if DEBUG
                return StatusCode(500, new { message = ex.Message });
#else
                return StatusCode(500, new { message = "Error interno del servidor." });
#endif
            }
        }
    }
}
