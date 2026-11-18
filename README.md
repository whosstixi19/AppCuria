# Sistema de Gestión para la Curia de Cuenca

## Propósito de la Aplicación
El sistema está diseñado para satisfacer las necesidades administrativas y de registro de la Curia de Cuenca. Su objetivo principal es centralizar y facilitar la gestión de diferentes actividades relacionadas con la Iglesia y sus servicios.
La aplicacion a futuro se desplegara en una maquina virtual sea AWS (Amazon Web Services) o de Google Cloud, con contenedores Docker, el cual haciendo un `docker-compose up` se levantaran los microservicios, ademas se debera hacer la creacion de tablas que se encuentran en el archivo `Tablas.txt` en este mismo repositorio, usando el `PgAdmin` de Postgresql para la administracion de la base y a su solventar cualquier inconveniente que se presente.

## Funcionalidades Principales
La aplicación incluye módulos para:

- **Registro de Matrimonios:** Gestión de información y documentación relacionada con los matrimonios celebrados en las parroquias e iglesias de la Curia.
- **Registro de Bautizos:** Almacenamiento de datos sobre actas de bautismo, padres, padrinos, y fechas relevantes.
- **Catequesis:** Organización de actividades y registros de primera comunión y confirmación.
- **Pagos:** Control de contribuciones y pagos relacionados con los servicios de la Iglesia.
- **Gestión de Ministros:** Registro de información sobre ministros ordenados, sus parroquias asignadas, fechas de ordenación, y actividades relacionadas.
- **Usuarios:** Administración de cuentas de acceso para el sistema, asegurando un entorno seguro y gestionado.
- **Parroquias e Iglesias:** Información detallada de las parroquias e iglesias, incluyendo direcciones, contactos y observaciones relevantes.
- **Agenda:** Planificación y seguimiento de actividades programadas, incluyendo eventos litúrgicos y sociales.
- **Ayuda Social:** Registro de actividades y servicios de ayuda comunitaria ofrecidos por la Curia.
- **Feligreses:** Gestión de datos de los feligreses y sus participaciones en las actividades de la Iglesia.
- **Gestión de Iglesias:** Registro, edición, búsqueda y eliminación de iglesias. Asignación de ministro y parroquia a cada iglesia. Visualización de nombres de ministros y parroquias en vez de IDs.
- **Gestión de Parroquias:** Registro, edición, búsqueda y eliminación de parroquias. Asignación de sacerdote responsable y visualización de datos completos.
- **Gestión de Ministros:** Registro, edición, búsqueda y eliminación de ministros. Almacenamiento de datos personales, fecha de ordenación y observaciones.
- **Gestión de Catequistas:** Registro, edición, búsqueda y eliminación de catequistas. Almacenamiento de datos personales y de contacto.
- **Gestión de Misas:** Registro, edición, búsqueda y eliminación de misas. Asignación de iglesia y sacerdote a cada misa. Registro de horarios, días de la semana y descripciones.
- **Vistas Dinámicas y Formularios Mejorados:** Formularios con selects dinámicos para asignar ministros, sacerdotes y parroquias mostrando nombres completos. Tablas que muestran nombres en vez de IDs para relaciones foráneas.
- **Mensajes de éxito y error en operaciones CRUD:** Notificaciones visuales para el usuario tras cada operación.
- **Búsqueda en tiempo real:** Filtros de búsqueda instantánea en tablas de cada módulo.
- **Validaciones de datos en frontend y backend:** Validaciones de campos obligatorios, formatos de email, longitudes máximas, etc.
- **Eliminación de dependencias circulares en el modelo de datos:** Refactorización de las relaciones entre tablas para mayor claridad y robustez.
- **Documentación interna mejorada:** Comentarios y explicaciones en el código y vistas para facilitar el mantenimiento.

## Beneficios
Este sistema proporciona una plataforma centralizada para agilizar los procesos administrativos y mejorar la organización interna de la Curia. Al digitalizar registros y actividades, se facilita el acceso a información clave y se optimiza el trabajo del personal administrativo y religioso.

## Tecnologías Utilizadas
- **Backend:** ASP.NET Core MVC
- **Base de Datos:** PostgreSQL
- **App:** HTML, CSS, JavaScript, CSHTML (Componente Razor de ASP .NET)
- **Lenguajes:** C#, Javascript

## Instalación y Puesta en Marcha Local (Docker Desktop - Recomendada)
1. Clona este repositorio:
   ```bash
   git clone https://github.com/whosstixi19/AppCuria.git
   cd AppCuria
   ```
