using Microsoft.AspNetCore.Mvc;
using PrediccionAsistenciaAPI.Models;
using PrediccionAsistenciaAPI.Repositories;

namespace PrediccionAsistenciaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PsicologoController : ControllerBase
    {
        private readonly PsicologoRepository _repositorio;

        public PsicologoController(PsicologoRepository repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<IActionResult> GetPsicologos()
        {
            var psicologos = await _repositorio.ObtenerTodosAsync();
            return Ok(psicologos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPsicologoPorId(int id)
        {
            var psicologo = await _repositorio.ObtenerPorIdAsync(id);
            if (psicologo == null)
                return NotFound($"No se encontró un psicólogo con Id {id}");

            return Ok(psicologo);
        }

        [HttpPost]
        public async Task<IActionResult> CrearPsicologo([FromBody] Psicologo psicologo)
        {
            try
            {
                int nuevoId = await _repositorio.CrearAsync(psicologo);
                psicologo.PsicologoId = nuevoId;
                return Ok(new { mensaje = "Psicólogo creado correctamente", id = nuevoId, data = psicologo });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear psicólogo: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPsicologo(int id, [FromBody] Psicologo psicologo)
        {
            try
            {
                int filasAfectadas = await _repositorio.ActualizarAsync(id, psicologo);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró un psicólogo con Id {id}");

                return Ok("Psicólogo actualizado correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar psicólogo: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPsicologo(int id)
        {
            try
            {
                int filasAfectadas = await _repositorio.EliminarAsync(id);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró un psicólogo con Id {id}");

                return Ok("Psicólogo eliminado correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar psicólogo: {ex.Message}");
            }
        }
    }
}
