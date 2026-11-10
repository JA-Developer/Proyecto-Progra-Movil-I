-- =============================================================================
-- DATOS DE PRUEBA (SEED DATA) - CLÍNICA PSICOLÓGICA UNIVERSITARIA
-- =============================================================================

PRAGMA foreign_keys = ON;

-- Limpiar datos previos manteniendo reinicio de autoincrementales
DELETE FROM intervenciones_cita;
DELETE FROM predicciones_cita;
DELETE FROM modelos_ml;
DELETE FROM citas;
DELETE FROM psicologos;
DELETE FROM pacientes;
DELETE FROM sqlite_sequence WHERE name IN ('pacientes', 'psicologos', 'citas', 'modelos_ml', 'predicciones_cita', 'intervenciones_cita');

-- -----------------------------------------------------------------------------
-- 1. PACIENTES (8 pacientes con códigos anónimos para investigación)
-- -----------------------------------------------------------------------------
INSERT INTO pacientes (codigo_anonimo, nombre_completo, telefono_contacto, consiente_recordatorios) VALUES
('PAC-UNAH-001', 'Andrea Carolina Mejía', '+504 9912-3401', 1),
('PAC-UNAH-002', 'Carlos Eduardo Valladares', '+504 9845-1122', 1),
('PAC-UNAH-003', 'Sofía Alejandra Pineda', '+504 9760-8890', 1),
('PAC-UNAH-004', 'Jorge Luis Hernández', '+504 3321-4509', 1),
('PAC-UNAH-005', 'Valeria Nicolle Zelaya', '+504 9510-7734', 1),
('PAC-UNAH-006', 'Diego Alejandro Flores', '+504 8834-2190', 1),
('PAC-UNAH-007', 'Gabriela María Reyes', '+504 9601-5543', 1),
('PAC-UNAH-008', 'Kevin Mauricio Castillo', '+504 9478-6612', 0);

-- -----------------------------------------------------------------------------
-- 2. PSICÓLOGOS (4 terapeutas de la clínica universitaria)
-- -----------------------------------------------------------------------------
INSERT INTO psicologos (codigo_psicologo, nombre_completo, rol_clinico, activo) VALUES
('PSI-CLIN-01', 'Lic. Elena Banegas', 'SUPERVISOR', 1),
('PSI-CLIN-02', 'MSc. Roberto Figueroa', 'PSICOLOGO_PLANTA', 1),
('PSI-MAES-03', 'Lic. Diana Maradiaga', 'MAESTRANTE', 1),
('PSI-PRAC-04', 'Br. Fernando Paz', 'PRACTICANTE', 1);

-- -----------------------------------------------------------------------------
-- 3. CITAS HISTÓRICAS Y FUTURAS (Para alimentar las 14 variables y ventanas)
-- -----------------------------------------------------------------------------
-- Paciente 1 (PAC-UNAH-001): Excelente historial (3 asistencias previas, 1 cita programada futura)
INSERT INTO citas (paciente_id, psicologo_id, modalidad, fecha_hora_solicitud, fecha_hora_cita, costo_sesion, estado_cita) VALUES
(1, 2, 'PRESENCIAL', '2026-09-25 10:00:00', '2026-10-01 09:00:00', 500.00, 'ASISTIO'),
(1, 2, 'PRESENCIAL', '2026-10-01 10:00:00', '2026-10-08 09:00:00', 500.00, 'ASISTIO'),
(1, 2, 'VIRTUAL',    '2026-10-08 10:00:00', '2026-10-15 09:00:00', 500.00, 'ASISTIO'),
(1, 2, 'PRESENCIAL', '2026-11-05 11:00:00', '2026-11-12 09:00:00', 500.00, 'PROGRAMADA'); -- Cita ID 4 (Prob ~82%)

