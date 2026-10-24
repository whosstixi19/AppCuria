-- =============================================================================
-- ARCHIVO: DatosSemilla.sql
-- DATOS DE PRUEBA COMPLETOS PARA LA BASE DE DATOS: curia (ECLESOFT 2.0)
-- Sincronizado con BD.sql y todos los Controladores / Modelos de AplicacionCuria
-- Omite imágenes (firma = NULL, foto = NULL)
-- =============================================================================

BEGIN;

-- 1. USUARIOS ADICIONALES (admin@curia.com ya existe con ID 1; agregamos personal de apoyo)
-- Contraseña para todos: Clave_curia
INSERT INTO usuarios (correo, nombre, apellido, rol, estado, pass, salt, iterations)
VALUES
    ('secretaria@curia.com', 'María', 'Espinoza', 'personal', 'ACTIVO', '65kDeHXTZytW/nl6iNieLWVt/Pub4KjYg9WBYuOjyUE=', 'AQIDBAUGBwgJCgsMDQ4PEA==', 310000),
    ('despacho@curia.com', 'Carlos', 'Mendoza', 'personal', 'ACTIVO', '65kDeHXTZytW/nl6iNieLWVt/Pub4KjYg9WBYuOjyUE=', 'AQIDBAUGBwgJCgsMDQ4PEA==', 310000)
ON CONFLICT (correo) DO NOTHING;

-- 2. SACERDOTES (firma = NULL para omitir imágenes)
INSERT INTO sacerdotes (nombre, apellido, cedula, telefono, email, estado, firma, observaciones)
VALUES
    ('Juan Carlos', 'Vintimilla', '0102345678', '0991234567', 'jvintimilla@curia.com', 'ACTIVO', NULL, 'Párroco principal de El Sagrario'),
    ('Roberto', 'Cordero', '0103456789', '0992345678', 'rcordero@curia.com', 'ACTIVO', NULL, 'Vicario parroquial de San Sebastián'),
    ('Patricio', 'Astudillo', '0104567890', '0993456789', 'pastudillo@curia.com', 'ACTIVO', NULL, 'Encargado de pastoral familiar'),
    ('Fernando', 'Crespo', '0105678901', '0994567890', 'fcrespo@curia.com', 'ACTIVO', NULL, 'Capellán y director espiritual')
ON CONFLICT (cedula) DO NOTHING;

-- 3. MINISTROS
INSERT INTO ministros (nombre, apellido, email, telefono, fechaordenacion, estado, observaciones)
VALUES
    ('Mons. Marcos', 'Pérez', 'mperez@curia.com', '072831111', '2005-06-15 10:00:00', 'ACTIVO', 'Arzobispo Metropolitano'),
    ('P. Esteban', 'Malo', 'emalo@curia.com', '072842222', '2012-09-20 10:00:00', 'ACTIVO', 'Ministro delegado de Bautismos y Matrimonios'),
    ('P. Diego', 'Carrasco', 'dcarrasco@curia.com', '072853333', '2016-11-10 10:00:00', 'ACTIVO', 'Ministro de Confirmaciones y Catequesis'),
    ('Diác. Andrés', 'Palacios', 'apalacios@curia.com', '072864444', '2020-04-25 10:00:00', 'ACTIVO', 'Diácono permanente');

-- 4. PARROQUIAS
INSERT INTO parroquias (nombre, direccion, telefono, email, sacerdote_id, estado, observaciones)
VALUES
    ('Parroquia El Sagrario', 'Benigno Malo y Sucre, Centro Histórico', '072823456', 'sagrario@curia.com', 1, 'ACTIVO', 'Parroquia matriz de la ciudad de Cuenca'),
    ('Parroquia San Sebastián', 'Coronel Talbot y Bolívar', '072834567', 'sansebastian@curia.com', 2, 'ACTIVO', 'Sector tradicional de San Sebastián'),
    ('Parroquia San Blas', 'Manuel Vega y Simón Bolívar', '072845678', 'sanblas@curia.com', 3, 'ACTIVO', 'Atención pastoral diaria'),
    ('Parroquia Virgen del Bronce', 'Av. 10 de Agosto y Ricardo Muñoz', '072856789', 'virgendelbronce@curia.com', 4, 'ACTIVO', 'Amplia participación en catequesis');

