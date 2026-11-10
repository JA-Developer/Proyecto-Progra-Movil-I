namespace PrediccionAsistenciaAPI.Models
{
    public class PrediccionCita
    {
        public int PrediccionId { get; set; }
        public int CitaId { get; set; }
        public int ModeloId { get; set; }
        public double ProbAsistencia { get; set; }
        public double? ProbNoAsistencia { get; set; }
        public double CostoSesionBase { get; set; } = 500.00;
        public double? CostoEsperadoVacia { get; set; }
        public string? NivelRiesgo { get; set; }
        public string FeaturesSnapshot { get; set; } = "{}";
        public string? FechaPrediccion { get; set; }
    }
}