-- Paciente 2 (PAC-UNAH-002): Historial mixto con inasistencias (1 asistió, 2 no asistió, 1 cita programada futura)
INSERT INTO citas (paciente_id, psicologo_id, modalidad, fecha_hora_solicitud, fecha_hora_cita, costo_sesion, estado_cita) VALUES
(2, 4, 'PRESENCIAL', '2026-09-15 08:30:00', '2026-10-02 15:00:00', 500.00, 'NO_ASISTIO'),
(2, 4, 'PRESENCIAL', '2026-10-03 14:00:00', '2026-10-09 15:00:00', 500.00, 'ASISTIO'),
(2, 4, 'PRESENCIAL', '2026-10-09 16:00:00', '2026-10-23 15:00:00', 500.00, 'NO_ASISTIO'),
(2, 4, 'PRESENCIAL', '2026-10-25 09:00:00', '2026-11-13 15:00:00', 500.00, 'PROGRAMADA'); -- Cita ID 8 (Prob asistencia 60% -> No-Show 40% -> Costo vacío L 200)

-- Paciente 3 (PAC-UNAH-003): Alto riesgo de inasistencia (2 no asistió, 1 cancelada tardía, 1 programada)
INSERT INTO citas (paciente_id, psicologo_id, modalidad, fecha_hora_solicitud, fecha_hora_cita, costo_sesion, estado_cita) VALUES
(3, 3, 'PRESENCIAL', '2026-09-20 11:00:00', '2026-10-05 11:00:00', 500.00, 'NO_ASISTIO'),
(3, 3, 'VIRTUAL',    '2026-10-06 09:00:00', '2026-10-19 11:00:00', 500.00, 'CANCELADA_TARDIA'),
(3, 3, 'PRESENCIAL', '2026-10-20 12:00:00', '2026-11-02 11:00:00', 500.00, 'NO_ASISTIO'),
(3, 3, 'PRESENCIAL', '2026-11-03 10:00:00', '2026-11-14 11:00:00', 500.00, 'PROGRAMADA'); -- Cita ID 12 (Riesgo Crítico)

-- Paciente 4 (PAC-UNAH-004): Paciente regular en modalidad virtual (2 asistencias, 1 cancelada a tiempo, 1 programada)
INSERT INTO citas (paciente_id, psicologo_id, modalidad, fecha_hora_solicitud, fecha_hora_cita, costo_sesion, estado_cita) VALUES
(4, 1, 'VIRTUAL',    '2026-10-01 13:00:00', '2026-10-07 14:00:00', 500.00, 'ASISTIO'),
(4, 1, 'VIRTUAL',    '2026-10-07 15:00:00', '2026-10-14 14:00:00', 500.00, 'CANCELADA_A_TIEMPO'), -- Excluida de penalización
(4, 1, 'VIRTUAL',    '2026-10-14 16:00:00', '2026-10-21 14:00:00', 500.00, 'ASISTIO'),
(4, 1, 'VIRTUAL',    '2026-11-08 09:00:00', '2026-11-11 14:00:00', 500.00, 'PROGRAMADA'); -- Cita ID 16 (Riesgo Bajo)

-- Paciente 5 (PAC-UNAH-005): Primera cita (Cold-start, 0 citas previas)
INSERT INTO citas (paciente_id, psicologo_id, modalidad, fecha_hora_solicitud, fecha_hora_cita, costo_sesion, estado_cita) VALUES
(5, 3, 'PRESENCIAL', '2026-11-01 08:00:00', '2026-11-15 10:00:00', 500.00, 'PROGRAMADA'); -- Cita ID 17 (1ª cita con 14 días de anticipación)

