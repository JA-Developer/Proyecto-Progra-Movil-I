namespace PrediccionAsistenciaAPI.Models
{
    public class ModeloMl
    {
        public int ModeloId { get; set; }
        public string CodigoVersion { get; set; } = string.Empty;
        public string Algoritmo { get; set; } = "LOGISTIC_REGRESSION";
        public string Enfoque { get; set; } = "ESTADISTICO_SIMPLE";
        public double RocAuc { get; set; }
        public double PrAuc { get; set; }
        public double Sensibilidad { get; set; }
        public double Especificidad { get; set; }
        public double PrecisionVpp { get; set; }
        public double F1Score { get; set; }
        public double BrierScore { get; set; }
        public double EceCalibracion { get; set; }
        public string CurvaCalibracion { get; set; } = "[]";
        public string Hiperparametros { get; set; } = "{}";
        public int ActivoEnProduccion { get; set; } = 0;
        public string? FechaEntrenamiento { get; set; }
    }
}
