using PrediccionAsistenciaAPI.Contexts;
using PrediccionAsistenciaAPI.Models;
using System.Data.Common;

namespace PrediccionAsistenciaAPI.Repositories
{
    public class PrediccionCitaRepository
    {
        private readonly ConexionSQLite _conexion;

        public PrediccionCitaRepository(ConexionSQLite conexion)
        {
            _conexion = conexion;
        }

        public async Task<List<PrediccionCita>> ObtenerTodosAsync()
        {
            var lista = new List<PrediccionCita>();

            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT prediccion_id, cita_id, modelo_id, prob_asistencia, prob_no_asistencia,
                               costo_sesion_base, costo_esperado_vacia, nivel_riesgo, features_snapshot, fecha_prediccion
                        FROM predicciones_cita
                        ORDER BY prediccion_id DESC;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapearPrediccion(reader));
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<PrediccionCita?> ObtenerPorIdAsync(int id)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT prediccion_id, cita_id, modelo_id, prob_asistencia, prob_no_asistencia,
                               costo_sesion_base, costo_esperado_vacia, nivel_riesgo, features_snapshot, fecha_prediccion
                        FROM predicciones_cita
                        WHERE prediccion_id = @id;";

                    AgregarParametro(command, "@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapearPrediccion(reader);
                        }
                    }
                }
            }

            return null;
        }

        public async Task<int> CrearAsync(PrediccionCita prediccion)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        INSERT INTO predicciones_cita (cita_id, modelo_id, prob_asistencia, costo_sesion_base, features_snapshot)
                        VALUES (@cita_id, @modelo_id, @prob_asistencia, @costo_sesion_base, @features_snapshot);
                        SELECT last_insert_rowid();";

                    AgregarParametro(command, "@cita_id", prediccion.CitaId);
                    AgregarParametro(command, "@modelo_id", prediccion.ModeloId);
                    AgregarParametro(command, "@prob_asistencia", prediccion.ProbAsistencia);
                    AgregarParametro(command, "@costo_sesion_base", prediccion.CostoSesionBase);
                    AgregarParametro(command, "@features_snapshot", string.IsNullOrWhiteSpace(prediccion.FeaturesSnapshot) ? "{}" : prediccion.FeaturesSnapshot);

                    var resultado = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(resultado);
                }
            }
        }

        public async Task<int> ActualizarAsync(int id, PrediccionCita prediccion)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        UPDATE predicciones_cita
                        SET cita_id = @cita_id,
                            modelo_id = @modelo_id,
                            prob_asistencia = @prob_asistencia,
                            costo_sesion_base = @costo_sesion_base,
                            features_snapshot = @features_snapshot
                        WHERE prediccion_id = @id;";

                    AgregarParametro(command, "@cita_id", prediccion.CitaId);
                    AgregarParametro(command, "@modelo_id", prediccion.ModeloId);
                    AgregarParametro(command, "@prob_asistencia", prediccion.ProbAsistencia);
                    AgregarParametro(command, "@costo_sesion_base", prediccion.CostoSesionBase);
                    AgregarParametro(command, "@features_snapshot", string.IsNullOrWhiteSpace(prediccion.FeaturesSnapshot) ? "{}" : prediccion.FeaturesSnapshot);
                    AgregarParametro(command, "@id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> EliminarAsync(int id)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        DELETE FROM predicciones_cita WHERE prediccion_id = @id;";

                    AgregarParametro(command, "@id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        private static PrediccionCita MapearPrediccion(DbDataReader reader)
        {
            return new PrediccionCita
            {
                PrediccionId = reader.GetInt32(0),
                CitaId = reader.GetInt32(1),
                ModeloId = reader.GetInt32(2),
                ProbAsistencia = reader.GetDouble(3),
                ProbNoAsistencia = reader.IsDBNull(4) ? null : reader.GetDouble(4),
                CostoSesionBase = reader.GetDouble(5),
                CostoEsperadoVacia = reader.IsDBNull(6) ? null : reader.GetDouble(6),
                NivelRiesgo = reader.IsDBNull(7) ? null : reader.GetString(7),
                FeaturesSnapshot = reader.GetString(8),
                FechaPrediccion = reader.IsDBNull(9) ? null : reader.GetString(9)
            };
        }

        private static void AgregarParametro(DbCommand command, string nombre, object? valor)
        {
            var parametro = command.CreateParameter();
            parametro.ParameterName = nombre;
            parametro.Value = valor ?? DBNull.Value;
            command.Parameters.Add(parametro);
        }
    }
}
