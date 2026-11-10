using PrediccionAsistenciaAPI.Contexts;
using PrediccionAsistenciaAPI.Models;
using System.Data.Common;

namespace PrediccionAsistenciaAPI.Repositories
{
    public class PsicologoRepository
    {
        private readonly ConexionSQLite _conexion;

        public PsicologoRepository(ConexionSQLite conexion)
        {
            _conexion = conexion;
        }

        public async Task<List<Psicologo>> ObtenerTodosAsync()
        {
            var lista = new List<Psicologo>();

            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT psicologo_id, codigo_psicologo, nombre_completo, rol_clinico, activo, created_at
                        FROM psicologos
                        ORDER BY psicologo_id DESC;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new Psicologo
                            {
                                PsicologoId = reader.GetInt32(0),
                                CodigoPsicologo = reader.GetString(1),
                                NombreCompleto = reader.GetString(2),
                                RolClinico = reader.GetString(3),
                                Activo = reader.GetInt32(4),
                                CreatedAt = reader.IsDBNull(5) ? null : reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Psicologo?> ObtenerPorIdAsync(int id)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT psicologo_id, codigo_psicologo, nombre_completo, rol_clinico, activo, created_at
                        FROM psicologos
                        WHERE psicologo_id = @id;";

                    AgregarParametro(command, "@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Psicologo
                            {
                                PsicologoId = reader.GetInt32(0),
                                CodigoPsicologo = reader.GetString(1),
                                NombreCompleto = reader.GetString(2),
                                RolClinico = reader.GetString(3),
                                Activo = reader.GetInt32(4),
                                CreatedAt = reader.IsDBNull(5) ? null : reader.GetString(5)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<int> CrearAsync(Psicologo psicologo)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        INSERT INTO psicologos (codigo_psicologo, nombre_completo, rol_clinico, activo)
                        VALUES (@codigo_psicologo, @nombre_completo, @rol_clinico, @activo);
                        SELECT last_insert_rowid();";

                    AgregarParametro(command, "@codigo_psicologo", psicologo.CodigoPsicologo);
                    AgregarParametro(command, "@nombre_completo", psicologo.NombreCompleto);
                    AgregarParametro(command, "@rol_clinico", psicologo.RolClinico);
                    AgregarParametro(command, "@activo", psicologo.Activo);

                    var resultado = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(resultado);
                }
            }
        }

        public async Task<int> ActualizarAsync(int id, Psicologo psicologo)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        UPDATE psicologos
                        SET codigo_psicologo = @codigo_psicologo,
                            nombre_completo = @nombre_completo,
                            rol_clinico = @rol_clinico,
                            activo = @activo
                        WHERE psicologo_id = @id;";

                    AgregarParametro(command, "@codigo_psicologo", psicologo.CodigoPsicologo);
                    AgregarParametro(command, "@nombre_completo", psicologo.NombreCompleto);
                    AgregarParametro(command, "@rol_clinico", psicologo.RolClinico);
                    AgregarParametro(command, "@activo", psicologo.Activo);
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
                        DELETE FROM psicologos WHERE psicologo_id = @id;";

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
