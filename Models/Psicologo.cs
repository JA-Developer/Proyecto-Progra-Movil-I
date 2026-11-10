namespace PrediccionAsistenciaAPI.Models
{
    public class Psicologo
    {
        public int PsicologoId { get; set; }
        public string CodigoPsicologo { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string RolClinico { get; set; } = "PRACTICANTE";
        public int Activo { get; set; } = 1;
        public string? CreatedAt { get; set; }
    }
}
