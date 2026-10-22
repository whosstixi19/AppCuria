-- Tabla: Feligres
CREATE TABLE Feligres (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255),
    apellido VARCHAR(255),
    cedula VARCHAR(20),
    fechanacimiento DATE,
    direccion VARCHAR(255),
    telefono VARCHAR(20)
);

CREATE TABLE Ministros (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255),
    apellido VARCHAR(255),
    email VARCHAR(255),
    telefono VARCHAR(20),
    iglesia VARCHAR(255),
    fechaordenacion DATE,
    observaciones TEXT
);

-- Tabla: Iglesia
CREATE TABLE Iglesia (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255),
    direccion VARCHAR(255),
    ciudad VARCHAR(255),
    telefono VARCHAR(20),
    email VARCHAR(255),
    ministro VARCHAR(255),
    denominacion VARCHAR(255),
    observaciones TEXT
);

CREATE TABLE Parroquia (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255),
    direccion TEXT,
    telefono VARCHAR(20)
);

ALTER TABLE Feligres ADD CONSTRAINT unique_cedula UNIQUE (cedula);


-- Tabla: ActaBautismo
CREATE TABLE ActaBautismo (
    id SERIAL PRIMARY KEY,
    nombrecompleto VARCHAR(255),
    lugarnacimiento VARCHAR(255),
    cedula VARCHAR(20),
    sexo VARCHAR(10),
    fechanacimiento DATE,
    fechabautizo DATE,
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
    ministro BIGINT,
    iglesia BIGINT,
    feligreses BIGINT,

    CONSTRAINT fk_cedulapadre FOREIGN KEY (cedulapadre) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulamadre FOREIGN KEY (cedulamadre) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulapadrino1 FOREIGN KEY (cedulapadrino1) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulapadrino2 FOREIGN KEY (cedulapadrino2) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulapadrino3 FOREIGN KEY (cedulapadrino3) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulapadrino4 FOREIGN KEY (cedulapadrino4) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulapadrino5 FOREIGN KEY (cedulapadrino5) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_ministro FOREIGN KEY (ministro) REFERENCES Ministros(id) ON DELETE SET NULL,
    CONSTRAINT fk_iglesia FOREIGN KEY (iglesia) REFERENCES Iglesia(id) ON DELETE SET NULL,
    CONSTRAINT fk_feligreses FOREIGN KEY (feligreses) REFERENCES Parroquia(id) ON DELETE SET NULL
);


-- Tabla: ActaDeMatrimonio
CREATE TABLE ActaDeMatrimonio (
    id SERIAL PRIMARY KEY,
    padrino1 VARCHAR(255),
    padrino2 VARCHAR(255),
    padrino3 VARCHAR(255),
    padrino4 VARCHAR(255),
    fecharegistro DATE
);

-- Tabla: ActaPreMatrimonial
CREATE TABLE ActaPreMatrimonial (
    id SERIAL PRIMARY KEY,
    nombrenovio VARCHAR(255),
    lugarbautizonovio VARCHAR(255),
    fechanacimientonovio DATE,
    fechabautizonovio DATE,
    nombrenovia VARCHAR(255),
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

    

    CONSTRAINT fk_cedulapadrenovio FOREIGN KEY (cedulapadrenovio) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulamadrenovio FOREIGN KEY (cedulamadrenovio) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulapadrenovia FOREIGN KEY (cedulapadrenovia) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulamadrenovia FOREIGN KEY (cedulamadrenovia) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulatestigo1 FOREIGN KEY (cedulatestigo1) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulatestigo2 FOREIGN KEY (cedulatestigo2) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulatestigo3 FOREIGN KEY (cedulatestigo3) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_cedulatestigo4 FOREIGN KEY (cedulatestigo4) REFERENCES Feligres(cedula) ON DELETE SET NULL
);

-- Tabla: Agenda
CREATE TABLE Agenda (
    id SERIAL PRIMARY KEY,
    fechaagenda DATE,
    hora TIME,
    descripcion TEXT,
    lugar VARCHAR(255)
);

-- Tabla: AlumnoCatequesis
CREATE TABLE AlumnoCatequesis (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255),
    apellido VARCHAR(255),
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
    relacionconelestudiante VARCHAR(100)
);

-- Tabla: Bautismos Antiguos
CREATE TABLE Antiguos (
    id SERIAL PRIMARY KEY,
    nombrecompleto VARCHAR(255),
    lugarnacimiento VARCHAR(255),
    cedula VARCHAR(20),
    sexo VARCHAR(10),
    fechanacimiento DATE,
    fechabautizo DATE,
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
    ministro BIGINT,
    iglesia BIGINT,
    feligreses BIGINT,

    CONSTRAINT fk_2cedulapadrino1 FOREIGN KEY (cedulapadrino1) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulapadrino2 FOREIGN KEY (cedulapadrino2) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulapadrino3 FOREIGN KEY (cedulapadrino3) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulapadrino4 FOREIGN KEY (cedulapadrino4) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulapadrino5 FOREIGN KEY (cedulapadrino5) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulapadre FOREIGN KEY (cedulapadre) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulamadre FOREIGN KEY (cedulamadre) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2ministro FOREIGN KEY (ministro) REFERENCES ministros(id) ON DELETE SET NULL,
    CONSTRAINT fk_2iglesia FOREIGN KEY (iglesia) REFERENCES iglesia(id) ON DELETE SET NULL,
    CONSTRAINT fk_2feligreses FOREIGN KEY (feligreses) REFERENCES parroquia(id) ON DELETE SET NULL
);