2. Levanta la base de datos PostgreSQL 17 (con `BD.sql` automático) y el servidor web ASP.NET Core 8:
   ```bash
   docker compose up -d --build
   ```
3. Ingresa en el navegador a `http://localhost:5155` con el usuario inicial `admin@curia.com` y contraseña temporal `Clave_curia`.
4. **Orden inicial de registro de catálogos:** Registra primero al **Sacerdote** (`Registrar -> Sacerdote`), luego la **Parroquia** (`Registrar -> Parroquia`), el **Ministro/Catequista**, la **Iglesia** y finalmente crea el usuario propio de la secretaria/párroco en **Crear Usuario**.
5. **Automatización local (Un solo clic):**
   - **Auto-actualización desde GitHub:** Ejecuta `instalar_auto_actualizacion.bat` para que Windows verifique y aplique actualizaciones automáticamente al iniciar sesión y cada día a las 08:30 AM sin tocar la base de datos.
   - **Correo SMTP de la Parroquia (Agenda `.ics`):** Ejecuta `configurar_correo_parroquia.bat` para guardar el correo y la contraseña de aplicación de 16 letras de la parroquia en el archivo local `.env` (ignorado por Git para no perderse en `git pull`).
   - **Doble respaldo automático (Local + Google Drive compartido):** Inicia sesión en Google Drive for Desktop con la cuenta de la parroquia, añade en *Compartido conmigo* el acceso directo a *Mi unidad* de la subcarpeta compartida por el Administrador Supremo (sin espacios, con guiones bajos, ej. `Parroquia_San_Sebastian`) y ejecuta `instalar_respaldos_automaticos.bat`.

Como funciona la aplicacion, mas claro, como realiza las operaciones CRUD con la base de datos, aqui te explico.
La aplicacion usa el patron MVC como arquitectura para su desarrollo.
Tendremos una carpeta llamada Conexion, con una clase llamada `ConexionPostgreSQL` donde se incluiran las tablas con los modelos de cada tabla creada, mismos que se encuentran en la carpeta `Models`, las operaciones CRUD se realizan en la carpeta `Controllers`, con su respectivo controlador.
En las siguientes carpetas se pondra la estructura a la que van las ventanas de la aplicacion.
`Views/Home`: En esta carpeta estan las ventanas principales de cada pestaña.
`Views/Shared`: En esta carpeta estara las pestañas para actualizar y eliminar cualquier registro que exista en las tablas de la base de datos y tambien para imprimir certificados.
`Views/Submenus`: En esta carpeta se encontraran las Vistas principales de cada pestaña, una puede tener subpestañas, como por ejemplo `Cursullo Prebautismal | Acta de Bautismo | Bautismos Antiguos`
Ahora lo primordial de como la vista accede al Controlador es por medio del entorno de .NET, en las vistas `.cshtml` existe una propiedad en cada etiqueta, que son, `asp-controller`, `aps-action`, `asp-route-id` y otros, esta aplicacion solo usa estos tres en su mayoria.
Entonces por ejemplo, dentro de un formulario con la etiqueta `<form>` podemos usar la propiedad `asp-controller` y pondremos el nombre del controlador en la entre las comillas, y algo particular aqui es la forma de referenciar al controlador, si bien podemos tener un controlador llamado `ActaBautismoController`, dentro de la propiedad `asp-controller` debes referenciarlo de esta forma: `asp-controller="ActaBautismo"`, pero no asi `asp-controller="ActaBautismoController"`, en este caso omitimos el final del nombre que seria `Controller` y lo dejamos como `ActaBautismo`, ya que esta tecnologia opera de esta manera, y en `asp-action` pondremos el nombre de nuestro metodo creado en el controlador como por ejemplo `asp-action="Create"` el cual es un metodo que tiene como argumento el modelo de ActaPrebautismal y simplemente se hace referencia de esta manera.
El modelo dentro de la vista se llama simplemente por el nombre de los `id's` de las etiquetas, los `id's` de cada etiqueta deben ser iguales a los de nombres de las variables de los modelos.

Otra particularidad bastante interasante es el uso de `@` dentro de los componentes `.cshtml` que usan `.razor` y este operador nos permitira acceder a variables de la logica escrita en C#, comunmente a los modelos

## Contribuciones
Ing. Pablo Bermeo
|
Jorge Lituma
|
Oscar Campoverde
|
Henry Granda
|
Marco Cajamarca

Video: https://youtu.be/TCuQUw8z730

**Nota:** Este sistema está diseñado para ser utilizado exclusivamente por la Curia de Cuenca y sus instituciones afiliadas.
