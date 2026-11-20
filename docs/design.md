# Sistema de Diseño y Especificaciones Técnicas: ECLESOFT 2.0
**Curia Arquidiocesana de Cuenca**

Documento de referencia y fuente de verdad para el diseño y maquetación visual del sistema.

---

## 1. Identidad y Propósito

* **Nombre Oficial:** `ECLESOFT 2.0` (*Sistema de Gestión para la Curia de Cuenca*).
* **Entidad:** Curia Arquidiocesana de Cuenca (Ecuador) e instituciones católicas afiliadas.
* **Propósito:** Centralizar, digitalizar y agilizar la gestión administrativa, sacramental, catequética y pastoral de parroquias, iglesias y feligreses.
* **Usuarios Finales:**
  * **Personal administrativo y pastoral:** Secretarios parroquiales, sacerdotes, encargados de despacho.
  * **Administrador del sistema:** Control de roles y usuarios (`rol: admin`).

---

## 2. Paleta de Colores y Tokens Visuales

| Token / Rol | Valor Hex | Uso en la Aplicación |
|---|---|---|
| **Azul Curia Institucional** | `#003366` | Identidad corporativa, títulos de mayor jerarquía, encabezados principales. |
| **Dorado Litúrgico (Accent)** | `#D4AF37` | Borde inferior de submenús (`border-bottom: 2px solid #d4af37`), enlaces activos y hovers. |
| **Fondo Neutro Cálido** | `#F7F9FC` | Fondo de barras secundarias, tarjetas de submenú y áreas de contenido suave. |
| **Texto Principal** | `#343A40` | Títulos de campos, enlaces inactivos y cuerpo de texto. |
| **Texto Secundario / Muted** | `#6C757D` | Descripciones cortas, etiquetas auxiliares y versión del sistema. |
| **Primario Interactivo** | `#0D6EFD` / `#258CFB` | Botones de registro/búsqueda, estados de foco en inputs (`:focus`). |
| **Éxito (Success)** | `#198754` | Botones de selección de relación, alertas de guardado exitoso. |
| **Advertencia (Warning)** | `#FFC107` | Botón "Modificar" en filas de tablas, advertencias de validación. |
| **Peligro (Danger)** | `#DC3545` | Botón "Eliminar" en tablas, botón "Cerrar sesión", alertas de error. |

---

## 3. Tipografía y Estilos de Texto

* **Fuente Base:** Segoe UI, Roboto, Helvetica Neue, Arial, sans-serif (pila estándar Bootstrap).
* **Escala Tipográfica:**
  * `h1` / `display-5`: 1.75rem – 2.25rem (Títulos de vista principal, bienvenida).
  * `h2` – `h3`: 1.25rem – 1.5rem (Títulos de paneles y pestañas).
  * `h4`: 1.1rem (Títulos de sección en formularios y cabeceras de tablas).
  * `body`: 14px en móvil / 16px en pantallas `≥ 768px`.
  * `small`: 0.85rem (Metadatos de usuario, rol, fecha).

---

## 4. Estructura de Navegación Global

### 4.1 Barra Superior Principal (`_Layout.cshtml`)
* **Marca:** `ECLESOFT 2.0` (Negrita + sufijo en gris atenuado).
* **Módulos Principales (con FontAwesome):**
  1. `fa-home` **Inicio:** Resumen de bienvenida y sesión activa.
  2. `fa-cross` **Sacramentos:** Acceso a los libros de sacramentos.
  3. `fa-book` **Catequesis:** Registro de alumnos y faltas.
  4. `fa-certificate` **Certificados:** Búsqueda e impresión oficial en PDF.
  5. `fa-calendar` **Agenda:** Planificación de eventos litúrgicos/sociales y recordatorios.
  6. `fa-hands-helping` **Ayuda Social:** Ficha de beneficiarios de programas pastorales.
  7. `fa-question-circle` **Ayuda:** Manual y procedimientos para usuarios.
  8. `fa-user-plus` **Registrar:** Mantenimiento de catálogos base indispensables.
  9. `fa-credit-card` **Pagos:** Control de aranceles y contribuciones por servicios.
  10. `fa-user-cog` **Crear Usuario:** Gestión de usuarios (`admin`).
* **Área de Usuario (Derecha):** Indicador de usuario (`👤 Administrador`) + botón `Cerrar sesión`.

### 4.2 Subbarras Secundarias Horizontales
Ubicadas debajo del navbar con fondo `#F7F9FC` y línea inferior `#D4AF37`:
* **Sacramentos:** Bautizados | Primera Comunión | Confirmación | Matrimonios.
* **Certificados:** Pase de Nivel | Primera Comunión | Confirmación | Dispensas | Bautizos | Matrimonios.
* **Registrar (Catálogos Base):** Feligreses | Encargados | Sacerdotes | Parroquias | Iglesias | Ministros/Catequistas | Periodos | Misas | Barrios.

---

## 5. Patrón Arquitectónico de Pantallas (Dual Split Layout)

La inmensa mayoría de las interfaces operativas utiliza un diseño dividido en dos columnas:

```
+-----------------------------------------------------------------------------------+
|  [<- Volver]  Título del Módulo (ej. Gestión de Iglesias)                        |
|  [Tab 1: Registro Activo]  [Tab 2: Histórico / Antiguos]                          |
+------------------------------------------+----------------------------------------+
| COLUMNA IZQUIERDA (col-lg-5 / col-lg-6)  | COLUMNA DERECHA (col-lg-7 / col-lg-6)  |
|                                          |                                        |
| [Card: Formulario de Captura]            | [Card: Consulta y Filtrado]            |
| - Inputs apilados o a 2 columnas         | - Buscador: [ Input de texto ] [Buscar]|
| - Selects dinámicos vinculados           | - Tabla interactiva:                   |
|   (Sacerdote, Ministro, Parroquia)       |   * Columnas de datos legibles         |
| - Textarea de observaciones              |   * Acciones: [Modificar] [Eliminar]   |
| - Botones: [Registrar] [Limpiar]         | - Paginación / Scroll horizontal       |
+------------------------------------------+----------------------------------------+
```

---

## 6. Reglas de Contenido y Terminología

* **Nombres reales vs. IDs:** Todas las listas y tablas deben mostrar nombres legibles (ej. *Pbro. Juan Pérez* o *Parroquia San Blas*) y nunca identificadores numéricos en bruto.
* **Campos canónicos:**
  * Feligreses: Cédula (10 dígitos), Nombres, Apellidos, Contacto.
  * Sacramentos: Bautizado/Contrayentes, Padres, Padrinos/Testigos, Libro/Tomo/Página/Acta.
  * Ministros/Sacerdotes: Fecha de ordenación, parroquia asignada, firma digital.
* **Estados estándar en selects:** `ACTIVO` / `INACTIVO`.
