PRAGMA foreign_keys = ON;
PRAGMA journal_mode = WAL;

CREATE TABLE IF NOT EXISTS pacientes (
    paciente_id INTEGER PRIMARY KEY AUTOINCREMENT,
    codigo_anonimo TEXT NOT NULL UNIQUE,
    nombre_completo TEXT NOT NULL,
    telefono_contacto TEXT NOT NULL,
    consiente_recordatorios INTEGER NOT NULL DEFAULT 1
        CHECK (consiente_recordatorios IN (0, 1)),
    created_at TEXT NOT NULL DEFAULT (datetime('now', 'localtime'))
);

CREATE TABLE IF NOT EXISTS psicologos (
    psicologo_id INTEGER PRIMARY KEY AUTOINCREMENT,
    codigo_psicologo TEXT NOT NULL UNIQUE,
    nombre_completo TEXT NOT NULL,
    rol_clinico TEXT NOT NULL DEFAULT 'PRACTICANTE'
        CHECK (rol_clinico IN ('PRACTICANTE', 'MAESTRANTE', 'PSICOLOGO_PLANTA', 'SUPERVISOR')),
    activo INTEGER NOT NULL DEFAULT 1 CHECK (activo IN (0, 1)),
    created_at TEXT NOT NULL DEFAULT (datetime('now', 'localtime'))
);

CREATE TABLE IF NOT EXISTS citas (
    cita_id INTEGER PRIMARY KEY AUTOINCREMENT,
    paciente_id INTEGER NOT NULL REFERENCES pacientes(paciente_id) ON DELETE RESTRICT,
    psicologo_id INTEGER NOT NULL REFERENCES psicologos(psicologo_id) ON DELETE RESTRICT,
    modalidad TEXT NOT NULL DEFAULT 'PRESENCIAL'
        CHECK (modalidad IN ('PRESENCIAL', 'VIRTUAL', 'TELEFONICA')),
    fecha_hora_solicitud TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    fecha_hora_cita TEXT NOT NULL,
    costo_sesion REAL NOT NULL DEFAULT 500.00 CHECK (costo_sesion >= 0),
    estado_cita TEXT NOT NULL DEFAULT 'PROGRAMADA'
        CHECK (estado_cita IN (
            'PROGRAMADA',
            'ASISTIO',
            'NO_ASISTIO',
            'CANCELADA_TARDIA',
            'CANCELADA_A_TIEMPO',
            'ANULADA_CLINICA'
        )),
    y_asistencia INTEGER GENERATED ALWAYS AS (
        CASE
            WHEN estado_cita = 'ASISTIO' THEN 1
            WHEN estado_cita IN ('NO_ASISTIO', 'CANCELADA_TARDIA') THEN 0
            ELSE NULL
        END
    ) STORED,
    created_at TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    CONSTRAINT chk_fechas_coherentes CHECK (julianday(fecha_hora_cita) >= julianday(fecha_hora_solicitud))
);

CREATE TABLE IF NOT EXISTS modelos_ml (
    modelo_id INTEGER PRIMARY KEY AUTOINCREMENT,
    codigo_version TEXT NOT NULL UNIQUE,
    algoritmo TEXT NOT NULL
        CHECK (algoritmo IN ('LOGISTIC_REGRESSION', 'RANDOM_FOREST', 'XGBOOST', 'BAYESIAN')),
    enfoque TEXT NOT NULL
        CHECK (enfoque IN ('ESTADISTICO_SIMPLE', 'MACHINE_LEARNING', 'BAYESIANO')),
    roc_auc REAL NOT NULL CHECK (roc_auc BETWEEN 0.0 AND 1.0),
    pr_auc REAL NOT NULL CHECK (pr_auc BETWEEN 0.0 AND 1.0),
    sensibilidad REAL NOT NULL CHECK (sensibilidad BETWEEN 0.0 AND 1.0),
    especificidad REAL NOT NULL CHECK (especificidad BETWEEN 0.0 AND 1.0),
    precision_vpp REAL NOT NULL CHECK (precision_vpp BETWEEN 0.0 AND 1.0),
    f1_score REAL NOT NULL CHECK (f1_score BETWEEN 0.0 AND 1.0),
    brier_score REAL NOT NULL CHECK (brier_score BETWEEN 0.0 AND 1.0),
    ece_calibracion REAL NOT NULL CHECK (ece_calibracion BETWEEN 0.0 AND 1.0),
    curva_calibracion TEXT NOT NULL DEFAULT '[]' CHECK (json_valid(curva_calibracion)),
    hiperparametros TEXT NOT NULL DEFAULT '{}' CHECK (json_valid(hiperparametros)),
    activo_en_produccion INTEGER NOT NULL DEFAULT 0 CHECK (activo_en_produccion IN (0, 1)),
    fecha_entrenamiento TEXT NOT NULL DEFAULT (datetime('now', 'localtime'))
);

