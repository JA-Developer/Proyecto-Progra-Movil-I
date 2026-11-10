using Microsoft.AspNetCore.Mvc;
using PrediccionAsistenciaAPI.Repositories;

namespace PrediccionAsistenciaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatasetMlController : ControllerBase
    {
        private readonly DatasetMlRepository _repositorio;

        public DatasetMlController(DatasetMlRepository repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<IActionResult> GetDatasetCompleto()
        {
            var dataset = await _repositorio.ObtenerDatasetCompletoAsync();
            return Ok(dataset);
        }

        [HttpGet("{citaId}")]
        public async Task<IActionResult> GetFeaturesPorCitaId(int citaId)
        {
            var registro = await _repositorio.ObtenerPorCitaIdAsync(citaId);
            if (registro == null)
                return NotFound($"No se encontró información en el dataset ML para la cita con Id {citaId}");

            return Ok(registro);
        }
    }
}
