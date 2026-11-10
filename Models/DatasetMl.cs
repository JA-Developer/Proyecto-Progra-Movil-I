namespace PrediccionAsistenciaAPI.Models
{
    public class DatasetMl
    {
        public int CitaId { get; set; }
        public string DiaCita { get; set; } = string.Empty;
        public double HoraCita { get; set; }
        public int PsicologoId { get; set; }
        public string PacienteCodigoAnonimo { get; set; } = string.Empty;
        public int NumSesionesPrevias { get; set; }
        public double AsistenciaHistorica { get; set; }
        public int Inasistencias { get; set; }
        public double? DiasDesdeUltimaSesion { get; set; }
        public int NumCitasAnteriores { get; set; }
        public double AnticipacionCitaDias { get; set; }
        public int DiaSemana { get; set; }
        public int Mes { get; set; }
        public string Modalidad { get; set; } = string.Empty;
        public int EsPrimeraCita { get; set; }
        public int? YAsistencia { get; set; }
    }
}