CREATE TABLE IF NOT EXISTS predicciones_cita (
    prediccion_id INTEGER PRIMARY KEY AUTOINCREMENT,
    cita_id INTEGER NOT NULL REFERENCES citas(cita_id) ON DELETE CASCADE,
    modelo_id INTEGER NOT NULL REFERENCES modelos_ml(modelo_id) ON DELETE RESTRICT,
    prob_asistencia REAL NOT NULL CHECK (prob_asistencia BETWEEN 0.0 AND 1.0),
    prob_no_asistencia REAL GENERATED ALWAYS AS (ROUND(1.0 - prob_asistencia, 4)) STORED,
    costo_sesion_base REAL NOT NULL DEFAULT 500.00 CHECK (costo_sesion_base >= 0),
    costo_esperado_vacia REAL GENERATED ALWAYS AS (
        ROUND((1.0 - prob_asistencia) * costo_sesion_base, 2)
    ) STORED,
    nivel_riesgo TEXT GENERATED ALWAYS AS (
        CASE
            WHEN (1.0 - prob_asistencia) < 0.15 THEN 'BAJO'
            WHEN (1.0 - prob_asistencia) < 0.30 THEN 'MODERADO'
            WHEN (1.0 - prob_asistencia) < 0.50 THEN 'ALTO'
            ELSE 'CRITICO'
        END
    ) STORED,
    features_snapshot TEXT NOT NULL DEFAULT '{}' CHECK (json_valid(features_snapshot)),
    fecha_prediccion TEXT NOT NULL DEFAULT (datetime('now', 'localtime'))
);

CREATE TABLE IF NOT EXISTS intervenciones_cita (
    intervencion_id INTEGER PRIMARY KEY AUTOINCREMENT,
    cita_id INTEGER NOT NULL REFERENCES citas(cita_id) ON DELETE CASCADE,
    prediccion_id INTEGER REFERENCES predicciones_cita(prediccion_id) ON DELETE SET NULL,
    tipo_accion TEXT NOT NULL
        CHECK (tipo_accion IN ('RECORDATORIO', 'MENSAJE_PERSONALIZADO', 'CONFIRMACION', 'LLAMADA')),
    estado_envio TEXT NOT NULL DEFAULT 'PENDIENTE'
        CHECK (estado_envio IN ('PENDIENTE', 'ENVIADO', 'CONFIRMADO_PACIENTE', 'SOLICITA_REPROGRAMAR', 'SIN_RESPUESTA')),
    fecha_programada TEXT NOT NULL,
    fecha_ejecutada TEXT,
    created_at TEXT NOT NULL DEFAULT (datetime('now', 'localtime'))
);

CREATE INDEX IF NOT EXISTS idx_citas_historial_paciente ON citas (paciente_id, fecha_hora_cita, cita_id);
CREATE INDEX IF NOT EXISTS idx_citas_programadas ON citas (fecha_hora_cita) WHERE estado_cita = 'PROGRAMADA';
CREATE UNIQUE INDEX IF NOT EXISTS idx_unico_modelo_produccion ON modelos_ml (activo_en_produccion) WHERE activo_en_produccion = 1;

CREATE VIEW IF NOT EXISTS vw_dataset_ml AS
WITH historial_punto_en_el_tiempo AS (
    SELECT
        c.cita_id,
        c.paciente_id,
        p.codigo_anonimo AS paciente_codigo_anonimo,
        c.psicologo_id,
        c.modalidad,
        c.fecha_hora_solicitud,
        c.fecha_hora_cita,
        c.costo_sesion,
        c.estado_cita,
        c.y_asistencia,
        COALESCE(SUM(CASE WHEN c.y_asistencia = 1 THEN 1 ELSE 0 END) OVER w_previas, 0) AS num_sesiones_previas,
        COALESCE(SUM(CASE WHEN c.y_asistencia = 0 THEN 1 ELSE 0 END) OVER w_previas, 0) AS num_inasistencias_previas,
        COALESCE(COUNT(c.y_asistencia) OVER w_previas, 0) AS num_citas_anteriores,
        LAG(c.fecha_hora_cita, 1) OVER (
            PARTITION BY c.paciente_id ORDER BY c.fecha_hora_cita ASC, c.cita_id ASC
        ) AS fecha_ultima_cita
    FROM citas c
    JOIN pacientes p ON p.paciente_id = c.paciente_id
    WHERE c.estado_cita NOT IN ('CANCELADA_A_TIEMPO', 'ANULADA_CLINICA')
    WINDOW w_previas AS (
        PARTITION BY c.paciente_id
        ORDER BY c.fecha_hora_cita ASC, c.cita_id ASC
        ROWS BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING
    )
)
SELECT
    cita_id,
    date(fecha_hora_cita) AS dia_cita,
    ROUND(CAST(strftime('%H', fecha_hora_cita) AS REAL) +
          CAST(strftime('%M', fecha_hora_cita) AS REAL) / 60.0, 2) AS hora_cita,
    psicologo_id,
    paciente_codigo_anonimo,
    num_sesiones_previas,
    ROUND((num_sesiones_previas + 3.0) / (num_citas_anteriores + 4.0), 4) AS asistencia_historica,
    num_inasistencias_previas AS inasistencias,
    CASE
        WHEN fecha_ultima_cita IS NULL THEN NULL
        ELSE ROUND(julianday(fecha_hora_cita) - julianday(fecha_ultima_cita), 2)
    END AS dias_desde_ultima_sesion,
    num_citas_anteriores,
    ROUND(julianday(fecha_hora_cita) - julianday(fecha_hora_solicitud), 2) AS anticipacion_cita_dias,
    CASE CAST(strftime('%w', fecha_hora_cita) AS INTEGER)
        WHEN 0 THEN 7
        ELSE CAST(strftime('%w', fecha_hora_cita) AS INTEGER)
    END AS dia_semana,
    CAST(strftime('%m', fecha_hora_cita) AS INTEGER) AS mes,
    modalidad,
    CASE WHEN num_citas_anteriores = 0 THEN 1 ELSE 0 END AS es_primera_cita,
    y_asistencia
FROM historial_punto_en_el_tiempo;
