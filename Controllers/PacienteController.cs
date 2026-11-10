using Microsoft.AspNetCore.Mvc;
using PrediccionAsistenciaAPI.Models;
using PrediccionAsistenciaAPI.Repositories;

namespace PrediccionAsistenciaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacienteController : ControllerBase
    {
        private readonly PacienteRepository _repositorio;

        public PacienteController(PacienteRepository repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<IActionResult> GetPacientes()
        {
            var pacientes = await _repositorio.ObtenerTodosAsync();
            return Ok(pacientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPacientePorId(int id)
        {
            var paciente = await _repositorio.ObtenerPorIdAsync(id);
            if (paciente == null)
                return NotFound($"No se encontró un paciente con Id {id}");

            return Ok(paciente);
        }

        [HttpPost]
        public async Task<IActionResult> CrearPaciente([FromBody] Paciente paciente)
        {
            try
            {
                int nuevoId = await _repositorio.CrearAsync(paciente);
                paciente.PacienteId = nuevoId;
                return Ok(new { mensaje = "Paciente creado correctamente", id = nuevoId, data = paciente });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear paciente: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPaciente(int id, [FromBody] Paciente paciente)
        {
            try
            {
                int filasAfectadas = await _repositorio.ActualizarAsync(id, paciente);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró un paciente con Id {id}");

                return Ok("Paciente actualizado correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar paciente: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPaciente(int id)
        {
            try
            {
                int filasAfectadas = await _repositorio.EliminarAsync(id);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró un paciente con Id {id}");

                return Ok("Paciente eliminado correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar paciente: {ex.Message}");
            }
        }
    }
}
