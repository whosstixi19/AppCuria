-- Script corregido para crear todas las tablas de la base de datos Eclesoft
-- Versión sin errores de dependencias circulares

-- PASO 1: Crear todas las tablas SIN restricciones de clave foránea

-- Tabla: Feligreses
CREATE TABLE Feligreses (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    apellido VARCHAR(255) NOT NULL,
    cedula VARCHAR(20) UNIQUE,
    fechanacimiento DATE,
    direccion VARCHAR(255),
    telefono VARCHAR(20),
    email VARCHAR(255),
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- Tabla: Usuarios
CREATE TABLE Usuarios (
    id SERIAL PRIMARY KEY,
    correo VARCHAR(255) UNIQUE NOT NULL,    
    nombre VARCHAR(255),
    apellido VARCHAR(255),
    rol VARCHAR(50) DEFAULT 'USUARIO',
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    ultimo_acceso TIMESTAMP,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    pass VARCHAR(255) NOT NULL,
    salt VARCHAR(255) NOT NULL,
    iterations INT DEFAULT 10000
);

-- Tabla: Sacerdotes *Eliminada la restricción de parroquia_id para evitar dependencia circular*
CREATE TABLE Sacerdotes (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    apellido VARCHAR(255) NOT NULL,
    cedula VARCHAR(20) UNIQUE,
    telefono VARCHAR(20),
    email VARCHAR(255),
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- Tabla: Ministros *Eliminada la restricción de iglesia_id para evitar dependencia circular*
-- Nota: La relación con Iglesias se maneja a través de la tabla Iglesias
CREATE TABLE Ministros (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    apellido VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    telefono VARCHAR(20),
    fechaordenacion DATE,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- Tabla: Catequistas
CREATE TABLE Catequistas (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    apellido VARCHAR(255) NOT NULL,
    cedula VARCHAR(20) UNIQUE,
    telefono VARCHAR(20),
    direccion TEXT,
    email VARCHAR(255),
    fecha_nacimiento DATE,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- Tabla: Parroquias
CREATE TABLE Parroquias (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    direccion TEXT,
    telefono VARCHAR(20),
    email VARCHAR(255),
    sacerdote_id INTEGER,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- Tabla: Iglesias
CREATE TABLE Iglesias (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    direccion VARCHAR(255),
    ciudad VARCHAR(255),
    telefono VARCHAR(20),
    email VARCHAR(255),
    ministro_id INTEGER,
    parroquia_id INTEGER,
    denominacion VARCHAR(255),
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- Tabla: Periodos
CREATE TABLE Periodos (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- Tabla: Misas
CREATE TABLE Misas (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    hora_inicio TIME NOT NULL,
    hora_fin TIME,
    dia_semana VARCHAR(20),
    iglesia_id INTEGER,
    sacerdote_id INTEGER,
    descripcion TEXT,
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- Tabla: Barrios
CREATE TABLE Barrios (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    ministro_id INTEGER,
    iglesia_id INTEGER,
    misa_id INTEGER,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- Tabla: CatequistasNiveles
CREATE TABLE CatequistasNiveles (
    id SERIAL PRIMARY KEY,
    catequista_id INTEGER NOT NULL,
    periodo_id INTEGER NOT NULL,
    nivel VARCHAR(50) NOT NULL,
    paralelo VARCHAR(10),
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- Tabla: AlumnosCatequesis
CREATE TABLE AlumnosCatequesis (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    apellido VARCHAR(255) NOT NULL,
    fechanacimiento DATE,
    fechabautizo DATE,
    cedula VARCHAR(20),
    direccion TEXT,
    telefono VARCHAR(20),
    foto BYTEA,
    nacionalidad VARCHAR(50),
    estadocivil VARCHAR(50),
    correo VARCHAR(255),
    nombrerepresentante VARCHAR(255),
    telefonorepresentante VARCHAR(20),
    relacionconelestudiante VARCHAR(100),
    nivel VARCHAR(50),
    catequista_id INTEGER,
    periodo_id INTEGER,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- Tabla: Faltas
CREATE TABLE Faltas (
    id SERIAL PRIMARY KEY,
    alumno_id INTEGER NOT NULL,
    fechafalta DATE NOT NULL,
    observacion TEXT,
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- Tabla: CursillosPrebautismales
CREATE TABLE CursillosPrebautismales (
    id SERIAL PRIMARY KEY,
    nombrecompletonino VARCHAR(255) NOT NULL,
    quieninscribe VARCHAR(255),
    telefono VARCHAR(20),
    celular VARCHAR(20),
    fechabautizo DATE,
    fechacursillo DATE NOT NULL,
    estado VARCHAR(50) DEFAULT 'PENDIENTE',
    cedulapadre VARCHAR(20),
    cedulamadre VARCHAR(20),
    cedulapadrino VARCHAR(20),
    cedulamadrina VARCHAR(20),
    observaciones TEXT
);

-- Tabla: ActasBautismo
CREATE TABLE ActasBautismo (
    id SERIAL PRIMARY KEY,
    nombrecompleto VARCHAR(255) NOT NULL,
    lugarnacimiento VARCHAR(255),
    cedula VARCHAR(20),
    sexo VARCHAR(10),
    fechanacimiento DATE,
    fechabautizo DATE NOT NULL,
    pagina VARCHAR(50),
    tomo VARCHAR(50),
    acta VARCHAR(50),
    registrocivilano VARCHAR(10),
    registrociviltomo VARCHAR(50),
    registrocivilfolio VARCHAR(50),
    registrocivilacta VARCHAR(50),
    registrocivilfecha DATE,
    observaciones TEXT,
    cedulapadre VARCHAR(20),
    cedulamadre VARCHAR(20),
    cedulapadrino1 VARCHAR(20),
    cedulapadrino2 VARCHAR(20),
    cedulapadrino3 VARCHAR(20),
    cedulapadrino4 VARCHAR(20),
    cedulapadrino5 VARCHAR(20),
    ministro_id INTEGER,
    iglesia_id INTEGER,
    parroquia_id INTEGER,
    cursillo_id INTEGER,
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- Tabla: BautismosAntiguos
CREATE TABLE BautismosAntiguos (
    id SERIAL PRIMARY KEY,
    nombrecompleto VARCHAR(255) NOT NULL,
    lugarnacimiento VARCHAR(255),
    cedula VARCHAR(20),
    sexo VARCHAR(10),
    fechanacimiento DATE,
    fechabautizo DATE NOT NULL,
    pagina VARCHAR(50),
    tomo VARCHAR(50),
    acta VARCHAR(50),
    registrocivilano VARCHAR(10),
    registrociviltomo VARCHAR(50),
    registrocivilfolio VARCHAR(50),
    registrocivilacta VARCHAR(50),
    registrocivilfecha DATE,
    observaciones TEXT,
    cedulapadrino1 VARCHAR(20),
    cedulapadrino2 VARCHAR(20),
    cedulapadrino3 VARCHAR(20),
    cedulapadrino4 VARCHAR(20),
    cedulapadrino5 VARCHAR(20),
    cedulapadre VARCHAR(20),
    cedulamadre VARCHAR(20),
    ministro_id INTEGER,
    iglesia_id INTEGER,
    parroquia_id INTEGER,
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- Tabla: CursillosPrematrimoniales
CREATE TABLE CursillosPrematrimoniales (
    id SERIAL PRIMARY KEY,
    nombrenovio VARCHAR(255) NOT NULL,
    cedulanovio VARCHAR(20),
    telefononovio VARCHAR(20),
    fechanacimientonovio DATE,
    nombrenovia VARCHAR(255) NOT NULL,
    cedulanovia VARCHAR(20),
    telefononovia VARCHAR(20),
    fechanacimientonovia DATE,
    fechacursillo DATE NOT NULL,
    fechamatrimonio DATE,
    estado VARCHAR(50) DEFAULT 'PENDIENTE',
    observaciones TEXT
);

-- Tabla: ActasPreMatrimoniales
CREATE TABLE ActasPreMatrimoniales (
    id SERIAL PRIMARY KEY,
    nombrenovio VARCHAR(255) NOT NULL,
    lugarbautizonovio VARCHAR(255),
    fechanacimientonovio DATE,
    fechabautizonovio DATE,
    nombrenovia VARCHAR(255) NOT NULL,
    lugarbautizonovia VARCHAR(255),
    fechanacimientonovia DATE,
    fechabautizonovia DATE,
    fechamatrimonio DATE,
    fecharegistrocivil DATE,
    lugarregistrocivil VARCHAR(255),
    numeroactaregistrocivil VARCHAR(50),
    tomo VARCHAR(50),
    pagina VARCHAR(50),
    acta VARCHAR(50),
    fecharegistro DATE,
    observaciones TEXT,
    cedulapadrenovio VARCHAR(20),
    cedulamadrenovio VARCHAR(20),
    cedulapadrenovia VARCHAR(20),
    cedulamadrenovia VARCHAR(20),
    cedulatestigo1 VARCHAR(20),
    cedulatestigo2 VARCHAR(20),
    cedulatestigo3 VARCHAR(20),
    cedulatestigo4 VARCHAR(20),
    cursillo_id INTEGER,
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- Tabla: ActasDeMatrimonio
CREATE TABLE ActasDeMatrimonio (
    id SERIAL PRIMARY KEY,
    acta_prematrimonial_id INTEGER,
    iglesia_id INTEGER,
    ministro_id INTEGER,
    fecha_matrimonio DATE NOT NULL,
    padrino1 VARCHAR(255),
    padrino2 VARCHAR(255),
    padrino3 VARCHAR(255),
    padrino4 VARCHAR(255),
    tomo VARCHAR(50),
    pagina VARCHAR(50),
    acta VARCHAR(50),
    fecharegistro DATE,
    observaciones TEXT,
    cedulapadrino1 VARCHAR(20),
    cedulapadrino2 VARCHAR(20),
    cedulapadrino3 VARCHAR(20),
    cedulapadrino4 VARCHAR(20),
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- Tabla: MatrimoniosAntiguos
CREATE TABLE MatrimoniosAntiguos (
    id SERIAL PRIMARY KEY,
    nombrenovio VARCHAR(255) NOT NULL,
    lugarbautizonovio VARCHAR(255),
    fechanacimientonovio DATE,
    fechabautizonovio DATE,
    nombrenovia VARCHAR(255) NOT NULL,
    lugarbautizonovia VARCHAR(255),
    fechanacimientonovia DATE,
    fechabautizonovia DATE,
    fechamatrimonio DATE NOT NULL,
    fecharegistrocivil DATE,
    lugarregistrocivil VARCHAR(255),
    numeroactaregistrocivil VARCHAR(255),
    tomo VARCHAR(50),
    pagina VARCHAR(50),
    acta VARCHAR(50),
    fecharegistro DATE,
    observaciones TEXT,
    cedulapadrenovia VARCHAR(20),
    cedulapadrenovio VARCHAR(20),
    cedulamadrenovia VARCHAR(20),
    cedulamadrenovio VARCHAR(20),
    cedulatestigo1 VARCHAR(20),
    cedulatestigo2 VARCHAR(20),
    cedulatestigo3 VARCHAR(20),
    cedulatestigo4 VARCHAR(20),
    iglesia_id INTEGER,
    ministro_id INTEGER,
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- Tabla: PrimerasComuniones
CREATE TABLE PrimerasComuniones (
    id SERIAL PRIMARY KEY,
    alumno_id INTEGER,
    nombre VARCHAR(255),
    apellido VARCHAR(255),
    fechanacimiento DATE,
    cedula VARCHAR(20),
    direccion TEXT,
    telefono VARCHAR(20),
    fechaprimeracomunion DATE NOT NULL,
    parroquia_id INTEGER,
    sacerdote_id INTEGER,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- Tabla: Confirmaciones
CREATE TABLE Confirmaciones (
    id SERIAL PRIMARY KEY,
    alumno_id INTEGER,
    nombre VARCHAR(255),
    apellido VARCHAR(255),
    fechanacimiento DATE,
    cedula VARCHAR(20),
    direccion TEXT,
    telefono VARCHAR(20),
    fechaconfirmacion DATE NOT NULL,
    parroquia_id INTEGER,
    sacerdote_id INTEGER,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    observaciones TEXT
);

-- Tabla: Agendas
CREATE TABLE Agendas (
    id SERIAL PRIMARY KEY,
    fechaagenda DATE NOT NULL,
    hora TIME,
    descripcion TEXT,
    lugar VARCHAR(255),
    responsable_id INTEGER,
    estado VARCHAR(20) DEFAULT 'ACTIVO'
);

-- Tabla: AyudasSociales
CREATE TABLE AyudasSociales (
    id SERIAL PRIMARY KEY,
    foto BYTEA,
    FotoBase64 TEXT,
    nombre VARCHAR(255) NOT NULL,
    apellidos VARCHAR(255) NOT NULL,
    cedula VARCHAR(20) UNIQUE,
    edad INT,
    genero VARCHAR(10),
    instruccion VARCHAR(255),
    estadocivil VARCHAR(50),
    religion VARCHAR(255),
    fechanacimiento DATE,
    provincia VARCHAR(255),
    canton VARCHAR(255),
    parroquia VARCHAR(255),
    calleprincipal VARCHAR(255),
    numero VARCHAR(50),
    callesecundaria VARCHAR(255),
    sector VARCHAR(255),
    telefono VARCHAR(20),
    referencia TEXT,
    discapacidad VARCHAR(255),
    especificardiscapacidad VARCHAR(255),
    porcentajediscapacidad INT,
    carnetdiscapacidad VARCHAR(255),
    recibebdh VARCHAR(255),
    jubilacion VARCHAR(255),
    trabaja VARCHAR(255),
    conquienvive VARCHAR(255),
    viviendaes VARCHAR(255),
    tipoconstruccion VARCHAR(255),
    descripcion TEXT,
    nombrerepresentante VARCHAR(255),
    telefonorepresentante VARCHAR(20),
    relacion VARCHAR(100),
    antecedentesmedicos TEXT,
    fechavisita DATE,
    observaciones TEXT,
    estado VARCHAR(20) DEFAULT 'ACTIVO',
    usuario_registro_id INTEGER
);

-- Tabla: Pagos
CREATE TABLE Pagos (
    id SERIAL PRIMARY KEY,
    tipo_pago VARCHAR(50) CHECK (tipo_pago IN ('BAUTISMO_CURSILLO', 'BAUTISMO_CELEBRACION', 'MATRIMONIO_CURSILLO', 'MATRIMONIO_CELEBRACION', 'OTRO')),
    concepto VARCHAR(50) NOT NULL,
    nombres VARCHAR(255) NOT NULL,
    apellidos VARCHAR(255) NOT NULL,
    direccion TEXT,
    cedula VARCHAR(20),
    telefono VARCHAR(20),
    valor DECIMAL(10, 2) NOT NULL,
    tipoPago VARCHAR(50),
    fechaPago DATE NOT NULL,
    referencia_id INTEGER,
    referencia_tabla VARCHAR(50),
    estado VARCHAR(20) DEFAULT 'PAGADO',
    observaciones TEXT,
    usuario_registro_id INTEGER,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- PASO 2: Agregar todas las restricciones de clave foránea

-- Restricciones para Sacerdotes
-- Eliminada la restricción de parroquia_id en Sacerdotes para evitar dependencia circular

-- Restricciones para Parroquias
ALTER TABLE Parroquias 
ADD CONSTRAINT fk_parroquia_sacerdote FOREIGN KEY (sacerdote_id) REFERENCES Sacerdotes(id) ON DELETE SET NULL;

-- Restricciones para Iglesias
ALTER TABLE Iglesias 
ADD CONSTRAINT fk_iglesia_ministro FOREIGN KEY (ministro_id) REFERENCES Ministros(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_iglesia_parroquia FOREIGN KEY (parroquia_id) REFERENCES Parroquias(id) ON DELETE SET NULL;

-- Restricciones para Ministros
-- Eliminada la restricción de iglesia_id en Ministros para evitar dependencia circular

-- Restricciones para Misas
ALTER TABLE Misas 
ADD CONSTRAINT fk_misa_iglesia FOREIGN KEY (iglesia_id) REFERENCES Iglesias(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_misa_sacerdote FOREIGN KEY (sacerdote_id) REFERENCES Sacerdotes(id) ON DELETE SET NULL;

-- Restricciones para Barrios
ALTER TABLE Barrios 
ADD CONSTRAINT fk_barrio_ministro FOREIGN KEY (ministro_id) REFERENCES Ministros(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_barrio_iglesia FOREIGN KEY (iglesia_id) REFERENCES Iglesias(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_barrio_misa FOREIGN KEY (misa_id) REFERENCES Misas(id) ON DELETE SET NULL;

-- Restricciones para CatequistasNiveles
ALTER TABLE CatequistasNiveles 
ADD CONSTRAINT fk_catequista_nivel_catequista FOREIGN KEY (catequista_id) REFERENCES Catequistas(id) ON DELETE CASCADE,
ADD CONSTRAINT fk_catequista_nivel_periodo FOREIGN KEY (periodo_id) REFERENCES Periodos(id) ON DELETE CASCADE;

-- Restricciones para AlumnosCatequesis
ALTER TABLE AlumnosCatequesis 
ADD CONSTRAINT fk_alumno_catequista FOREIGN KEY (catequista_id) REFERENCES Catequistas(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_alumno_periodo FOREIGN KEY (periodo_id) REFERENCES Periodos(id) ON DELETE SET NULL;

-- Restricciones para Faltas
ALTER TABLE Faltas 
ADD CONSTRAINT fk_falta_alumno FOREIGN KEY (alumno_id) REFERENCES AlumnosCatequesis(id) ON DELETE CASCADE;

-- Restricciones para CursillosPrebautismales
ALTER TABLE CursillosPrebautismales 
ADD CONSTRAINT fk_cursillo_padre FOREIGN KEY (cedulapadre) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_cursillo_madre FOREIGN KEY (cedulamadre) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_cursillo_padrino FOREIGN KEY (cedulapadrino) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_cursillo_madrina FOREIGN KEY (cedulamadrina) REFERENCES Feligreses(cedula) ON DELETE SET NULL;

-- Restricciones para ActasBautismo
ALTER TABLE ActasBautismo 
ADD CONSTRAINT fk_bautismo_padre FOREIGN KEY (cedulapadre) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_madre FOREIGN KEY (cedulamadre) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_padrino1 FOREIGN KEY (cedulapadrino1) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_padrino2 FOREIGN KEY (cedulapadrino2) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_padrino3 FOREIGN KEY (cedulapadrino3) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_padrino4 FOREIGN KEY (cedulapadrino4) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_padrino5 FOREIGN KEY (cedulapadrino5) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_ministro FOREIGN KEY (ministro_id) REFERENCES Ministros(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_iglesia FOREIGN KEY (iglesia_id) REFERENCES Iglesias(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_parroquia FOREIGN KEY (parroquia_id) REFERENCES Parroquias(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_cursillo FOREIGN KEY (cursillo_id) REFERENCES CursillosPrebautismales(id) ON DELETE SET NULL;

-- Restricciones para BautismosAntiguos
ALTER TABLE BautismosAntiguos 
ADD CONSTRAINT fk_bautismo_antiguo_padrino1 FOREIGN KEY (cedulapadrino1) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_antiguo_padrino2 FOREIGN KEY (cedulapadrino2) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_antiguo_padrino3 FOREIGN KEY (cedulapadrino3) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_antiguo_padrino4 FOREIGN KEY (cedulapadrino4) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_antiguo_padrino5 FOREIGN KEY (cedulapadrino5) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_antiguo_padre FOREIGN KEY (cedulapadre) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_antiguo_madre FOREIGN KEY (cedulamadre) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_antiguo_ministro FOREIGN KEY (ministro_id) REFERENCES Ministros(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_antiguo_iglesia FOREIGN KEY (iglesia_id) REFERENCES Iglesias(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_bautismo_antiguo_parroquia FOREIGN KEY (parroquia_id) REFERENCES Parroquias(id) ON DELETE SET NULL;

-- Restricciones para CursillosPrematrimoniales
ALTER TABLE CursillosPrematrimoniales 
ADD CONSTRAINT fk_cursillo_matrimonial_novio FOREIGN KEY (cedulanovio) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_cursillo_matrimonial_novia FOREIGN KEY (cedulanovia) REFERENCES Feligreses(cedula) ON DELETE SET NULL;

-- Restricciones para ActasPreMatrimoniales
ALTER TABLE ActasPreMatrimoniales 
ADD CONSTRAINT fk_acta_prematrimonial_padrenovio FOREIGN KEY (cedulapadrenovio) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_prematrimonial_madrenovio FOREIGN KEY (cedulamadrenovio) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_prematrimonial_padrenovia FOREIGN KEY (cedulapadrenovia) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_prematrimonial_madrenovia FOREIGN KEY (cedulamadrenovia) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_prematrimonial_testigo1 FOREIGN KEY (cedulatestigo1) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_prematrimonial_testigo2 FOREIGN KEY (cedulatestigo2) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_prematrimonial_testigo3 FOREIGN KEY (cedulatestigo3) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_prematrimonial_testigo4 FOREIGN KEY (cedulatestigo4) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_prematrimonial_cursillo FOREIGN KEY (cursillo_id) REFERENCES CursillosPrematrimoniales(id) ON DELETE SET NULL;

-- Restricciones para ActasDeMatrimonio
ALTER TABLE ActasDeMatrimonio 
ADD CONSTRAINT fk_acta_matrimonio_prematrimonial FOREIGN KEY (acta_prematrimonial_id) REFERENCES ActasPreMatrimoniales(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_matrimonio_iglesia FOREIGN KEY (iglesia_id) REFERENCES Iglesias(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_matrimonio_ministro FOREIGN KEY (ministro_id) REFERENCES Ministros(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_matrimonio_padrino1 FOREIGN KEY (cedulapadrino1) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_matrimonio_padrino2 FOREIGN KEY (cedulapadrino2) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_matrimonio_padrino3 FOREIGN KEY (cedulapadrino3) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_acta_matrimonio_padrino4 FOREIGN KEY (cedulapadrino4) REFERENCES Feligreses(cedula) ON DELETE SET NULL;

-- Restricciones para MatrimoniosAntiguos
ALTER TABLE MatrimoniosAntiguos 
ADD CONSTRAINT fk_matrimonio_antiguo_padrenovia FOREIGN KEY (cedulapadrenovia) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_matrimonio_antiguo_padrenovio FOREIGN KEY (cedulapadrenovio) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_matrimonio_antiguo_madrenovia FOREIGN KEY (cedulamadrenovia) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_matrimonio_antiguo_madrenovio FOREIGN KEY (cedulamadrenovio) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_matrimonio_antiguo_testigo1 FOREIGN KEY (cedulatestigo1) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_matrimonio_antiguo_testigo2 FOREIGN KEY (cedulatestigo2) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_matrimonio_antiguo_testigo3 FOREIGN KEY (cedulatestigo3) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_matrimonio_antiguo_testigo4 FOREIGN KEY (cedulatestigo4) REFERENCES Feligreses(cedula) ON DELETE SET NULL,
ADD CONSTRAINT fk_matrimonio_antiguo_iglesia FOREIGN KEY (iglesia_id) REFERENCES Iglesias(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_matrimonio_antiguo_ministro FOREIGN KEY (ministro_id) REFERENCES Ministros(id) ON DELETE SET NULL;

-- Restricciones para PrimerasComuniones
ALTER TABLE PrimerasComuniones 
ADD CONSTRAINT fk_primera_comunion_alumno FOREIGN KEY (alumno_id) REFERENCES AlumnosCatequesis(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_primera_comunion_parroquia FOREIGN KEY (parroquia_id) REFERENCES Parroquias(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_primera_comunion_sacerdote FOREIGN KEY (sacerdote_id) REFERENCES Sacerdotes(id) ON DELETE SET NULL;

-- Restricciones para Confirmaciones
ALTER TABLE Confirmaciones 
ADD CONSTRAINT fk_confirmacion_alumno FOREIGN KEY (alumno_id) REFERENCES AlumnosCatequesis(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_confirmacion_parroquia FOREIGN KEY (parroquia_id) REFERENCES Parroquias(id) ON DELETE SET NULL,
ADD CONSTRAINT fk_confirmacion_sacerdote FOREIGN KEY (sacerdote_id) REFERENCES Sacerdotes(id) ON DELETE SET NULL;

-- Restricciones para Agendas
ALTER TABLE Agendas 
ADD CONSTRAINT fk_agenda_responsable FOREIGN KEY (responsable_id) REFERENCES Usuarios(id) ON DELETE SET NULL;

-- Restricciones para AyudasSociales
ALTER TABLE AyudasSociales 
ADD CONSTRAINT fk_ayuda_social_usuario FOREIGN KEY (usuario_registro_id) REFERENCES Usuarios(id) ON DELETE SET NULL;

-- Restricciones para Pagos
ALTER TABLE Pagos 
ADD CONSTRAINT fk_pago_usuario FOREIGN KEY (usuario_registro_id) REFERENCES Usuarios(id) ON DELETE SET NULL;

-- PASO 3: Crear índices para mejorar el rendimiento
CREATE INDEX idx_feligreses_cedula ON Feligreses(cedula);
CREATE INDEX idx_feligreses_nombre_apellido ON Feligreses(nombre, apellido);
CREATE INDEX idx_actas_bautismo_nombre ON ActasBautismo(nombrecompleto);
CREATE INDEX idx_actas_bautismo_fecha ON ActasBautismo(fechabautizo);
CREATE INDEX idx_cursillos_prebautismales_nombre ON CursillosPrebautismales(nombrecompletonino);
CREATE INDEX idx_cursillos_prebautismales_fecha ON CursillosPrebautismales(fechacursillo);
CREATE INDEX idx_actas_prematrimoniales_novio ON ActasPreMatrimoniales(nombrenovio);
CREATE INDEX idx_actas_prematrimoniales_novia ON ActasPreMatrimoniales(nombrenovia);
CREATE INDEX idx_cursillos_prematrimoniales_fecha ON CursillosPrematrimoniales(fechacursillo);
CREATE INDEX idx_actas_matrimonio_fecha ON ActasDeMatrimonio(fecha_matrimonio);
CREATE INDEX idx_alumnos_catequesis_nombre ON AlumnosCatequesis(nombre, apellido);
CREATE INDEX idx_pagos_cedula ON Pagos(cedula);
CREATE INDEX idx_pagos_nombres ON Pagos(nombres, apellidos);
CREATE INDEX idx_pagos_tipo_pago ON Pagos(tipo_pago);
CREATE INDEX idx_pagos_fecha ON Pagos(fechaPago);

-- PASO 4: Insertar un usuario administrador por defecto
INSERT INTO Usuarios (correo, pass, salt, iterations, nombre, apellido, rol) 
VALUES ('admin@eclesoft.com', 'Xklwq05sez6kPpMkq9jUz+V0EfSa/V04hjHDd5ndjQ0=', 'x8faNF/z7LD6ZMFJFXmbfg==', '310000', 'Administrador', 'Sistema', 'ADMINISTRADOR');