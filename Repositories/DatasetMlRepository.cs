using PrediccionAsistenciaAPI.Contexts;
using PrediccionAsistenciaAPI.Models;
using System.Data.Common;

namespace PrediccionAsistenciaAPI.Repositories
{
    public class DatasetMlRepository
    {
        private readonly ConexionSQLite _conexion;

        public DatasetMlRepository(ConexionSQLite conexion)
        {
            _conexion = conexion;
        }

        public async Task<List<DatasetMl>> ObtenerDatasetCompletoAsync()
        {
            var lista = new List<DatasetMl>();

            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT cita_id, dia_cita, hora_cita, psicologo_id, paciente_codigo_anonimo,
                               num_sesiones_previas, asistencia_historica, inasistencias,
                               dias_desde_ultima_sesion, num_citas_anteriores, anticipacion_cita_dias,
                               dia_semana, mes, modalidad, es_primera_cita, y_asistencia
                        FROM vw_dataset_ml
                        ORDER BY cita_id ASC;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapearDataset(reader));
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<DatasetMl?> ObtenerPorCitaIdAsync(int citaId)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT cita_id, dia_cita, hora_cita, psicologo_id, paciente_codigo_anonimo,
                               num_sesiones_previas, asistencia_historica, inasistencias,
                               dias_desde_ultima_sesion, num_citas_anteriores, anticipacion_cita_dias,
                               dia_semana, mes, modalidad, es_primera_cita, y_asistencia
                        FROM vw_dataset_ml
                        WHERE cita_id = @cita_id;";

                    var parametro = command.CreateParameter();
                    parametro.ParameterName = "@cita_id";
                    parametro.Value = citaId;
                    command.Parameters.Add(parametro);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapearDataset(reader);
                        }
                    }
                }
            }

            return null;
        }

        private static DatasetMl MapearDataset(DbDataReader reader)
        {
            return new DatasetMl
            {
                CitaId = reader.GetInt32(0),
                DiaCita = reader.GetString(1),
                HoraCita = reader.GetDouble(2),
                PsicologoId = reader.GetInt32(3),
                PacienteCodigoAnonimo = reader.GetString(4),
                NumSesionesPrevias = reader.GetInt32(5),
                AsistenciaHistorica = reader.GetDouble(6),
                Inasistencias = reader.GetInt32(7),
                DiasDesdeUltimaSesion = reader.IsDBNull(8) ? null : reader.GetDouble(8),
                NumCitasAnteriores = reader.GetInt32(9),
                AnticipacionCitaDias = reader.GetDouble(10),
                DiaSemana = reader.GetInt32(11),
                Mes = reader.GetInt32(12),
                Modalidad = reader.GetString(13),
                EsPrimeraCita = reader.GetInt32(14),
                YAsistencia = reader.IsDBNull(15) ? null : reader.GetInt32(15)
            };
        }
    }
}