-- 5. IGLESIAS
INSERT INTO iglesias (nombre, direccion, ciudad, telefono, email, ministro_id, parroquia_id, denominacion, estado, observaciones)
VALUES
    ('Catedral de la Inmaculada Concepción', 'Padre Aguirre y Sucre', 'Cuenca', '072821122', 'catedral@curia.com', 1, 1, 'Catedral Metropolitana', 'ACTIVO', 'Templo mayor de la Arquidiócesis'),
    ('Iglesia de San Sebastián', 'Parque San Sebastián', 'Cuenca', '072832233', 'iglesiasansebastian@curia.com', 2, 2, 'Templo Parroquial', 'ACTIVO', 'Celebraciones dominicales y bautizos'),
    ('Iglesia de San Blas', 'Parque San Blas', 'Cuenca', '072843344', 'iglesiasanblas@curia.com', 3, 3, 'Templo Parroquial', 'ACTIVO', 'Celebraciones de matrimonios y comuniones'),
    ('Santuario Virgen del Bronce', 'Av. 10 de Agosto', 'Cuenca', '072854455', 'santuariobronce@curia.com', 4, 4, 'Santuario Mariano', 'ACTIVO', 'Misas comunitarias y catequesis');

-- 6. MISAS
INSERT INTO misas (nombre, hora_inicio, hora_fin, dia_semana, iglesia_id, sacerdote_id, descripcion, estado)
VALUES
    ('Misa Dominical Matutina', '08:00:00', '09:00:00', 'Domingo', 1, 1, 'Misa solemne dominical con coro', 'ACTIVO'),
    ('Misa Familiar de Mediodía', '11:00:00', '12:00:00', 'Domingo', 2, 2, 'Misa dedicada a las familias y niños de catequesis', 'ACTIVO'),
    ('Misa Vespertina de Sábado', '18:00:00', '19:00:00', 'Sábado', 3, 3, 'Misa de precepto vespertina', 'ACTIVO'),
    ('Misa de Jueves Eucarístico', '19:00:00', '20:00:00', 'Jueves', 4, 4, 'Eucaristía seguida de Hora Santa', 'ACTIVO');

-- 7. BARRIOS
INSERT INTO barrios (nombre, ministro_id, iglesia_id, misa_id, estado, observaciones)
VALUES
    ('Barrio El Vado', 1, 1, 1, 'ACTIVO', 'Comunidad activa en fiestas patronales'),
    ('Barrio San Sebastián', 2, 2, 2, 'ACTIVO', 'Grupo de oración los miércoles'),
    ('Barrio Todos Santos', 3, 3, 3, 'ACTIVO', 'Coordinación de pastoral juvenil'),
    ('Barrio El Vergel', 4, 4, 4, 'ACTIVO', 'Visitas a enfermos los viernes');

-- 8. FELIGRESES (Cédulas usadas en cursillos y actas de bautismo/matrimonio)
INSERT INTO feligreses (nombre, apellido, cedula, fechanacimiento, direccion, telefono, email, estado, observaciones)
VALUES
    ('Luis Alberto', 'Andrade', '0101112221', '1988-03-14 00:00:00', 'Av. Solano 4-55', '0981112221', 'landrade@correo.com', 'ACTIVO', 'Padre de familia / Feligrés activo'),
    ('Gabriela Fernanda', 'Mora', '0101112222', '1990-07-22 00:00:00', 'Av. Solano 4-55', '0981112222', 'gmora@correo.com', 'ACTIVO', 'Madre de familia / Feligresa activa'),
    ('Santiago Xavier', 'Peralta', '0101112223', '1985-11-05 00:00:00', 'Calle Larga 8-12', '0981112223', 'speralta@correo.com', 'ACTIVO', 'Padrino de bautismo y testigo'),
    ('Verónica Lucía', 'Salazar', '0101112224', '1987-01-30 00:00:00', 'Calle Larga 8-12', '0981112224', 'vsalazar@correo.com', 'ACTIVO', 'Madrina de bautismo y testigo'),
    ('Daniel Alejandro', 'Ochoa', '0101112225', '1994-05-18 00:00:00', 'Av. Remigio Crespo 2-10', '0981112225', 'dochoa@correo.com', 'ACTIVO', 'Novio inscrito en cursillo prematrimonial'),
    ('Valeria Estefanía', 'Durán', '0101112226', '1996-09-12 00:00:00', 'Av. Las Américas 11-20', '0981112226', 'vduran@correo.com', 'ACTIVO', 'Novia inscrita en cursillo prematrimonial'),
    ('Ricardo Patricio', 'Vega', '0101112227', '1982-12-03 00:00:00', 'Gran Colombia 5-40', '0981112227', 'rvega@correo.com', 'ACTIVO', 'Padre de familia'),
    ('Carmen Elena', 'Rojas', '0101112228', '1984-04-19 00:00:00', 'Gran Colombia 5-40', '0981112228', 'crojas@correo.com', 'ACTIVO', 'Madre de familia'),
    ('Jorge Eduardo', 'Toral', '0101112229', '1992-08-25 00:00:00', 'Av. Loja 3-88', '0981112229', 'jtoral@correo.com', 'ACTIVO', 'Novio inscrito en cursillo prematrimonial'),
    ('Andrea Carolina', 'Calle', '0101112230', '1995-02-14 00:00:00', 'Av. Primero de Mayo', '0981112230', 'acalle@correo.com', 'ACTIVO', 'Novia inscrita en cursillo prematrimonial')