-- Pacientes 6, 7 y 8: Historial adicional cerrado para enriquecer el dataset de entrenamiento
INSERT INTO citas (paciente_id, psicologo_id, modalidad, fecha_hora_solicitud, fecha_hora_cita, costo_sesion, estado_cita) VALUES
(6, 2, 'PRESENCIAL', '2026-10-01 09:00:00', '2026-10-06 16:00:00', 500.00, 'ASISTIO'),
(6, 2, 'PRESENCIAL', '2026-10-06 17:00:00', '2026-10-13 16:00:00', 500.00, 'ASISTIO'),
(7, 4, 'TELEFONICA', '2026-10-02 10:00:00', '2026-10-10 10:00:00', 500.00, 'ASISTIO'),
(7, 4, 'PRESENCIAL', '2026-10-10 11:00:00', '2026-10-24 10:00:00', 500.00, 'NO_ASISTIO'),
(8, 1, 'PRESENCIAL', '2026-09-28 14:00:00', '2026-10-12 08:00:00', 500.00, 'NO_ASISTIO');

-- -----------------------------------------------------------------------------
-- 4. MODELOS DE MACHINE LEARNING (Sección 4.3 y 4.4: Los 4 Modelos y las 8 Métricas)
-- -----------------------------------------------------------------------------
INSERT INTO modelos_ml (
    codigo_version, algoritmo, enfoque,
    roc_auc, pr_auc, sensibilidad, especificidad, precision_vpp, f1_score, brier_score, ece_calibracion,
    curva_calibracion, hiperparametros, activo_en_produccion
) VALUES
(
    'M1_LOGREG_V1', 'LOGISTIC_REGRESSION', 'ESTADISTICO_SIMPLE',
    0.7640, 0.7810, 0.7500, 0.7100, 0.7650, 0.7574, 0.1680, 0.0420,
    '[{"bin":0.2,"n":45,"pred_media":0.21,"obs_real":0.20},{"bin":0.5,"n":80,"pred_media":0.51,"obs_real":0.49},{"bin":0.8,"n":100,"pred_media":0.80,"obs_real":0.78}]',
    '{"penalty":"l2","C":1.0,"solver":"lbfgs"}',
    0
),
(
    'M2_RF_V1', 'RANDOM_FOREST', 'MACHINE_LEARNING',
    0.8210, 0.8390, 0.8120, 0.7680, 0.8050, 0.8085, 0.1390, 0.0310,
    '[{"bin":0.2,"n":50,"pred_media":0.20,"obs_real":0.22},{"bin":0.5,"n":75,"pred_media":0.50,"obs_real":0.52},{"bin":0.8,"n":100,"pred_media":0.80,"obs_real":0.81}]',
    '{"n_estimators":500,"max_depth":8,"min_samples_leaf":10,"calibration":"isotonic"}',
    0
),
(
    'M3_XGB_V1', 'XGBOOST', 'MACHINE_LEARNING',
    0.8520, 0.8690, 0.8450, 0.8020, 0.8380, 0.8415, 0.1180, 0.0190,
    '[{"bin":0.2,"n":48,"pred_media":0.19,"obs_real":0.19},{"bin":0.6,"n":90,"pred_media":0.60,"obs_real":0.60},{"bin":0.8,"n":100,"pred_media":0.80,"obs_real":0.80}]',
    '{"n_estimators":300,"learning_rate":0.03,"max_depth":5,"subsample":0.8,"calibration":"platt_sigmoid"}',
    1 -- Modelo activo en producción (AUC = 0.852 como en la literatura citada)
),
(
    'M4_BAYES_V1', 'BAYESIAN', 'BAYESIANO',
    0.8150, 0.8310, 0.8010, 0.7740, 0.8100, 0.8055, 0.1310, 0.0220,
    '[{"bin":0.2,"n":46,"pred_media":0.20,"obs_real":0.20},{"bin":0.5,"n":82,"pred_media":0.50,"obs_real":0.51},{"bin":0.8,"n":100,"pred_media":0.80,"obs_real":0.80}]',
    '{"priors":"normal(0,1.5)","random_intercepts":["paciente_id","psicologo_id"],"chains":4,"draws":2000}',
    0
);

