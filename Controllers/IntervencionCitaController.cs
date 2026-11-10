using Microsoft.AspNetCore.Mvc;
using PrediccionAsistenciaAPI.Models;
using PrediccionAsistenciaAPI.Repositories;

namespace PrediccionAsistenciaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntervencionCitaController : ControllerBase
    {
        private readonly IntervencionCitaRepository _repositorio;

        public IntervencionCitaController(IntervencionCitaRepository repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<IActionResult> GetIntervenciones()
        {
            var intervenciones = await _repositorio.ObtenerTodosAsync();
            return Ok(intervenciones);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIntervencionPorId(int id)
        {
            var intervencion = await _repositorio.ObtenerPorIdAsync(id);
            if (intervencion == null)
                return NotFound($"No se encontró una intervención con Id {id}");

            return Ok(intervencion);
        }

        [HttpPost]
        public async Task<IActionResult> CrearIntervencion([FromBody] IntervencionCita intervencion)
        {
            try
            {
                int nuevoId = await _repositorio.CrearAsync(intervencion);
                intervencion.IntervencionId = nuevoId;
                return Ok(new { mensaje = "Intervención creada correctamente", id = nuevoId, data = intervencion });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear intervención: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarIntervencion(int id, [FromBody] IntervencionCita intervencion)
        {
            try
            {
                int filasAfectadas = await _repositorio.ActualizarAsync(id, intervencion);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró una intervención con Id {id}");

                return Ok("Intervención actualizada correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar intervención: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarIntervencion(int id)
        {
            try
            {
                int filasAfectadas = await _repositorio.EliminarAsync(id);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró una intervención con Id {id}");

                return Ok("Intervención eliminada correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar intervención: {ex.Message}");
            }
        }
    }
}
