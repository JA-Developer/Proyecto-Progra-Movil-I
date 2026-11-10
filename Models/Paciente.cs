namespace PrediccionAsistenciaAPI.Models
{
    public class Paciente
    {
        public int PacienteId { get; set; }
        public string CodigoAnonimo { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string TelefonoContacto { get; set; } = string.Empty;
        public int ConsienteRecordatorios { get; set; } = 1;
        public string? CreatedAt { get; set; }
    }
}
