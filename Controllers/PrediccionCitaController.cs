using Microsoft.AspNetCore.Mvc;
using PrediccionAsistenciaAPI.Models;
using PrediccionAsistenciaAPI.Repositories;

namespace PrediccionAsistenciaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrediccionCitaController : ControllerBase
    {
        private readonly PrediccionCitaRepository _repositorio;

        public PrediccionCitaController(PrediccionCitaRepository repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<IActionResult> GetPredicciones()
        {
            var predicciones = await _repositorio.ObtenerTodosAsync();
            return Ok(predicciones);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrediccionPorId(int id)
        {
            var prediccion = await _repositorio.ObtenerPorIdAsync(id);
            if (prediccion == null)
                return NotFound($"No se encontró una predicción con Id {id}");

            return Ok(prediccion);
        }

        [HttpPost]
        public async Task<IActionResult> CrearPrediccion([FromBody] PrediccionCita prediccion)
        {
            try
            {
                int nuevoId = await _repositorio.CrearAsync(prediccion);
                var prediccionCreada = await _repositorio.ObtenerPorIdAsync(nuevoId);
                return Ok(new { mensaje = "Predicción creada correctamente", id = nuevoId, data = prediccionCreada });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear predicción: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPrediccion(int id, [FromBody] PrediccionCita prediccion)
        {
            try
            {
                int filasAfectadas = await _repositorio.ActualizarAsync(id, prediccion);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró una predicción con Id {id}");

                return Ok("Predicción actualizada correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar predicción: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPrediccion(int id)
        {
            try
            {
                int filasAfectadas = await _repositorio.EliminarAsync(id);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró una predicción con Id {id}");

                return Ok("Predicción eliminada correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar predicción: {ex.Message}");
            }
        }
    }
}