ON CONFLICT (cedula) DO NOTHING;

-- 9. CURSILLOS PREBAUTISMALES
-- Nota: nombrepadre, nombremadre, nombrepadrino, nombremadrina guardan la cédula del feligrés, y estado 'Aprobado' permite seleccionarlos en Actas
INSERT INTO cursillosprebautismales (nombrecompletonino, nombrepadre, nombremadre, nombrepadrino, nombremadrina, parroquia, quieninscribe, telefono, celular, fechabautizo, fechacursillo, estado, observaciones)
VALUES
    ('Mateo Nicolás Andrade Mora', '0101112221', '0101112222', '0101112223', '0101112224', 1, 'Luis Alberto Andrade', '072811111', '0981112221', '2026-06-20 10:00:00', '2026-06-10 15:00:00', 'Aprobado', 'Asistencia completa de padres y padrinos'),
    ('Sofía Valentina Vega Rojas', '0101112227', '0101112228', '0101112221', '0101112222', 2, 'Ricardo Patricio Vega', '072822222', '0981112227', '2026-07-15 11:00:00', '2026-07-05 15:00:00', 'Aprobado', 'Documentación entregada en secretaría'),
    ('Lucas Emiliano Peralta Salazar', '0101112223', '0101112224', '0101112227', '0101112228', 3, 'Santiago Xavier Peralta', '072833333', '0981112223', '2026-08-22 10:00:00', '2026-08-12 16:00:00', 'Aprobado', 'Cursillo aprobado satisfactoriamente');

-- 10. ACTAS DE BAUTISMO
INSERT INTO actasbautismo (nombrecompleto, lugarnacimiento, cedula, sexo, fechanacimiento, fechabautizo, cedulapadre, cedulamadre, cedulapadrino1, cedulapadrino2, cedulapadrino3, cedulapadrino4, cedulapadrino5, pagina, tomo, acta, registrocivilano, registrociviltomo, registrocivilfolio, registrocivilacta, registrocivilfecha, observaciones, ministro_id, sacerdote_id, iglesia_id, parroquia_id, cursillo_id, estado)
VALUES
    ('Mateo Nicolás Andrade Mora', 'Cuenca', '0150112231', 'Masculino', '2025-11-10 00:00:00', '2026-06-20 10:00:00', '0101112221', '0101112222', '0101112223', '0101112224', NULL, NULL, NULL, '14', '2', '104', '2025', '1', '45', '890', '2025-11-18 00:00:00', 'Bautizo celebrado sin novedades', 1, 1, 1, 1, 1, 'ACTIVO'),
    ('Sofía Valentina Vega Rojas', 'Cuenca', '0150112232', 'Femenino', '2026-01-05 00:00:00', '2026-07-15 11:00:00', '0101112227', '0101112228', '0101112221', '0101112222', NULL, NULL, NULL, '28', '2', '118', '2026', '1', '12', '215', '2026-01-12 00:00:00', 'Certificado emitido a los padres', 2, 2, 2, 2, 2, 'ACTIVO'),
    ('Lucas Emiliano Peralta Salazar', 'Cuenca', '0150112233', 'Masculino', '2026-02-20 00:00:00', '2026-08-22 10:00:00', '0101112223', '0101112224', '0101112227', '0101112228', NULL, NULL, NULL, '35', '3', '129', '2026', '2', '67', '340', '2026-03-01 00:00:00', 'Acta verificada en archivo parroquial', 3, 3, 3, 3, 3, 'ACTIVO');

