using PrediccionAsistenciaAPI.Contexts;
using PrediccionAsistenciaAPI.Models;
using System.Data.Common;

namespace PrediccionAsistenciaAPI.Repositories
{
    public class CitaRepository
    {
        private readonly ConexionSQLite _conexion;

        public CitaRepository(ConexionSQLite conexion)
        {
            _conexion = conexion;
        }

        public async Task<List<Cita>> ObtenerTodosAsync()
        {
            var lista = new List<Cita>();

            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT cita_id, paciente_id, psicologo_id, modalidad, fecha_hora_solicitud,
                               fecha_hora_cita, costo_sesion, estado_cita, y_asistencia, created_at
                        FROM citas
                        ORDER BY fecha_hora_cita DESC;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapearCita(reader));
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Cita?> ObtenerPorIdAsync(int id)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT cita_id, paciente_id, psicologo_id, modalidad, fecha_hora_solicitud,
                               fecha_hora_cita, costo_sesion, estado_cita, y_asistencia, created_at
                        FROM citas
                        WHERE cita_id = @id;";

                    AgregarParametro(command, "@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapearCita(reader);
                        }
                    }
                }
            }

            return null;
        }

        public async Task<int> CrearAsync(Cita cita)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        INSERT INTO citas (paciente_id, psicologo_id, modalidad, fecha_hora_solicitud, fecha_hora_cita, costo_sesion, estado_cita)
                        VALUES (@paciente_id, @psicologo_id, @modalidad, @fecha_hora_solicitud, @fecha_hora_cita, @costo_sesion, @estado_cita);
                        SELECT last_insert_rowid();";

                    var fechaSolicitud = string.IsNullOrWhiteSpace(cita.FechaHoraSolicitud)
                        ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        : cita.FechaHoraSolicitud;

                    AgregarParametro(command, "@paciente_id", cita.PacienteId);
                    AgregarParametro(command, "@psicologo_id", cita.PsicologoId);
                    AgregarParametro(command, "@modalidad", cita.Modalidad);
                    AgregarParametro(command, "@fecha_hora_solicitud", fechaSolicitud);
                    AgregarParametro(command, "@fecha_hora_cita", cita.FechaHoraCita);
                    AgregarParametro(command, "@costo_sesion", cita.CostoSesion);
                    AgregarParametro(command, "@estado_cita", cita.EstadoCita);

                    var resultado = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(resultado);
                }
            }
        }

        public async Task<int> ActualizarAsync(int id, Cita cita)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        UPDATE citas
                        SET paciente_id = @paciente_id,
                            psicologo_id = @psicologo_id,
                            modalidad = @modalidad,
                            fecha_hora_solicitud = @fecha_hora_solicitud,
                            fecha_hora_cita = @fecha_hora_cita,
                            costo_sesion = @costo_sesion,
                            estado_cita = @estado_cita
                        WHERE cita_id = @id;";

                    AgregarParametro(command, "@paciente_id", cita.PacienteId);
                    AgregarParametro(command, "@psicologo_id", cita.PsicologoId);
                    AgregarParametro(command, "@modalidad", cita.Modalidad);
                    AgregarParametro(command, "@fecha_hora_solicitud", cita.FechaHoraSolicitud);
                    AgregarParametro(command, "@fecha_hora_cita", cita.FechaHoraCita);
                    AgregarParametro(command, "@costo_sesion", cita.CostoSesion);
                    AgregarParametro(command, "@estado_cita", cita.EstadoCita);
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
                        DELETE FROM citas WHERE cita_id = @id;";

                    AgregarParametro(command, "@id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        private static Cita MapearCita(DbDataReader reader)
        {
            return new Cita
            {
                CitaId = reader.GetInt32(0),
                PacienteId = reader.GetInt32(1),
                PsicologoId = reader.GetInt32(2),
                Modalidad = reader.GetString(3),
                FechaHoraSolicitud = reader.GetString(4),
                FechaHoraCita = reader.GetString(5),
                CostoSesion = reader.GetDouble(6),
                EstadoCita = reader.GetString(7),
                YAsistencia = reader.IsDBNull(8) ? null : reader.GetInt32(8),
                CreatedAt = reader.IsDBNull(9) ? null : reader.GetString(9)
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
