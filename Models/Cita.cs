namespace PrediccionAsistenciaAPI.Models
{
    public class Cita
    {
        public int CitaId { get; set; }
        public int PacienteId { get; set; }
        public int PsicologoId { get; set; }
        public string Modalidad { get; set; } = "PRESENCIAL";
        public string FechaHoraSolicitud { get; set; } = string.Empty;
        public string FechaHoraCita { get; set; } = string.Empty;
        public double CostoSesion { get; set; } = 500.00;
        public string EstadoCita { get; set; } = "PROGRAMADA";
        public int? YAsistencia { get; set; }
        public string? CreatedAt { get; set; }
    }
}