-- 11. BAUTISMOS ANTIGUOS
INSERT INTO bautismosantiguos (nombrecompleto, lugarnacimiento, cedula, sexo, fechanacimiento, fechabautizo, nombrepadre, nombremadre, cedulapadrino1, cedulapadrino2, cedulapadrino3, cedulapadrino4, cedulapadrino5, pagina, tomo, acta, registrocivilano, registrociviltomo, registrocivilfolio, registrocivilacta, registrocivilfecha, observaciones, ministro_id, iglesia_id, parroquia_id, sacerdote_id, estado)
VALUES
    ('Pedro José Sarmiento', 'Cuenca', '0100889901', 'Masculino', '1975-04-12 00:00:00', '1975-06-01 10:00:00', 'Manuel Sarmiento', 'Rosa Piedra', '0101112221', '0101112222', NULL, NULL, NULL, '112', '1', '450', '1975', '1', '89', '321', '1975-04-20 00:00:00', 'Transcripción de libro histórico parroquial', 1, 1, 1, 1, 'ACTIVO'),
    ('Rosa María Albornoz', 'Azogues', '0100889902', 'Femenino', '1980-09-25 00:00:00', '1980-11-15 11:00:00', 'Antonio Albornoz', 'Teresa Vázquez', '0101112223', '0101112224', NULL, NULL, NULL, '210', '2', '715', '1980', '3', '54', '612', '1980-10-02 00:00:00', 'Digitalizado del archivo histórico de la Curia', 2, 2, 2, 2, 'ACTIVO');

-- 12. CURSILLOS PREMATRIMONIALES
-- Nota: estado 'Aprobado' para poder vincularlos en Actas Prematrimoniales
INSERT INTO cursillosprematrimoniales (nombrenovio, cedulanovio, telefononovio, fechanacimientonovio, nombrenovia, cedulanovia, telefononovia, fechanacimientonovia, fechacursillo, fechamatrimonio, estado, parroquia, observaciones)
VALUES
    ('Daniel Alejandro Ochoa', '0101112225', '0981112225', '1994-05-18 00:00:00', 'Valeria Estefanía Durán', '0101112226', '0981112226', '1996-09-12 00:00:00', '2026-05-10 09:00:00', '2026-09-19 16:00:00', 'Aprobado', 1, 'Pareja asistió a las 3 jornadas completas'),
    ('Jorge Eduardo Toral', '0101112229', '0981112229', '1992-08-25 00:00:00', 'Andrea Carolina Calle', '0101112230', '0981112230', '1995-02-14 00:00:00', '2026-06-14 09:00:00', '2026-10-24 17:00:00', 'Aprobado', 2, 'Certificado de cursillo prematrimonial entregado');

