# =============================================================================
# SINCRONIZADOR AUTOMÁTICO PARA LAPTOP DEL PADRE (DESDE RESPALDO DE SECRETARÍA)
# 1. Guarda un respaldo preventivo de la laptop del Padre en su propia carpeta.
# 2. Busca en Google Drive el respaldo (.dump) más reciente de la Secretaría.
# 3. Si hay un respaldo nuevo de la Secretaría, lo restaura automáticamente
#    en el contenedor curia-postgres de la laptop del Padre.
# =============================================================================

param (
    [string]$CarpetaSecretaria = "Secretaria",
    [string]$CarpetaPadre = "Padre",
    [string]$RutaDriveSecretaria = ""
)

$ErrorActionPreference = "Continue"
$ProyectoDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ProyectoDir

$LogFile = Join-Path $ProyectoDir "sincronizacion_padre.log"
$StateFile = Join-Path $ProyectoDir ".ultimo_sync_secretaria.txt"

function Write-SyncLog([string]$Mensaje) {
    $Fecha = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $Linea = "[$Fecha] $Mensaje"
    Write-Host $Linea
    Add-Content -Path $LogFile -Value $Linea
}

Write-SyncLog "=== Iniciando sincronizacion automatica (Secretaria -> Padre) ==="

# 1. Esperar hasta 60s a que Docker Desktop y curia-postgres esten listos
$Intentos = 0
$PostgresListo = $false
while ($Intentos -lt 12) {
    docker exec curia-postgres pg_isready -U postgres -d curia > $null 2>&1
    if ($LASTEXITCODE -eq 0) {
        $PostgresListo = $true
        break
    }
    Start-Sleep -Seconds 5
    $Intentos++
}

if (-not $PostgresListo) {
    Write-SyncLog "[AVISO] El contenedor curia-postgres aun no esta listo. Se reintentara en el siguiente ciclo."
    exit 0
}

# 2. Localizar la carpeta de la Secretaria en Google Drive (G:\)
$RutaSec = ""
if ($RutaDriveSecretaria -ne "" -and (Test-Path $RutaDriveSecretaria)) {
    $RutaSec = $RutaDriveSecretaria
} else {
    $Candidatas = @(
        "G:\Mi unidad\Respaldo_Curia\$CarpetaSecretaria",
        "G:\Mi unidad\$CarpetaSecretaria",
        "G:\My Drive\Respaldo_Curia\$CarpetaSecretaria",
        "G:\My Drive\$CarpetaSecretaria"
    )

    foreach ($baseDrive in @("G:\Mi unidad", "G:\My Drive", "G:\.shortcut-targets-by-id")) {
        if (Test-Path $baseDrive) {
            $dirs = Get-ChildItem -Path $baseDrive -Directory -Depth 3 -ErrorAction SilentlyContinue | Where-Object { $_.Name -eq $CarpetaSecretaria }
            foreach ($d in $dirs) {
                $Candidatas += $d.FullName
            }
        }
    }

    foreach ($r in $Candidatas) {
        if ($r -ne "" -and (Test-Path $r -ErrorAction SilentlyContinue)) {
            $RutaSec = $r
            break
        }
    }
}

if ($RutaSec -eq "") {
    Write-SyncLog "[AVISO] No se encontro la carpeta de Google Drive '$CarpetaSecretaria' en G:\. Verifique que Google Drive for Desktop este abierto."
    exit 0
}

# 3. Buscar el archivo .dump mas reciente en Diarios o Semanales de la Secretaria
$UltimoDump = Get-ChildItem -Path $RutaSec -Filter "*.dump" -Recurse -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if ($null -eq $UltimoDump) {
    Write-SyncLog "[AVISO] Aun no hay archivos .dump en la carpeta de la Secretaria ($RutaSec)."
    exit 0
}

$FirmaDump = "$($UltimoDump.Name)|$($UltimoDump.LastWriteTime.ToString('yyyyMMddHHmmss'))|$($UltimoDump.Length)"
$UltimaFirmaAplicada = ""
if (Test-Path $StateFile) {
    $UltimaFirmaAplicada = (Get-Content $StateFile -Raw).Trim()
}

if ($FirmaDump -eq $UltimaFirmaAplicada) {
    Write-SyncLog "[OK] La laptop del Padre ya tiene aplicado el ultimo respaldo de la Secretaria ($($UltimoDump.Name))."
    exit 0
}

# 4. Respaldo preventivo de la laptop del Padre hacia la carpeta del Padre antes de restaurar
$BackupScript = Join-Path $ProyectoDir "backup_curia.local.ps1"
if (-not (Test-Path $BackupScript)) {
    $BackupScript = Join-Path $ProyectoDir "backup_curia.ps1"
}
if (Test-Path $BackupScript) {
    Write-SyncLog "[1/2] Generando respaldo preventivo de la laptop del Padre en carpeta '$CarpetaPadre'..."
    & $BackupScript -Parroquia $CarpetaPadre -Tipo diario | Out-Null
}

# 5. Restaurar el respaldo mas reciente de la Secretaria en curia-postgres
Write-SyncLog "[2/2] Importando datos recientes de la Secretaria desde: $($UltimoDump.FullName)..."
docker cp "$($UltimoDump.FullName)" curia-postgres:/tmp/sync_secretaria.dump
docker exec curia-postgres pg_restore -U postgres -d curia --clean --if-exists --no-owner --no-privileges /tmp/sync_secretaria.dump > $null 2>&1
docker exec curia-postgres rm -f /tmp/sync_secretaria.dump

Set-Content -Path $StateFile -Value $FirmaDump
Write-SyncLog "[EXITO] Base de datos del Padre actualizada con el respaldo '$($UltimoDump.Name)' de la Secretaria."
