# Guía Operativa: Migración y Coexistencia Local vs. Google Cloud (ECLESOFT 2.0)

Esta guía describe el protocolo de arquitectura multi-parroquia para asegurar que ninguna sede sufra pérdida de datos, colisión de identificadores ni cruce de escudos/logos.

---

## 1. Arquitectura de Aislamiento (Multi-Tenancy)

1. **Parroquia Centralizada (Nube):**
   * Una sola base de datos unificada en Google Cloud (Compute Engine / Cloud SQL).
   * Cada registro (`actasbautismo`, `confirmaciones`, `cursillos`, etc.) tiene el discriminador `parroquia_id`.
   * El usuario de despacho solo ve los datos de su sede (`WHERE parroquia_id = @id`).
   * El Administrador Diocesano (Superadmin) puede inspeccionar cualquier parroquia o ver todas mediante el selector en la barra superior.

2. **Parroquia Autónoma (Instalación Local):**
   * Corre el mismo software en Docker (`docker-compose.yml`) o binario local con su PostgreSQL local.
   * Se le asigna de antemano el código de parroquia oficial que tendrá en la diócesis (ej. `parroquia_id = 4`).

---

## 2. Personalización de Escudos y Logos

* Cada parroquia puede subir su imagen PNG/JPG en el módulo **Gestión de Parroquias**.
* El escudo se almacena de forma segura en Base64 (`logo_base64`) asociado directamente al ID de la parroquia.
* Al emitir certificados en PDF (Bautismo, Confirmación, Primera Comunión, Matrimonio, Soltería, etc.), el sistema inyecta dinámicamente el escudo de la parroquia a la que pertenece el acta.
* Si una parroquia no tiene escudo propio cargado, el sistema automáticamente utiliza el escudo oficial de la Arquidiócesis (`/imagenes/escudo.png`).
* **Garantía técnica:** La imagen viaja atada al registro; es imposible que se mezcle el escudo de una parroquia con el de otra.

---

## 3. Protocolo de "Upgrade": Salto de Local a Google Cloud

Cuando una parroquia que operaba en local decide pasar a la nube:

1. **Fecha de Corte:**
   * Se notifica a la secretaría parroquial la fecha de corte.
   * Se cierra el sistema local temporalmente para evitar que se sigan registrando actas en la máquina local.

2. **Extracción del Respaldo:**
   * Se ejecuta en la máquina local:
     ```bash
     pg_dump -U postgres -d curia --data-only --inserts -f parroquia_local_backup.sql
     ```

3. **Carga en la Nube con Cero Choques:**
   * Se ejecuta el script [`MigracionParroquiaLocalANube.sql`](../MigracionParroquiaLocalANube.sql).
   * Los registros se insertan omitiendo el campo `id` autoincremental local; PostgreSQL en Google Cloud les genera el correlativo siguiente sin colisionar con ninguna otra parroquia.
   * Las llaves foráneas (`cursillo_id`) se remapean mediante la tabla temporal de traducción.

4. **Transición a la Web:**
   * Se le entrega al despacho parroquial la URL web en Google Cloud y sus credenciales.
   * La máquina local apaga su contenedor de base de datos y pasa a ser un cliente más que entra desde Google Chrome o Microsoft Edge.
