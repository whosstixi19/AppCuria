# ECLESOFT 2.0 — Sistema de Gestión Pastoral y Sacramental para la Curia de Cuenca

Plataforma web integral multi-parroquia diseñada para digitalizar, centralizar y respaldar la gestión sacramental, catequética, pastoral y administrativa de las parroquias y de la Arquidiócesis / Curia de Cuenca.

---

## 1. Arquitectura y Tecnologías

- **Backend:** ASP.NET Core 8.0 MVC (C#) con Entity Framework Core 8 y Npgsql.
- **Base de Datos:** PostgreSQL 17 (`BD.sql` unificado con 24 tablas relacionales).
- **Frontend:** Razor Views (`.cshtml`), Bootstrap 5, JavaScript / jQuery y Select2.
- **Generación de Documentos PDF:** `Rotativa.AspNetCore` (`wkhtmltopdf`) con inyección dinámica en Base64 del escudo propio de cada parroquia y firma digital de los párrocos.
- **Seguridad y Multi-Tenancy Lógico:**
  - Autenticación por cookies con hash criptográfico `PBKDF2-SHA256` (310,000 iteraciones) y sal aleatoria por usuario.
  - Cambio obligatorio de contraseña temporal en el primer inicio de sesión (`DebeCambiarClaveFilter`).
  - Protección global anti-CSRF (`AutoValidateAntiforgeryTokenAttribute`) en formularios y peticiones AJAX.
  - Aislamiento multi-sucursal (`ParroquiaTenantHelper`): cada parroquia opera únicamente sobre sus propios registros, mientras que el rol **Superadmin** (Curia Central) dispone de un selector global en tiempo real para auditar cualquier sede o ver el consolidado diocesano.
- **Pruebas Automatizadas:** Suite xUnit (`AplicacionCuria.Tests`) con 17 pruebas unitarias y de integración que validan seguridad, aislamiento multi-parroquia, contraseñas y despacho de correos/calendario.

---

## 2. Módulos Principales

1. **Sacramentos:**
   - **Bautizos:** Cursillos prebautismales, actas de bautismo y libro histórico de bautismos antiguos.
   - **Primera Comunión y Confirmación:** Inscripción vinculada a catequesis y libros sacramentales.
   - **Matrimonios:** Cursillos prematrimoniales, expedientes/actas prematrimoniales (con dispensas de consanguinidad, religión mixta y cultos), actas de matrimonio y matrimonios antiguos.
2. **Catequesis:** Registro de alumnos con fotografía, niveles, paralelos, representantes y control de asistencia/faltas.
3. **Certificados y Actas en PDF:** Emisión e impresión instantánea de certificados de Bautismo, Primera Comunión, Confirmación, Matrimonio, Soltería, Cursillos y Pase de Nivel con el escudo institucional de la parroquia emisora.
4. **Agenda Parroquial e Integración de Calendario (`.ics`):** Programación de eventos litúrgicos con asignación de sacerdotes/encargados y envío automático de invitaciones interactivas por correo electrónico compatibles con Google Calendar y Outlook (estándar RFC 5545).
5. **Ayuda Social:** Ficha socioeconómica integral por pasos (beneficiario, ubicación/croquis, salud/discapacidad, ingresos/egresos y visita domiciliaria) con generación de reporte impreso.
6. **Catálogos Diocesanos (`Registrar`) y Control de Aranceles (`Pagos`):** Administración de Feligreses, Encargados, Sacerdotes, Parroquias (con logo propio), Iglesias, Ministros, Catequistas, Periodos, Misas y recaudación vinculada a cada sacramento.

---

## 3. Modelos de Distribución, Despliegue y Versionado

El sistema está preparado para operar bajo dos modalidades sincronizadas con este repositorio:

- **Nube (Google Cloud Compute Engine + CI/CD):**
  - Despliegue automatizado mediante **GitHub Actions** (`.github/workflows/deploy.yml`).
  - Cada `git push` a la rama `main` compila la solución, ejecuta las 17 pruebas automatizadas y actualiza por SSH los contenedores Docker en la máquina virtual de Google Cloud sin tiempo de inactividad para la base de datos.
- **Instalación Local en Parroquias (Docker Desktop en Windows):**
  - Orquestación completa con `docker-compose.yml` (`curia-postgres` + `curia-web`) y volumen persistente `postgres_data`.
  - Capacidad de operar en la máquina principal (`http://localhost:5155`) o abrirse a todas las computadoras de la red local (LAN) de la parroquia mediante un nombre de dominio interno sin costo (ej. `http://cristosalvador`).
  - **Migración Local a Nube sin colisión de IDs:** Protocolo `MigracionParroquiaLocalANube.sql` para incorporar parroquias locales a la base central en la nube en cualquier momento.
- **Versionado Automático por Commit:**
  - El pie de página (*footer*, visible al desplazarse al final de la interfaz) muestra automáticamente el número de versión calculado a partir del historial de Git (`v2.0.<commits> (<hash>)`), permitiendo verificar al instante qué versión exacta está corriendo en cada parroquia o en la nube.

---

## Créditos y Contribuciones

- **Desarrollo y Arquitectura ECLESOFT 2.0:** José Tixi (`@whosstixi19`)

**Nota:** Este sistema está diseñado para uso exclusivo de la Curia Arquidiocesana de Cuenca y sus parroquias afiliadas.