-- Tabla: AyudaSocial
CREATE TABLE AyudaSocial (
    id SERIAL PRIMARY KEY,
    foto BYTEA,
    FotoBase64 TEXT,
    nombre VARCHAR(255),
    apellidos VARCHAR(255),
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
    observaciones TEXT
);

-- Tabla: Barrio
CREATE TABLE Barrio (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255),
    ministro VARCHAR(255),
    iglesia VARCHAR(255),
    misa TIMESTAMP
);

-- Tabla: Confirmacion
CREATE TABLE Confirmacion (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255),
    apellido VARCHAR(255),
    fechanacimiento DATE,
    cedula VARCHAR(20),
    direccion TEXT,
    telefono VARCHAR(20),
    fechaconfirmacion DATE,
    parroquia VARCHAR(255),
    sacerdote VARCHAR(255)
);

-- Tabla: Cursillo Prebautismal
CREATE TABLE Cursillo (
    id SERIAL PRIMARY KEY,
    nombrecompletonino VARCHAR(255),
    quieninscribe VARCHAR(255),
    telefono VARCHAR(20),
    celular VARCHAR(20),
    fechabautizo DATE,
    fechacursillo DATE,
    estado VARCHAR(50),
    cedulapadre VARCHAR(20),
    cedulamadre VARCHAR(20),
    cedulapadrino VARCHAR(20),
    cedulamadrina VARCHAR(20),

    
    CONSTRAINT fk_3cedulapadre FOREIGN KEY (cedulapadre) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_3cedulamadre FOREIGN KEY (cedulamadre) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_3cedulapadrino FOREIGN KEY (cedulapadrino) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_3cedulamadrina FOREIGN KEY (cedulamadrina) REFERENCES Feligres(cedula) ON DELETE SET NULL
);

-- Tabla: CursilloPreMatrimonial
CREATE TABLE CursilloPreMatrimonial (
    id SERIAL PRIMARY KEY,
    nombrenovio VARCHAR(255),
    cedulanovio VARCHAR(20),
    telefononovio VARCHAR(20),
    fechanacimientonovio DATE,
    nombrenovia VARCHAR(255),
    cedulanovia VARCHAR(20),
    telefononovia VARCHAR(20),
    fechanacimientonovia DATE,
    fechacursillo DATE,
    fechamatrimonio DATE,
    estado VARCHAR(50)
);

-- Tabla: Faltas del alumno
CREATE TABLE falta(
    id SERIAL PRIMARY KEY,
    alumnoid INT NOT NULL,
    fechafalta DATE,
    observacion TEXT,


    CONSTRAINT falta_alumnoid_fkey FOREIGN KEY (alumnoid) REFERENCES AlumnoCatequesis(id)
);


-- Tabla: MatrimoniosAntiguos
CREATE TABLE MatrimoniosAntiguos (
    id SERIAL PRIMARY KEY,
    nombrenovio VARCHAR(255),
    lugarbautizonovio VARCHAR(255),
    fechanacimientonovio DATE,
    fechabautizonovio DATE,
    nombrenovia VARCHAR(255),
    lugarbautizonovia VARCHAR(255),
    fechanacimientonovia DATE,
    fechabautizonovia DATE,
    fechamatrimonio DATE,
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

    
    CONSTRAINT fk_2cedulapadrenovia FOREIGN KEY (cedulapadrenovia) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulapadrenovio FOREIGN KEY (cedulapadrenovio) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulamadrenovia FOREIGN KEY (cedulamadrenovia) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulamadrenovio FOREIGN KEY (cedulamadrenovio) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulatestigo1 FOREIGN KEY (cedulatestigo1) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulatestigo2 FOREIGN KEY (cedulatestigo2) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulatestigo3 FOREIGN KEY (cedulatestigo3) REFERENCES Feligres(cedula) ON DELETE SET NULL,
    CONSTRAINT fk_2cedulatestigo4 FOREIGN KEY (cedulatestigo4) REFERENCES Feligres(cedula) ON DELETE SET NULL
);


CREATE TABLE Pago (
    id SERIAL PRIMARY KEY,
    concepto VARCHAR(50),
    nombres VARCHAR(255),
    apellidos VARCHAR(255),
    direccion TEXT,
    cedula VARCHAR(20),
    telefono VARCHAR(20),
    valor DECIMAL(10, 2),
    tipoPago VARCHAR(50),
    fechaPago DATE,
    observaciones TEXT
);


