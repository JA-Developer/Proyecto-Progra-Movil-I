using PrediccionAsistenciaAPI.Contexts;
using PrediccionAsistenciaAPI.Models;
using System.Data.Common;

namespace PrediccionAsistenciaAPI.Repositories
{
    public class IntervencionCitaRepository
    {
        private readonly ConexionSQLite _conexion;

        public IntervencionCitaRepository(ConexionSQLite conexion)
        {
            _conexion = conexion;
        }

        public async Task<List<IntervencionCita>> ObtenerTodosAsync()
        {
            var lista = new List<IntervencionCita>();

            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT intervencion_id, cita_id, prediccion_id, tipo_accion, estado_envio,
                               fecha_programada, fecha_ejecutada, created_at
                        FROM intervenciones_cita
                        ORDER BY intervencion_id DESC;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapearIntervencion(reader));
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<IntervencionCita?> ObtenerPorIdAsync(int id)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT intervencion_id, cita_id, prediccion_id, tipo_accion, estado_envio,
                               fecha_programada, fecha_ejecutada, created_at
                        FROM intervenciones_cita
                        WHERE intervencion_id = @id;";

                    AgregarParametro(command, "@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapearIntervencion(reader);
                        }
                    }
                }
            }

            return null;
        }

        public async Task<int> CrearAsync(IntervencionCita intervencion)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        INSERT INTO intervenciones_cita (cita_id, prediccion_id, tipo_accion, estado_envio, fecha_programada, fecha_ejecutada)
                        VALUES (@cita_id, @prediccion_id, @tipo_accion, @estado_envio, @fecha_programada, @fecha_ejecutada);
                        SELECT last_insert_rowid();";

                    AgregarParametro(command, "@cita_id", intervencion.CitaId);
                    AgregarParametro(command, "@prediccion_id", intervencion.PrediccionId);
                    AgregarParametro(command, "@tipo_accion", intervencion.TipoAccion);
                    AgregarParametro(command, "@estado_envio", intervencion.EstadoEnvio);
                    AgregarParametro(command, "@fecha_programada", intervencion.FechaProgramada);
                    AgregarParametro(command, "@fecha_ejecutada", intervencion.FechaEjecutada);

                    var resultado = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(resultado);
                }
            }
        }

        public async Task<int> ActualizarAsync(int id, IntervencionCita intervencion)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        UPDATE intervenciones_cita
                        SET cita_id = @cita_id,
                            prediccion_id = @prediccion_id,
                            tipo_accion = @tipo_accion,
                            estado_envio = @estado_envio,
                            fecha_programada = @fecha_programada,
                            fecha_ejecutada = @fecha_ejecutada
                        WHERE intervencion_id = @id;";

                    AgregarParametro(command, "@cita_id", intervencion.CitaId);
                    AgregarParametro(command, "@prediccion_id", intervencion.PrediccionId);
                    AgregarParametro(command, "@tipo_accion", intervencion.TipoAccion);
                    AgregarParametro(command, "@estado_envio", intervencion.EstadoEnvio);
                    AgregarParametro(command, "@fecha_programada", intervencion.FechaProgramada);
                    AgregarParametro(command, "@fecha_ejecutada", intervencion.FechaEjecutada);
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
                        DELETE FROM intervenciones_cita WHERE intervencion_id = @id;";

                    AgregarParametro(command, "@id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        private static IntervencionCita MapearIntervencion(DbDataReader reader)
        {
            return new IntervencionCita
            {
                IntervencionId = reader.GetInt32(0),
                CitaId = reader.GetInt32(1),
                PrediccionId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                TipoAccion = reader.GetString(3),
                EstadoEnvio = reader.GetString(4),
                FechaProgramada = reader.GetString(5),
                FechaEjecutada = reader.IsDBNull(6) ? null : reader.GetString(6),
                CreatedAt = reader.IsDBNull(7) ? null : reader.GetString(7)
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
