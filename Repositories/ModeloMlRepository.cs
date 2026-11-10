using PrediccionAsistenciaAPI.Contexts;
using PrediccionAsistenciaAPI.Models;
using System.Data.Common;

namespace PrediccionAsistenciaAPI.Repositories
{
    public class ModeloMlRepository
    {
        private readonly ConexionSQLite _conexion;

        public ModeloMlRepository(ConexionSQLite conexion)
        {
            _conexion = conexion;
        }

        public async Task<List<ModeloMl>> ObtenerTodosAsync()
        {
            var lista = new List<ModeloMl>();

            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT modelo_id, codigo_version, algoritmo, enfoque, roc_auc, pr_auc,
                               sensibilidad, especificidad, precision_vpp, f1_score, brier_score,
                               ece_calibracion, curva_calibracion, hiperparametros, activo_en_produccion, fecha_entrenamiento
                        FROM modelos_ml
                        ORDER BY modelo_id DESC;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(MapearModelo(reader));
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<ModeloMl?> ObtenerPorIdAsync(int id)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT modelo_id, codigo_version, algoritmo, enfoque, roc_auc, pr_auc,
                               sensibilidad, especificidad, precision_vpp, f1_score, brier_score,
                               ece_calibracion, curva_calibracion, hiperparametros, activo_en_produccion, fecha_entrenamiento
                        FROM modelos_ml
                        WHERE modelo_id = @id;";

                    AgregarParametro(command, "@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapearModelo(reader);
                        }
                    }
                }
            }

            return null;
        }

        public async Task<int> CrearAsync(ModeloMl modelo)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        INSERT INTO modelos_ml (
                            codigo_version, algoritmo, enfoque, roc_auc, pr_auc,
                            sensibilidad, especificidad, precision_vpp, f1_score,
                            brier_score, ece_calibracion, curva_calibracion, hiperparametros, activo_en_produccion
                        )
                        VALUES (
                            @codigo_version, @algoritmo, @enfoque, @roc_auc, @pr_auc,
                            @sensibilidad, @especificidad, @precision_vpp, @f1_score,
                            @brier_score, @ece_calibracion, @curva_calibracion, @hiperparametros, @activo_en_produccion
                        );
                        SELECT last_insert_rowid();";

                    AgregarParametro(command, "@codigo_version", modelo.CodigoVersion);
                    AgregarParametro(command, "@algoritmo", modelo.Algoritmo);
                    AgregarParametro(command, "@enfoque", modelo.Enfoque);
                    AgregarParametro(command, "@roc_auc", modelo.RocAuc);
                    AgregarParametro(command, "@pr_auc", modelo.PrAuc);
                    AgregarParametro(command, "@sensibilidad", modelo.Sensibilidad);
                    AgregarParametro(command, "@especificidad", modelo.Especificidad);
                    AgregarParametro(command, "@precision_vpp", modelo.PrecisionVpp);
                    AgregarParametro(command, "@f1_score", modelo.F1Score);
                    AgregarParametro(command, "@brier_score", modelo.BrierScore);
                    AgregarParametro(command, "@ece_calibracion", modelo.EceCalibracion);
                    AgregarParametro(command, "@curva_calibracion", string.IsNullOrWhiteSpace(modelo.CurvaCalibracion) ? "[]" : modelo.CurvaCalibracion);
                    AgregarParametro(command, "@hiperparametros", string.IsNullOrWhiteSpace(modelo.Hiperparametros) ? "{}" : modelo.Hiperparametros);
                    AgregarParametro(command, "@activo_en_produccion", modelo.ActivoEnProduccion);

                    var resultado = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(resultado);
                }
            }
        }

        public async Task<int> ActualizarAsync(int id, ModeloMl modelo)
        {
            using (var connection = _conexion.GetConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        PRAGMA foreign_keys = ON;
                        UPDATE modelos_ml
                        SET codigo_version = @codigo_version,
                            algoritmo = @algoritmo,
                            enfoque = @enfoque,
                            roc_auc = @roc_auc,
                            pr_auc = @pr_auc,
                            sensibilidad = @sensibilidad,
                            especificidad = @especificidad,
                            precision_vpp = @precision_vpp,
                            f1_score = @f1_score,
                            brier_score = @brier_score,
                            ece_calibracion = @ece_calibracion,
                            curva_calibracion = @curva_calibracion,
                            hiperparametros = @hiperparametros,
                            activo_en_produccion = @activo_en_produccion
                        WHERE modelo_id = @id;";

                    AgregarParametro(command, "@codigo_version", modelo.CodigoVersion);
                    AgregarParametro(command, "@algoritmo", modelo.Algoritmo);
                    AgregarParametro(command, "@enfoque", modelo.Enfoque);
                    AgregarParametro(command, "@roc_auc", modelo.RocAuc);
                    AgregarParametro(command, "@pr_auc", modelo.PrAuc);
                    AgregarParametro(command, "@sensibilidad", modelo.Sensibilidad);
                    AgregarParametro(command, "@especificidad", modelo.Especificidad);
                    AgregarParametro(command, "@precision_vpp", modelo.PrecisionVpp);
                    AgregarParametro(command, "@f1_score", modelo.F1Score);
                    AgregarParametro(command, "@brier_score", modelo.BrierScore);
                    AgregarParametro(command, "@ece_calibracion", modelo.EceCalibracion);
                    AgregarParametro(command, "@curva_calibracion", string.IsNullOrWhiteSpace(modelo.CurvaCalibracion) ? "[]" : modelo.CurvaCalibracion);
                    AgregarParametro(command, "@hiperparametros", string.IsNullOrWhiteSpace(modelo.Hiperparametros) ? "{}" : modelo.Hiperparametros);
                    AgregarParametro(command, "@activo_en_produccion", modelo.ActivoEnProduccion);
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
                        DELETE FROM modelos_ml WHERE modelo_id = @id;";

                    AgregarParametro(command, "@id", id);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        private static ModeloMl MapearModelo(DbDataReader reader)
        {
            return new ModeloMl
            {
                ModeloId = reader.GetInt32(0),
                CodigoVersion = reader.GetString(1),
                Algoritmo = reader.GetString(2),
                Enfoque = reader.GetString(3),
                RocAuc = reader.GetDouble(4),
                PrAuc = reader.GetDouble(5),
                Sensibilidad = reader.GetDouble(6),
                Especificidad = reader.GetDouble(7),
                PrecisionVpp = reader.GetDouble(8),
                F1Score = reader.GetDouble(9),
                BrierScore = reader.GetDouble(10),
                EceCalibracion = reader.GetDouble(11),
                CurvaCalibracion = reader.GetString(12),
                Hiperparametros = reader.GetString(13),
                ActivoEnProduccion = reader.GetInt32(14),
                FechaEntrenamiento = reader.IsDBNull(15) ? null : reader.GetString(15)
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
