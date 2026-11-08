-- =============================================================================
-- SCRIPT DE MIGRACIÓN: SUBIDA DE PARROQUIA LOCAL A LA NUBE (CERO CHOQUES)
-- SISTEMA ECLESOFT 2.0 (Google Cloud / PostgreSQL)
-- =============================================================================
-- PROPÓSITO:
-- Permite tomar los datos de una parroquia que operaba de forma local/autónoma
-- e integrarlos a la base de datos central en la nube SIN COLISIÓN DE IDs,
-- sin alterar las actas de otras parroquias y preservando toda la integridad.
-- =============================================================================

-- PASO 0: Crear tablas temporales de mapeo de IDs para resolver claves foráneas
CREATE TEMP TABLE IF NOT EXISTS temp_map_cursillos_bautismo (
    id_local INT PRIMARY KEY,
    id_nube INT
);

CREATE TEMP TABLE IF NOT EXISTS temp_map_cursillos_matrimonio (
    id_local INT PRIMARY KEY,
    id_nube INT
);

-- =============================================================================
-- INSTRUCCIONES DE USO:
-- 1. En la base local, exportar solo las filas correspondientes con COPY o INSERT:
--    pg_dump -h localhost -U postgres -d curia --data-only --column-inserts ...
-- 2. En la nube, ejecutar dentro de una transacción BEGIN / COMMIT:
-- =============================================================================

DO $$
DECLARE
    -- Asignar el ID definitivo que tendrá la parroquia en la nube:
    v_parroquia_nube_id INT := 1; -- <-- Reemplazar por el ID destino en la nube
BEGIN
    RAISE NOTICE 'Iniciando migración segura para la parroquia destino ID: %', v_parroquia_nube_id;
END $$;

-- EJEMPLO DE INSERCIÓN LIBRE DE COLISIONES:
-- Al omitir la columna 'id' en el INSERT, PostgreSQL en la nube genera
-- automáticamente el siguiente ID correlativo libre mediante la secuencia 'SERIAL'.

/*
-- 1. Migración de Cursillos Prebautismales (con captura de nuevo ID generado)
WITH cursillos_insertados AS (
    INSERT INTO cursillosprebautismales (
        nombrecompletonino, nombrepadre, nombremadre, nombrepadrino, nombremadrina,
        parroquia, quieninscribe, telefono, celular, fechabautizo, fechacursillo,
        estado, observaciones
    )
    SELECT
        c_loc.nombrecompletonino, c_loc.nombrepadre, c_loc.nombremadre, c_loc.nombrepadrino, c_loc.nombremadrina,
        v_parroquia_nube_id, c_loc.quieninscribe, c_loc.telefono, c_loc.celular, c_loc.fechabautizo, c_loc.fechacursillo,
        c_loc.estado, c_loc.observaciones
    FROM temp_cursillos_locales c_loc
    RETURNING id
)
INSERT INTO temp_map_cursillos_bautismo (id_local, id_nube)
SELECT c_loc.id_original, c_ins.id
FROM temp_cursillos_locales c_loc
JOIN cursillos_insertados c_ins ON ...;

-- 2. Migración de Actas de Bautismo vinculadas al nuevo cursillo
INSERT INTO actasbautismo (
    nombrecompleto, lugarnacimiento, cedula, sexo, fechanacimiento, fechabautizo,
    cedulapadre, cedulamadre, cedulapadrino1, cedulapadrino2, cedulapadrino3,
    cedulapadrino4, cedulapadrino5, pagina, tomo, acta, registrocivilano,
    registrociviltomo, registrocivilfolio, registrocivilacta, registrocivilfecha,
    observaciones, ministro_id, sacerdote_id, iglesia_id, parroquia_id,
    cursillo_id, estado
)
SELECT
    a.nombrecompleto, a.lugarnacimiento, a.cedula, a.sexo, a.fechanacimiento, a.fechabautizo,
    a.cedulapadre, a.cedulamadre, a.cedulapadrino1, a.cedulapadrino2, a.cedulapadrino3,
    a.cedulapadrino4, a.cedulapadrino5, a.pagina, a.tomo, a.acta, a.registrocivilano,
    a.registrociviltomo, a.registrocivilfolio, a.registrocivilacta, a.registrocivilfecha,
    a.observaciones, a.ministro_id, a.sacerdote_id, a.iglesia_id, v_parroquia_nube_id,
    m.id_nube, a.estado
FROM temp_actas_locales a
LEFT JOIN temp_map_cursillos_bautismo m ON a.cursillo_id_original = m.id_local;
*/
