-- =============================================================================
-- ARCHIVO: BD.sql
-- SCRIPT UNIFICADO DEFINITIVO - BASE DE DATOS: curia (ECLESOFT 2.0)
-- Sincronizado 100% con AplicacionCuria.Conexion.ConexionPostgreSQL y Models/
-- =============================================================================

-- 1. Tabla: usuarios (Models/Usuario.cs)
CREATE TABLE usuarios (
    id SERIAL PRIMARY KEY,
    correo VARCHAR(255) UNIQUE NOT NULL,
    nombre VARCHAR(255),
    apellido VARCHAR(255),
    rol VARCHAR(50) DEFAULT 'admin',
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    ultimo_acceso TIMESTAMPTZ,
    fecha_creacion TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    pass VARCHAR(255) NOT NULL,
    salt VARCHAR(255) NOT NULL,
    iterations INT DEFAULT 310000
);

-- Usuario Administrador listo para iniciar sesión:
-- Correo:     admin@curia.com
-- Contraseña: Clave_curia
INSERT INTO usuarios (correo, nombre, apellido, rol, estado, pass, salt, iterations)
VALUES (
    'admin@curia.com',
    'Administrador',
    'Curia',
    'admin',
    'ACTIVO',
    '65kDeHXTZytW/nl6iNieLWVt/Pub4KjYg9WBYuOjyUE=',
    'AQIDBAUGBwgJCgsMDQ4PEA==',
    310000
);

-- 2. Tabla: sacerdotes (Models/Sacerdote.cs)
CREATE TABLE sacerdotes (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    apellido VARCHAR(255) NOT NULL,
    cedula VARCHAR(20) UNIQUE,
    telefono VARCHAR(20),
    email VARCHAR(255),
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    firma BYTEA,
    observaciones TEXT
);

