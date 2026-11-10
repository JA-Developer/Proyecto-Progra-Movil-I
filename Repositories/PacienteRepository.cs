using PrediccionAsistenciaAPI.Contexts;
using PrediccionAsistenciaAPI.Models;
using System.Data.Common;

namespace PrediccionAsistenciaAPI.Repositories
{
    public class PacienteRepository
    {
        private readonly ConexionSQLite _conexion;

        public PacienteRepository(ConexionSQLite conexion)
        {
            _conexion = conexion;
        }

        public async Task<List<Paciente>> ObtenerTodosAsync()
        {
            var lista = new List<Paciente>();

            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT paciente_id, codigo_anonimo, nombre_completo, telefono_contacto, consiente_recordatorios, created_at
                        FROM pacientes
                        ORDER BY paciente_id DESC;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new Paciente
                            {
                                PacienteId = reader.GetInt32(0),
                                CodigoAnonimo = reader.GetString(1),
                                NombreCompleto = reader.GetString(2),
                                TelefonoContacto = reader.GetString(3),
                                ConsienteRecordatorios = reader.GetInt32(4),
                                CreatedAt = reader.IsDBNull(5) ? null : reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Paciente?> ObtenerPorIdAsync(int id)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT paciente_id, codigo_anonimo, nombre_completo, telefono_contacto, consiente_recordatorios, created_at
                        FROM pacientes
                        WHERE paciente_id = @id;";

                    AgregarParametro(command, "@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Paciente
                            {
                                PacienteId = reader.GetInt32(0),
                                CodigoAnonimo = reader.GetString(1),
                                NombreCompleto = reader.GetString(2),
                                TelefonoContacto = reader.GetString(3),
                                ConsienteRecordatorios = reader.GetInt32(4),
                                CreatedAt = reader.IsDBNull(5) ? null : reader.GetString(5)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<int> CrearAsync(Paciente paciente)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        INSERT INTO pacientes (codigo_anonimo, nombre_completo, telefono_contacto, consiente_recordatorios)
                        VALUES (@codigo_anonimo, @nombre_completo, @telefono_contacto, @consiente_recordatorios);
                        SELECT last_insert_rowid();";

                    AgregarParametro(command, "@codigo_anonimo", paciente.CodigoAnonimo);
                    AgregarParametro(command, "@nombre_completo", paciente.NombreCompleto);
                    AgregarParametro(command, "@telefono_contacto", paciente.TelefonoContacto);
                    AgregarParametro(command, "@consiente_recordatorios", paciente.ConsienteRecordatorios);

                    var resultado = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(resultado);
                }
            }
        }

        public async Task<int> ActualizarAsync(int id, Paciente paciente)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        UPDATE pacientes
                        SET codigo_anonimo = @codigo_anonimo,
                            nombre_completo = @nombre_completo,
                            telefono_contacto = @telefono_contacto,
                            consiente_recordatorios = @consiente_recordatorios
                        WHERE paciente_id = @id;";

                    AgregarParametro(command, "@codigo_anonimo", paciente.CodigoAnonimo);
                    AgregarParametro(command, "@nombre_completo", paciente.NombreCompleto);
                    AgregarParametro(command, "@telefono_contacto", paciente.TelefonoContacto);
                    AgregarParametro(command, "@consiente_recordatorios", paciente.ConsienteRecordatorios);
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
                        DELETE FROM pacientes WHERE paciente_id = @id;";

                    AgregarParametro(command, "@id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
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
