using Microsoft.AspNetCore.Mvc;
using PrediccionAsistenciaAPI.Models;
using PrediccionAsistenciaAPI.Repositories;

namespace PrediccionAsistenciaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitaController : ControllerBase
    {
        private readonly CitaRepository _repositorio;

        public CitaController(CitaRepository repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<IActionResult> GetCitas()
        {
            var citas = await _repositorio.ObtenerTodosAsync();
            return Ok(citas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCitaPorId(int id)
        {
            var cita = await _repositorio.ObtenerPorIdAsync(id);
            if (cita == null)
                return NotFound($"No se encontró una cita con Id {id}");

            return Ok(cita);
        }

        [HttpPost]
        public async Task<IActionResult> CrearCita([FromBody] Cita cita)
        {
            try
            {
                int nuevoId = await _repositorio.CrearAsync(cita);
                var citaCreada = await _repositorio.ObtenerPorIdAsync(nuevoId);
                return Ok(new { mensaje = "Cita creada correctamente", id = nuevoId, data = citaCreada });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear cita: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCita(int id, [FromBody] Cita cita)
        {
            try
            {
                int filasAfectadas = await _repositorio.ActualizarAsync(id, cita);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró una cita con Id {id}");

                return Ok("Cita actualizada correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar cita: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCita(int id)
        {
            try
            {
                int filasAfectadas = await _repositorio.EliminarAsync(id);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró una cita con Id {id}");

                return Ok("Cita eliminada correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar cita: {ex.Message}");
            }
        }
    }
}
