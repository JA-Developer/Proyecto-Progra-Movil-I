using Microsoft.AspNetCore.Mvc;
using PrediccionAsistenciaAPI.Models;
using PrediccionAsistenciaAPI.Repositories;

namespace PrediccionAsistenciaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModeloMlController : ControllerBase
    {
        private readonly ModeloMlRepository _repositorio;

        public ModeloMlController(ModeloMlRepository repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<IActionResult> GetModelos()
        {
            var modelos = await _repositorio.ObtenerTodosAsync();
            return Ok(modelos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetModeloPorId(int id)
        {
            var modelo = await _repositorio.ObtenerPorIdAsync(id);
            if (modelo == null)
                return NotFound($"No se encontró un modelo ML con Id {id}");

            return Ok(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> CrearModelo([FromBody] ModeloMl modelo)
        {
            try
            {
                int nuevoId = await _repositorio.CrearAsync(modelo);
                modelo.ModeloId = nuevoId;
                return Ok(new { mensaje = "Modelo ML creado correctamente", id = nuevoId, data = modelo });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear modelo ML: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarModelo(int id, [FromBody] ModeloMl modelo)
        {
            try
            {
                int filasAfectadas = await _repositorio.ActualizarAsync(id, modelo);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró un modelo ML con Id {id}");

                return Ok("Modelo ML actualizado correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar modelo ML: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarModelo(int id)
        {
            try
            {
                int filasAfectadas = await _repositorio.EliminarAsync(id);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró un modelo ML con Id {id}");

                return Ok("Modelo ML eliminado correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar modelo ML: {ex.Message}");
            }
        }
    }
}