-- 3. Tabla: ministros (Models/Ministro.cs)
CREATE TABLE ministros (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    apellido VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    telefono VARCHAR(20),
    fechaordenacion TIMESTAMP,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- 4. Tabla: parroquias (Models/Parroquia.cs)
CREATE TABLE parroquias (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    direccion TEXT,
    telefono VARCHAR(20),
    email VARCHAR(255),
    sacerdote_id INT REFERENCES sacerdotes(id) ON DELETE SET NULL,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- 5. Tabla: iglesias (Models/Iglesia.cs)
CREATE TABLE iglesias (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    direccion VARCHAR(255),
    ciudad VARCHAR(255),
    telefono VARCHAR(20),
    email VARCHAR(255),
    ministro_id INT REFERENCES ministros(id) ON DELETE SET NULL,
    parroquia_id INT REFERENCES parroquias(id) ON DELETE SET NULL,
    denominacion VARCHAR(255),
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- 6. Tabla: misas (Models/Misa.cs)
CREATE TABLE misas (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    hora_inicio INTERVAL NOT NULL,
    hora_fin INTERVAL,
    dia_semana VARCHAR(20),
    iglesia_id INT REFERENCES iglesias(id) ON DELETE SET NULL,
    sacerdote_id INT REFERENCES sacerdotes(id) ON DELETE SET NULL,
    descripcion TEXT,
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- 7. Tabla: barrios (Models/Barrio.cs)
CREATE TABLE barrios (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    ministro_id INT REFERENCES ministros(id) ON DELETE SET NULL,
    iglesia_id INT REFERENCES iglesias(id) ON DELETE SET NULL,
    misa_id INT REFERENCES misas(id) ON DELETE SET NULL,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- 8. Tabla: feligreses (Models/Feligres.cs)
CREATE TABLE feligreses (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    apellido VARCHAR(255) NOT NULL,
    cedula VARCHAR(20) UNIQUE,
    fechanacimiento TIMESTAMP,
    direccion VARCHAR(255),
    telefono VARCHAR(20),
    email VARCHAR(255),
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- 9. Tabla: cursillosprebautismales (Models/CursilloPrebautismal.cs)
CREATE TABLE cursillosprebautismales (
    id SERIAL PRIMARY KEY,
    nombrecompletonino VARCHAR(255) NOT NULL,
    nombrepadre VARCHAR(100) NOT NULL,
    nombremadre VARCHAR(100) NOT NULL,
    nombrepadrino VARCHAR(100) NOT NULL,
    nombremadrina VARCHAR(100) NOT NULL,
    parroquia INT REFERENCES parroquias(id) ON DELETE SET NULL,
    quieninscribe VARCHAR(255),
    telefono VARCHAR(20),
    celular VARCHAR(20),
    fechabautizo TIMESTAMP NOT NULL,
    fechacursillo TIMESTAMP NOT NULL,
    estado VARCHAR(50) DEFAULT 'PENDIENTE',
    observaciones TEXT
);

-- 10. Tabla: actasbautismo (Models/ActaBautismo.cs)
CREATE TABLE actasbautismo (
    id SERIAL PRIMARY KEY,
    nombrecompleto VARCHAR(255),
    lugarnacimiento VARCHAR(255),
    cedula VARCHAR(20),
    sexo VARCHAR(20),
    fechanacimiento TIMESTAMP NOT NULL,
    fechabautizo TIMESTAMP NOT NULL,
    cedulapadre VARCHAR(20),
    cedulamadre VARCHAR(20),
    cedulapadrino1 VARCHAR(20),
    cedulapadrino2 VARCHAR(20),
    cedulapadrino3 VARCHAR(20),
    cedulapadrino4 VARCHAR(20),
    cedulapadrino5 VARCHAR(20),
    pagina VARCHAR(50),
    tomo VARCHAR(50),
    acta VARCHAR(50),
    registrocivilano VARCHAR(10),
    registrociviltomo VARCHAR(50),
    registrocivilfolio VARCHAR(50),
    registrocivilacta VARCHAR(50),
    registrocivilfecha TIMESTAMP NOT NULL,
    observaciones TEXT,
    ministro_id INT REFERENCES ministros(id) ON DELETE SET NULL,
    sacerdote_id INT REFERENCES sacerdotes(id) ON DELETE SET NULL,
    iglesia_id INT REFERENCES iglesias(id) ON DELETE SET NULL,
    parroquia_id INT REFERENCES parroquias(id) ON DELETE SET NULL,
    cursillo_id INT REFERENCES cursillosprebautismales(id) ON DELETE SET NULL,
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- 11. Tabla: bautismosantiguos (Models/Antiguos.cs)
CREATE TABLE bautismosantiguos (
    id SERIAL PRIMARY KEY,
    nombrecompleto VARCHAR(255) NOT NULL,
    lugarnacimiento VARCHAR(255) NOT NULL,
    cedula VARCHAR(20) NOT NULL,
    sexo VARCHAR(20) NOT NULL,
    fechanacimiento TIMESTAMP NOT NULL,
    fechabautizo TIMESTAMP NOT NULL,
    nombrepadre VARCHAR(100) NOT NULL,
    nombremadre VARCHAR(100) NOT NULL,
    cedulapadrino1 VARCHAR(20) NOT NULL,
    cedulapadrino2 VARCHAR(20) NOT NULL,
    cedulapadrino3 VARCHAR(20),
    cedulapadrino4 VARCHAR(20),
    cedulapadrino5 VARCHAR(20),
    pagina VARCHAR(50),
    tomo VARCHAR(50),
    acta VARCHAR(50),
    registrocivilano VARCHAR(10),
    registrociviltomo VARCHAR(50) NOT NULL,
    registrocivilfolio VARCHAR(50) NOT NULL,
    registrocivilacta VARCHAR(50) NOT NULL,
    registrocivilfecha TIMESTAMP NOT NULL,
    observaciones TEXT NOT NULL,
    ministro_id INT REFERENCES ministros(id) ON DELETE SET NULL,
    iglesia_id INT REFERENCES iglesias(id) ON DELETE SET NULL,
    parroquia_id INT REFERENCES parroquias(id) ON DELETE SET NULL,
    sacerdote_id INT REFERENCES sacerdotes(id) ON DELETE SET NULL,
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- 12. Tabla: cursillosprematrimoniales (Models/CursilloPreMatrimonial.cs)
CREATE TABLE cursillosprematrimoniales (
    id SERIAL PRIMARY KEY,
    nombrenovio VARCHAR(255) NOT NULL,
    cedulanovio VARCHAR(20) NOT NULL,
    telefononovio VARCHAR(20) NOT NULL,
    fechanacimientonovio TIMESTAMP NOT NULL,
    nombrenovia VARCHAR(255) NOT NULL,
    cedulanovia VARCHAR(20) NOT NULL,
    telefononovia VARCHAR(20) NOT NULL,
    fechanacimientonovia TIMESTAMP NOT NULL,
    fechacursillo TIMESTAMP NOT NULL,
    fechamatrimonio TIMESTAMP NOT NULL,
    estado VARCHAR(50) DEFAULT 'PENDIENTE',
    parroquia INT REFERENCES parroquias(id) ON DELETE SET NULL,
    observaciones TEXT
);

-- 13. Tabla: actasprematrimoniales (Models/ActaPreMatrimonial.cs)
CREATE TABLE actasprematrimoniales (
    id SERIAL PRIMARY KEY,
    nombrenovio VARCHAR(255) NOT NULL,
    lugarbautizonovio VARCHAR(255) NOT NULL,
    nombrepadrenovio VARCHAR(255) NOT NULL,
    nombremadrenovio VARCHAR(255) NOT NULL,
    fechanacimientonovio TIMESTAMP NOT NULL,
    fechabautizonovio TIMESTAMP NOT NULL,
    nombrenovia VARCHAR(255) NOT NULL,
    lugarbautizonovia VARCHAR(255) NOT NULL,
    nombrepadrenovia VARCHAR(255) NOT NULL,
    nombremadrenovia VARCHAR(255) NOT NULL,
    fechanacimientonovia TIMESTAMP NOT NULL,
    fechabautizonovia TIMESTAMP NOT NULL,
    fechamatrimonio TIMESTAMP NOT NULL,
    iglesia_id INT REFERENCES iglesias(id) ON DELETE SET NULL,
    ministro_id INT REFERENCES ministros(id) ON DELETE SET NULL,
    fecharegistrocivil TIMESTAMP NOT NULL,
    lugarregistrocivil VARCHAR(255) NOT NULL,
    numeroactaregistrocivil VARCHAR(50) NOT NULL,
    numeropaginaregistrocivil VARCHAR(50) NOT NULL,
    numertomoregistrocivil VARCHAR(50) NOT NULL,
    tomo VARCHAR(50) NOT NULL,
    pagina VARCHAR(50) NOT NULL,
    acta VARCHAR(50) NOT NULL,
    fecharegistro TIMESTAMP NOT NULL,
    observaciones TEXT NOT NULL,
    testigo1 VARCHAR(255) NOT NULL,
    testigo2 VARCHAR(255) NOT NULL,
    testigo3 VARCHAR(255),
    testigo4 VARCHAR(255),
    cursillo_id INT REFERENCES cursillosprematrimoniales(id) ON DELETE SET NULL,
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- 14. Tabla: actasdematrimonio (Models/ActaDeMatrimonio.cs)
CREATE TABLE actasdematrimonio (
    id SERIAL PRIMARY KEY,
    padrino1 VARCHAR(255) NOT NULL,
    padrino2 VARCHAR(255) NOT NULL,
    padrino3 VARCHAR(255),
    padrino4 VARCHAR(255),
    fecha_matrimonio TIMESTAMP NOT NULL,
    fecharegistro TIMESTAMP,
    acta_prematrimonial_id INT REFERENCES actasprematrimoniales(id) ON DELETE SET NULL,
    iglesia_id INT REFERENCES iglesias(id) ON DELETE SET NULL,
    ministro_id INT REFERENCES ministros(id) ON DELETE SET NULL,
    acta VARCHAR(50),
    tomo VARCHAR(50),
    pagina VARCHAR(50),
    observaciones TEXT
);

-- 15. Tabla: matrimoniosantiguos (Models/MatrimoniosAntiguos.cs)
CREATE TABLE matrimoniosantiguos (
    id SERIAL PRIMARY KEY,
    nombrenovio VARCHAR(255) NOT NULL,
    lugarbautizonovio VARCHAR(255),
    cedulapadrenovio VARCHAR(20),
    cedulamadrenovio VARCHAR(20),
    fechanacimientonovio TIMESTAMP NOT NULL,
    fechabautizonovio TIMESTAMP NOT NULL,
    nombrenovia VARCHAR(255) NOT NULL,
    lugarbautizonovia VARCHAR(255),
    cedulapadrenovia VARCHAR(20),
    cedulamadrenovia VARCHAR(20),
    fechanacimientonovia TIMESTAMP NOT NULL,
    fechabautizonovia TIMESTAMP NOT NULL,
    fechamatrimonio TIMESTAMP NOT NULL,
    fecharegistrocivil TIMESTAMP NOT NULL,
    lugarregistrocivil VARCHAR(255),
    numeroactaregistrocivil VARCHAR(255),
    tomo VARCHAR(50),
    pagina VARCHAR(50),
    acta VARCHAR(50),
    fecharegistro TIMESTAMP NOT NULL,
    cedulatestigo1 VARCHAR(20),
    cedulatestigo2 VARCHAR(20),
    cedulatestigo3 VARCHAR(20),
    cedulatestigo4 VARCHAR(20),
    ministro_id INT REFERENCES ministros(id) ON DELETE SET NULL,
    iglesia_id INT REFERENCES iglesias(id) ON DELETE SET NULL,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- 16. Tabla: catequistas (Models/Catequista.cs)
CREATE TABLE catequistas (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    apellido VARCHAR(255) NOT NULL,
    cedula VARCHAR(20) UNIQUE,
    telefono VARCHAR(20),
    direccion TEXT,
    email VARCHAR(255),
    fecha_nacimiento TIMESTAMP,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- 17. Tabla: periodos (Models/Periodo.cs)
CREATE TABLE periodos (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    fecha_inicio TIMESTAMP NOT NULL,
    fecha_fin TIMESTAMP NOT NULL,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- 18. Tabla: alumnoscatequesis (Models/AlumnoCatequesis.cs)
CREATE TABLE alumnoscatequesis (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    apellido VARCHAR(255) NOT NULL,
    fechanacimiento TIMESTAMP NOT NULL,
    fechabautizo TIMESTAMP NOT NULL,
    cedula VARCHAR(20),
    direccion TEXT NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    foto BYTEA,
    nacionalidad VARCHAR(50) NOT NULL,
    estadocivil VARCHAR(50) NOT NULL,
    correo VARCHAR(255),
    nombrerepresentante VARCHAR(255) NOT NULL,
    telefonorepresentante VARCHAR(50) NOT NULL,
    relacionconelestudiante VARCHAR(50) NOT NULL,
    nivel VARCHAR(50) NOT NULL,
    catequista_id INT REFERENCES catequistas(id) ON DELETE SET NULL,
    catequista_id2 INT REFERENCES catequistas(id) ON DELETE SET NULL,
    paralelo VARCHAR(10) DEFAULT 'A',
    periodo_id INT REFERENCES periodos(id) ON DELETE SET NULL,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT NOT NULL
);

-- 19. Tabla: faltas (Models/Falta.cs)
CREATE TABLE faltas (
    id SERIAL PRIMARY KEY,
    alumno_id INT REFERENCES alumnoscatequesis(id) ON DELETE CASCADE,
    fechafalta TIMESTAMP NOT NULL,
    observacion VARCHAR(500) NOT NULL,
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- 20. Tabla: primerascomuniones (Models/PrimeraComunion.cs)
CREATE TABLE primerascomuniones (
    id SERIAL PRIMARY KEY,
    alumno_id INT REFERENCES alumnoscatequesis(id) ON DELETE SET NULL,
    nombre VARCHAR(255) NOT NULL,
    apellido VARCHAR(255) NOT NULL,
    fechanacimiento TIMESTAMP NOT NULL,
    cedula VARCHAR(20) NOT NULL,
    direccion TEXT NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    fechaprimeracomunion TIMESTAMP NOT NULL,
    parroquia_id INT REFERENCES parroquias(id) ON DELETE SET NULL,
    sacerdote_id INT REFERENCES sacerdotes(id) ON DELETE SET NULL,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT NOT NULL
);

-- 21. Tabla: confirmaciones (Models/Confirmacion.cs)
CREATE TABLE confirmaciones (
    id SERIAL PRIMARY KEY,
    alumno_id INT REFERENCES alumnoscatequesis(id) ON DELETE SET NULL,
    nombre VARCHAR(255) NOT NULL,
    apellido VARCHAR(255) NOT NULL,
    fechanacimiento TIMESTAMP NOT NULL,
    cedula VARCHAR(20) NOT NULL,
    direccion TEXT NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    fechaconfirmacion TIMESTAMP NOT NULL,
    parroquia_id INT REFERENCES parroquias(id) ON DELETE SET NULL,
    sacerdote_id INT REFERENCES sacerdotes(id) ON DELETE SET NULL,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- 22. Tabla: personas_evento (Models/PersonasEvento.cs)
CREATE TABLE personas_evento (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL,
    telefono VARCHAR(20),
    rol VARCHAR(50)
);

-- 23. Tabla: agendas (Models/Agenda.cs)
CREATE TABLE agendas (
    id SERIAL PRIMARY KEY,
    fechaagenda TIMESTAMP NOT NULL,
    hora INTERVAL,
    descripcion TEXT NOT NULL,
    lugar VARCHAR(255),
    responsables_ids TEXT,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    tipo_evento VARCHAR(50),
    titulo VARCHAR(100),
    notificar BOOLEAN DEFAULT TRUE,
    recordatorio_anticipacion INT DEFAULT 1
);

-- 24. Tabla: pagos (Models/Pago.cs)
CREATE TABLE pagos (
    id SERIAL PRIMARY KEY,
    tipo_pago VARCHAR(50) NOT NULL,
    concepto VARCHAR(50) NOT NULL,
    nombres VARCHAR(255) NOT NULL,
    apellidos VARCHAR(255) NOT NULL,
    direccion TEXT NOT NULL,
    cedula VARCHAR(20) NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    valor DECIMAL(10, 2) NOT NULL,
    tipopago VARCHAR(50) NOT NULL,
    fechapago TIMESTAMP NOT NULL,
    referencia_id INT,
    referencia_tabla VARCHAR(50),
    estado VARCHAR(20) DEFAULT 'PAGADO',
    observaciones TEXT,
    usuario_registro_id INT REFERENCES usuarios(id) ON DELETE SET NULL,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