-- 13. ACTAS PREMATRIMONIALES
INSERT INTO actasprematrimoniales (nombrenovio, lugarbautizonovio, nombrepadrenovio, nombremadrenovio, fechanacimientonovio, fechabautizonovio, nombrenovia, lugarbautizonovia, nombrepadrenovia, nombremadrenovia, fechanacimientonovia, fechabautizonovia, fechamatrimonio, iglesia_id, ministro_id, fecharegistrocivil, lugarregistrocivil, numeroactaregistrocivil, numeropaginaregistrocivil, numertomoregistrocivil, tomo, pagina, acta, fecharegistro, observaciones, testigo1, testigo2, testigo3, testigo4, cursillo_id, estado)
VALUES
    ('Daniel Alejandro Ochoa', 'Parroquia El Sagrario', 'Alberto Ochoa', 'Laura Palacios', '1994-05-18 00:00:00', '1994-08-20 00:00:00', 'Valeria Estefanía Durán', 'Parroquia San Sebastián', 'Fernando Durán', 'Silvia Bernal', '1996-09-12 00:00:00', '1996-12-15 00:00:00', '2026-09-19 16:00:00', 1, 1, '2026-09-10 00:00:00', 'Registro Civil de Cuenca', 'RC-2026-441', '18', '4', '1', '45', '88', '2026-08-25 10:00:00', 'Expediente matrimonial completo y proclamas publicadas', 'Santiago Xavier Peralta', 'Verónica Lucía Salazar', 'Luis Alberto Andrade', 'Gabriela Fernanda Mora', 1, 'ACTIVO'),
    ('Jorge Eduardo Toral', 'Parroquia San Blas', 'Eduardo Toral', 'Beatriz Montes', '1992-08-25 00:00:00', '1992-11-30 00:00:00', 'Andrea Carolina Calle', 'Parroquia Virgen del Bronce', 'Miguel Calle', 'Patricia Ordóñez', '1995-02-14 00:00:00', '1995-05-20 00:00:00', '2026-10-24 17:00:00', 2, 2, '2026-10-15 00:00:00', 'Registro Civil de Cuenca', 'RC-2026-512', '29', '5', '1', '52', '94', '2026-09-30 11:00:00', 'Sin impedimentos canónicos', 'Ricardo Patricio Vega', 'Carmen Elena Rojas', NULL, NULL, 2, 'ACTIVO');

-- 14. ACTAS DE MATRIMONIO
INSERT INTO actasdematrimonio (padrino1, padrino2, padrino3, padrino4, fecha_matrimonio, fecharegistro, acta_prematrimonial_id, iglesia_id, ministro_id, acta, tomo, pagina, observaciones)
VALUES
    ('Santiago Xavier Peralta', 'Verónica Lucía Salazar', 'Luis Alberto Andrade', 'Gabriela Fernanda Mora', '2026-09-19 16:00:00', '2026-09-20 09:00:00', 1, 1, 1, '88', '1', '45', 'Matrimonio solemne celebrado en la Catedral'),
    ('Ricardo Patricio Vega', 'Carmen Elena Rojas', NULL, NULL, '2026-10-24 17:00:00', '2026-10-25 09:00:00', 2, 2, 2, '94', '1', '52', 'Matrimonio celebrado en la Iglesia de San Sebastián');

-- 15. MATRIMONIOS ANTIGUOS
INSERT INTO matrimoniosantiguos (nombrenovio, lugarbautizonovio, cedulapadrenovio, cedulamadrenovio, fechanacimientonovio, fechabautizonovio, nombrenovia, lugarbautizonovia, cedulapadrenovia, cedulamadrenovia, fechanacimientonovia, fechabautizonovia, fechamatrimonio, fecharegistrocivil, lugarregistrocivil, numeroactaregistrocivil, tomo, pagina, acta, fecharegistro, cedulatestigo1, cedulatestigo2, cedulatestigo3, cedulatestigo4, ministro_id, iglesia_id, estado, observaciones)
VALUES
    ('Gustavo Adolfo Borrero', 'Parroquia El Sagrario', '0101112221', '0101112222', '1965-03-10 00:00:00', '1965-05-15 00:00:00', 'Elena Margarita Cueva', 'Parroquia San Blas', '0101112227', '0101112228', '1968-07-22 00:00:00', '1968-09-10 00:00:00', '1990-06-16 11:00:00', '1990-06-10 00:00:00', 'Cuenca', '1990-231', '3', '120', '340', '1990-06-17 00:00:00', '0101112223', '0101112224', NULL, NULL, 1, 1, 'ACTIVO', 'Registro histórico de matrimonio de 1990');

-- 16. CATEQUISTAS
INSERT INTO catequistas (nombre, apellido, cedula, telefono, direccion, email, fecha_nacimiento, estado, observaciones)
VALUES
    ('Ana Belén', 'Moscoso', '0106112233', '0995112233', 'Av. Don Bosco y Loja', 'amoscoso@curia.com', '1995-04-12 00:00:00', 'ACTIVO', 'Coordinadora de nivel Comunión'),
    ('David Sebastián', 'Neira', '0106112234', '0995112234', 'Calle Bolívar 12-40', 'dneira@curia.com', '1993-08-19 00:00:00', 'ACTIVO', 'Coordinador de nivel Confirmación'),
    ('Paola Cristina', 'Hidalgo', '0106112235', '0995112235', 'Av. Ordóñez Lasso', 'phidalgo@curia.com', '1997-11-28 00:00:00', 'ACTIVO', 'Catequista de Iniciación y Reconciliación')
