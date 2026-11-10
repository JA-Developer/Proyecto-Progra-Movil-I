namespace PrediccionAsistenciaAPI.Models
{
    public class IntervencionCita
    {
        public int IntervencionId { get; set; }
        public int CitaId { get; set; }
        public int? PrediccionId { get; set; }
        public string TipoAccion { get; set; } = "RECORDATORIO";
        public string EstadoEnvio { get; set; } = "PENDIENTE";
        public string FechaProgramada { get; set; } = string.Empty;
        public string? FechaEjecutada { get; set; }
        public string? CreatedAt { get; set; }
    }
}