CREATE TABLE PrimeraComunion (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255),
    apellido VARCHAR(255),
    fechanacimiento DATE,
    cedula VARCHAR(20),
    direccion TEXT,
    telefono VARCHAR(20),
    fechaprimeracomunion DATE,
    parroquia VARCHAR(255),
    sacerdote VARCHAR(255)
);


-- Tabla: Sacerdote
CREATE TABLE Sacerdote (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(255),
    apellido VARCHAR(255),
    cedula VARCHAR(20),
    parroquia VARCHAR(255),
    telefono VARCHAR(20),
    email VARCHAR(255)
);

CREATE TABLE Usuarios (
    id SERIAL PRIMARY KEY,
    correo VARCHAR(255) UNIQUE,
    pass VARCHAR(255)
);


--Tuplas para testeo de la base de datos

INSERT INTO Feligres (nombre, apellido, cedula, fechanacimiento, direccion, telefono) VALUES
('Juan', 'Pérez', '0102030405', '1985-06-15', 'Av. 12 de Abril y Tarqui', '0998765432'),
('María', 'González', '0104030506', '1990-09-21', 'Calle Larga y Vargas Machuca', '0987654321'),
('Luis', 'Rodríguez', '0101020304', '1978-12-10', 'Av. Remigio Crespo y Solano', '0996543210'),
('Ana', 'Vásquez', '0105060708', '1995-04-18', 'Sector Baños, Cuenca', '0981234567'),
('Carlos', 'Ortiz', '0107080901', '1980-01-30', 'Av. Loja y Unidad Nacional', '0991122334');


INSERT INTO Iglesia (nombre, direccion, ciudad, telefono, email, ministro, denominacion, observaciones) VALUES
('Catedral de la Inmaculada Concepción', 'Benigno Malo y Bolívar', 'Cuenca', '072841234', 'catedralcuenca@gmail.com', 'Padre Luis Sánchez', 'Católica', 'Iglesia principal de Cuenca'),
('Iglesia San Sebastián', 'Simón Bolívar y Coronel Talbot', 'Cuenca', '072839876', 'sansebastian@gmail.com', 'Padre Carlos Medina', 'Católica', 'Ubicada en el Parque San Sebastián'),
('Iglesia del Sagrario', 'Luis Cordero y Sucre', 'Cuenca', '072831234', 'sagrariocuenca@gmail.com', 'Padre Antonio Martínez', 'Católica', 'Conocida como la Catedral Vieja'),
('Iglesia de Todos Santos', 'Calle Larga y Bajada de Todos Santos', 'Cuenca', '072839123', 'todossantos@gmail.com', 'Padre José Pérez', 'Católica', 'Iglesia histórica de Cuenca'),
('Iglesia de San Blas', 'Simón Bolívar y Borrero', 'Cuenca', '072831567', 'sanblas@gmail.com', 'Padre Juan Ruiz', 'Católica', 'Ubicada cerca del mercado San Blas');

INSERT INTO Ministros (nombre, apellido, email, telefono, iglesia, fechaordenacion, observaciones) VALUES
('Luis', 'Sánchez', 'padreluis@gmail.com', '0987654321', 'Catedral de la Inmaculada Concepción', '2005-03-15', 'Encargado de la Catedral principal de Cuenca'),
('Carlos', 'Medina', 'padrecarlos@gmail.com', '0998765432', 'Iglesia San Sebastián', '2010-06-20', 'Con experiencia en misiones internacionales'),
('Antonio', 'Martínez', 'padreantonio@gmail.com', '0981234567', 'Iglesia del Sagrario', '2000-11-12', 'Responsable de varias parroquias en Cuenca'),
('José', 'Pérez', 'padrejose@gmail.com', '0996543210', 'Iglesia de Todos Santos', '2012-07-25', 'Participante activo en actividades juveniles'),
('Juan', 'Ruiz', 'padrejuan@gmail.com', '0989876543', 'Iglesia de San Blas', '2008-09-14', 'Foco en la pastoral social y ayuda comunitaria');

INSERT INTO Parroquia (nombre, direccion, telefono) VALUES
('Parroquia El Sagrario', 'Luis Cordero y Sucre', '072831234'),
('Parroquia San Sebastián', 'Simón Bolívar y Coronel Talbot', '072839876'),
('Parroquia Todos Santos', 'Calle Larga y Bajada de Todos Santos', '072839123'),
('Parroquia San Blas', 'Simón Bolívar y Borrero', '072831567'),
('Parroquia Cristo Rey', 'Av. Ordoñez Lasso y Paseo 3 de Noviembre', '072833456');

INSERT INTO Usuario (correo, pass) VALUES
('admin@iglesia.com', 'admin123'),
('usuario1@iglesia.com', 'password1'),
('usuario2@iglesia.com', 'password2'),
('usuario3@iglesia.com', 'password3'),
('usuario4@iglesia.com', 'password4');