ON CONFLICT (cedula) DO NOTHING;

-- 17. PERIODOS
INSERT INTO periodos (nombre, fecha_inicio, fecha_fin, estado, observaciones)
VALUES
    ('Periodo Catequético 2025 - 2026', '2025-09-15 00:00:00', '2026-06-30 00:00:00', 'ACTIVO', 'Año lectivo de catequesis regular'),
    ('Periodo Catequético 2026 - 2027', '2026-09-14 00:00:00', '2027-06-30 00:00:00', 'ACTIVO', 'Nuevo periodo de inscripciones abiertas');

-- 18. ALUMNOS DE CATEQUESIS (foto = NULL para omitir imágenes)
INSERT INTO alumnoscatequesis (nombre, apellido, fechanacimiento, fechabautizo, cedula, direccion, telefono, foto, nacionalidad, estadocivil, correo, nombrerepresentante, telefonorepresentante, relacionconelestudiante, nivel, catequista_id, catequista_id2, paralelo, periodo_id, estado, observaciones)
VALUES
    ('Camila Alejandra', 'Andrade', '2016-05-14 00:00:00', '2016-09-20 00:00:00', '0150223341', 'Av. Solano 4-55', '0981112221', NULL, 'Ecuatoriana', 'Soltero/a', 'camila.andrade@correo.com', 'Luis Alberto Andrade', '0981112221', 'Padre', 'COMUNIÓN', 1, 3, 'A', 1, 'ACTIVO', 'Alumna puntual y participativa'),
    ('Sebastián Andrés', 'Vega', '2011-08-22 00:00:00', '2012-01-15 00:00:00', '0150223342', 'Gran Colombia 5-40', '0981112227', NULL, 'Ecuatoriana', 'Soltero/a', 'sebastian.vega@correo.com', 'Ricardo Patricio Vega', '0981112227', 'Padre', 'CONFIRMACIÓN', 2, 1, 'B', 1, 'ACTIVO', 'Listo para recibir el sacramento de la Confirmación'),
    ('Martina Isabel', 'Peralta', '2017-02-10 00:00:00', '2017-06-12 00:00:00', '0150223343', 'Calle Larga 8-12', '0981112223', NULL, 'Ecuatoriana', 'Soltero/a', 'martina.peralta@correo.com', 'Santiago Xavier Peralta', '0981112223', 'Padre', 'INICIACIÓN', 3, NULL, 'A', 2, 'ACTIVO', 'Primer nivel de catequesis infantil'),
    ('Diego Alejandro', 'Mora', '2015-11-03 00:00:00', '2016-03-19 00:00:00', '0150223344', 'Av. Las Américas', '0981112222', NULL, 'Ecuatoriana', 'Soltero/a', 'diego.mora@correo.com', 'Gabriela Fernanda Mora', '0981112222', 'Madre', 'COMUNIÓN', 1, NULL, 'A', 1, 'ACTIVO', 'Excelente rendimiento en catequesis');

-- 19. FALTAS DE ALUMNOS
INSERT INTO faltas (alumno_id, fechafalta, observacion, estado)
VALUES
    (1, '2026-03-14 00:00:00', 'Falta justificada por cita médica', 'ACTIVO'),
    (2, '2026-04-11 00:00:00', 'Viaje familiar avisado con anticipación', 'ACTIVO'),
    (3, '2026-10-10 00:00:00', 'Reposo médico por gripe', 'ACTIVO');

-- 20. PRIMERAS COMUNIONES (Vinculadas a alumnos de catequesis por alumno_id y cédula)
INSERT INTO primerascomuniones (alumno_id, nombre, apellido, fechanacimiento, cedula, direccion, telefono, fechaprimeracomunion, parroquia_id, sacerdote_id, estado, observaciones)
VALUES
    (1, 'Camila Alejandra', 'Andrade', '2016-05-14 00:00:00', '0150223341', 'Av. Solano 4-55', '0981112221', '2026-06-27 10:00:00', 1, 1, 'ACTIVO', 'Primera Comunión celebrada en la Catedral'),
    (4, 'Diego Alejandro', 'Mora', '2015-11-03 00:00:00', '0150223344', 'Av. Las Américas', '0981112222', '2026-06-27 10:00:00', 1, 1, 'ACTIVO', 'Primera Comunión grupal periodo 2025-2026');