-- -----------------------------------------------------------------------------
-- 5. PREDICCIONES POR CITA E IMPACTO FINANCIERO (Sección 4.4 y 4.5)
-- -----------------------------------------------------------------------------
-- Cita ID 4 (Paciente A = 82% probabilidad de asistir -> Ejemplo exacto sección 4.4)
INSERT INTO predicciones_cita (cita_id, modelo_id, prob_asistencia, costo_sesion_base, features_snapshot) VALUES
(4, 3, 0.8200, 500.00, '{"num_sesiones_previas":3,"asistencia_historica":0.8571,"inasistencias":0,"anticipacion_cita_dias":6.92,"modalidad":"PRESENCIAL","es_primera_cita":0}');

-- Cita ID 8 (Probabilidad de no asistencia = 40% -> Prob asistencia = 60% -> Costo esperado vacío = L 200.00 -> Ejemplo exacto sección 4.5)
INSERT INTO predicciones_cita (cita_id, modelo_id, prob_asistencia, costo_sesion_base, features_snapshot) VALUES
(8, 3, 0.6000, 500.00, '{"num_sesiones_previas":1,"asistencia_historica":0.5714,"inasistencias":2,"anticipacion_cita_dias":19.25,"modalidad":"PRESENCIAL","es_primera_cita":0}');

-- Cita ID 12 (Paciente con múltiples inasistencias -> Prob asistencia = 35% -> Riesgo CRÍTICO -> Costo esperado vacío = L 325.00)
INSERT INTO predicciones_cita (cita_id, modelo_id, prob_asistencia, costo_sesion_base, features_snapshot) VALUES
(12, 3, 0.3500, 500.00, '{"num_sesiones_previas":0,"asistencia_historica":0.4286,"inasistencias":3,"anticipacion_cita_dias":11.04,"modalidad":"PRESENCIAL","es_primera_cita":0}');

-- Cita ID 16 (Paciente virtual muy cumplido -> Prob asistencia = 91% -> Riesgo BAJO -> Costo esperado vacío = L 45.00)
INSERT INTO predicciones_cita (cita_id, modelo_id, prob_asistencia, costo_sesion_base, features_snapshot) VALUES
(16, 3, 0.9100, 500.00, '{"num_sesiones_previas":2,"asistencia_historica":0.8333,"inasistencias":0,"anticipacion_cita_dias":3.21,"modalidad":"VIRTUAL","es_primera_cita":0}');

-- Cita ID 17 (Primera cita con 14 días de anticipación -> Prob asistencia = 72% -> Riesgo MODERADO -> Costo esperado vacío = L 140.00)
INSERT INTO predicciones_cita (cita_id, modelo_id, prob_asistencia, costo_sesion_base, features_snapshot) VALUES
(17, 3, 0.7200, 500.00, '{"num_sesiones_previas":0,"asistencia_historica":0.7500,"inasistencias":0,"anticipacion_cita_dias":14.08,"modalidad":"PRESENCIAL","es_primera_cita":1}');

-- -----------------------------------------------------------------------------
-- 6. INTERVENCIONES PREVENTIVAS (Recordatorios, Mensajes, Confirmaciones, Llamadas)
-- -----------------------------------------------------------------------------
INSERT INTO intervenciones_cita (cita_id, prediccion_id, tipo_accion, estado_envio, fecha_programada, fecha_ejecutada) VALUES
(4,  1, 'MENSAJE_PERSONALIZADO', 'CONFIRMADO_PACIENTE', '2026-11-10 09:00:00', '2026-11-10 09:01:12'),
(8,  2, 'CONFIRMACION',          'ENVIADO',             '2026-11-11 15:00:00', '2026-11-11 15:00:05'),
(12, 3, 'LLAMADA',               'PENDIENTE',           '2026-11-12 10:00:00', NULL),
(16, 4, 'RECORDATORIO',          'CONFIRMADO_PACIENTE', '2026-11-10 14:00:00', '2026-11-10 14:00:02'),
(17, 5, 'MENSAJE_PERSONALIZADO', 'PENDIENTE',           '2026-11-13 10:00:00', NULL);