-- 21. CONFIRMACIONES (Vinculadas a alumnos de catequesis por alumno_id y cédula)
INSERT INTO confirmaciones (alumno_id, nombre, apellido, fechanacimiento, cedula, direccion, telefono, fechaconfirmacion, parroquia_id, sacerdote_id, estado, observaciones)
VALUES
    (2, 'Sebastián Andrés', 'Vega', '2011-08-22 00:00:00', '0150223342', 'Gran Colombia 5-40', '0981112227', '2026-07-04 11:00:00', 2, 2, 'ACTIVO', 'Confirmación presidida por delegación episcopal');

-- 22. PERSONAS DE EVENTO (Encargados de eventos para la Agenda)
INSERT INTO personas_evento (nombre, email, telefono, rol)
VALUES
    ('María Espinoza', 'secretaria@curia.com', '0991113344', 'Coordinadora Litúrgica'),
    ('Carlos Mendoza', 'despacho@curia.com', '0992224455', 'Logística y Sacristía'),
    ('Ana Belén Moscoso', 'amoscoso@curia.com', '0995112233', 'Coordinadora de Catequesis');

-- 23. AGENDAS (responsables_ids almacena IDs separados por coma de personas_evento)
INSERT INTO agendas (fechaagenda, hora, descripcion, lugar, responsables_ids, estado, tipo_evento, titulo, notificar, recordatorio_anticipacion)
VALUES
    ('2026-11-02 00:00:00', '09:00:00', 'Misa solemne de Conmemoración de los Fieles Difuntos', 'Catedral de la Inmaculada Concepción', '1,2', 'ACTIVO', 'Misa Especial', 'Misa de Fieles Difuntos', TRUE, 2),
    ('2026-11-15 00:00:00', '10:00:00', 'Ceremonia comunitaria de Bautizos del mes de noviembre', 'Iglesia de San Sebastián', '1', 'ACTIVO', 'Bautizo', 'Bautizos Comunitarios Noviembre', TRUE, 3),
    ('2026-11-21 00:00:00', '16:00:00', 'Reunión general de padres de familia de Catequesis', 'Salón Parroquial El Sagrario', '3', 'ACTIVO', 'Otro', 'Reunión de Padres de Catequesis', TRUE, 1);

-- 24. PAGOS
INSERT INTO pagos (tipo_pago, concepto, nombres, apellidos, direccion, cedula, telefono, valor, tipopago, fechapago, referencia_id, referencia_tabla, estado, observaciones, usuario_registro_id)
VALUES
    ('bautismo-cursillo', 'Inscripción a cursillo prebautismal', 'Luis Alberto', 'Andrade', 'Av. Solano 4-55', '0101112221', '0981112221', 15.00, 'Efectivo', '2026-06-05 10:00:00', 1, 'cursillosprebautismales', 'PAGADO', 'Pago cancelado en secretaría', 1),
    ('bautismo-celebracion', 'Ofrenda por celebración de Bautismo', 'Ricardo Patricio', 'Vega', 'Gran Colombia 5-40', '0101112227', '0981112227', 25.00, 'Transferencia', '2026-07-10 11:30:00', 2, 'actasbautismo', 'PAGADO', 'Comprobante bancario verificado', 1),
    ('matrimonio-cursillo', 'Inscripción a cursillo prematrimonial', 'Daniel Alejandro', 'Ochoa', 'Av. Remigio Crespo 2-10', '0101112225', '0981112225', 30.00, 'Efectivo', '2026-05-02 09:15:00', 1, 'cursillosprematrimoniales', 'PAGADO', 'Incluye material para los novios', 1),
    ('matrimonio-celebracion', 'Derechos y arreglo floral de Matrimonio', 'Daniel Alejandro', 'Ochoa', 'Av. Remigio Crespo 2-10', '0101112225', '0981112225', 120.00, 'Tarjeta', '2026-09-01 15:00:00', 1, 'actasprematrimoniales', 'PAGADO', 'Reserva de templo y coro confirmada', 1);

COMMIT;
