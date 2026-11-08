# =============================================================================
# SCRIPT DE DOBLE RESPALDO AUTOMATIZADO: ECLESOFT 2.0 (AplicacionCuria)
# Respaldo 1: Disco Local (C:\Users\User\Documents\RespaldosCuria)
# Respaldo 2: Nube (Google Drive: G:\Mi unidad\Respaldo_Curia)
# =============================================================================

param (
    [string]$Parroquia = "Parroqui",
    [string]$Tipo = "diario", # "diario" o "semanal"
    [string]$RutaLocalBase = "$env:USERPROFILE\Documents\RespaldosCuria"
)

# 1. Definir rutas en disco local
$RutaLocalParroquia = Join-Path $RutaLocalBase $Parroquia
$LocalDiarios = Join-Path $RutaLocalParroquia "Diarios"
$LocalSemanales = Join-Path $RutaLocalParroquia "Semanales"

# Crear directorios locales si no existen
if (-not (Test-Path $LocalDiarios)) { New-Item -ItemType Directory -Force -Path $LocalDiarios | Out-Null }
if (-not (Test-Path $LocalSemanales)) { New-Item -ItemType Directory -Force -Path $LocalSemanales | Out-Null }

# 2. Detectar ruta de Google Drive
$PosiblesRutasDrive = @(
    "G:\Mi unidad\Respaldo_Curia\$Parroquia",
    "G:\My Drive\Respaldo_Curia\$Parroquia",
    "$env:USERPROFILE\Google Drive\Respaldo_Curia\$Parroquia"
)

$RutaDriveParroquia = ""
foreach ($ruta in $PosiblesRutasDrive) {
    if (Test-Path (Split-Path $ruta -Parent) -ErrorAction SilentlyContinue) {
        $RutaDriveParroquia = $ruta
        break
    }
}

$DriveDiarios = ""
$DriveSemanales = ""
if ($RutaDriveParroquia -ne "") {
    $DriveDiarios = Join-Path $RutaDriveParroquia "Diarios"
    $DriveSemanales = Join-Path $RutaDriveParroquia "Semanales"
    if (-not (Test-Path $DriveDiarios)) { New-Item -ItemType Directory -Force -Path $DriveDiarios | Out-Null }
    if (-not (Test-Path $DriveSemanales)) { New-Item -ItemType Directory -Force -Path $DriveSemanales | Out-Null }
}

$FechaHora = Get-Date -Format "yyyyMMdd_HHmm"
$Semana = Get-Date -UFormat "%V_%Y"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " INICIANDO DOBLE RESPALDO DE BASE DE DATOS ($($Tipo.ToUpper()))" -ForegroundColor Cyan
Write-Host " Parroquia    : $Parroquia" -ForegroundColor Yellow
Write-Host " Destino Local: $RutaLocalParroquia" -ForegroundColor Yellow
if ($RutaDriveParroquia -ne "") {
    Write-Host " Destino Nube : $RutaDriveParroquia (Google Drive)" -ForegroundColor Green
} else {
    Write-Host " Destino Nube : [NO DETECTADO] (Se guardara solo en disco local)" -ForegroundColor Magenta
}
Write-Host "==========================================================" -ForegroundColor Cyan

# 3. Ejecutar el respaldo segun el tipo
if ($Tipo -eq "diario") {
    $NombreArchivo = "${Parroquia}_diario_${FechaHora}.dump"
    $ArchivoLocal = Join-Path $LocalDiarios $NombreArchivo

    # A. Extraccion desde el contenedor Docker hacia el disco Local
    Write-Host "[1/3] Extrayendo base de datos desde PostgreSQL hacia disco local..." -ForegroundColor White
    docker exec -t curia-postgres pg_dump -U postgres -F c -b -v -f /tmp/backup_temp.dump curia
    docker cp curia-postgres:/tmp/backup_temp.dump $ArchivoLocal
    docker exec curia-postgres rm -f /tmp/backup_temp.dump

    if (Test-Path $ArchivoLocal) {
        $TamanoKB = [math]::Round((Get-Item $ArchivoLocal).Length / 1KB, 2)
        Write-Host "[OK] Respaldo 1 (Local): $ArchivoLocal ($TamanoKB KB)" -ForegroundColor Green

        # B. Copia hacia Google Drive
        if ($DriveDiarios -ne "") {
            $ArchivoDrive = Join-Path $DriveDiarios $NombreArchivo
            Copy-Item -Path $ArchivoLocal -Destination $ArchivoDrive -Force
            Write-Host "[OK] Respaldo 2 (Nube) : $ArchivoDrive (Sincronizando con Drive)" -ForegroundColor Green
        }
    } else {
        Write-Host "[ERROR] No se pudo generar el archivo de respaldo local." -ForegroundColor Red
    }
}
elseif ($Tipo -eq "semanal") {
    $NombreArchivo = "${Parroquia}_SEMANA_${Semana}.dump"
    $ArchivoLocal = Join-Path $LocalSemanales $NombreArchivo

    # A. Extraccion hacia disco local
    Write-Host "[1/4] Extrayendo respaldo semanal consolidado hacia disco local..." -ForegroundColor White
    docker exec -t curia-postgres pg_dump -U postgres -F c -b -v -f /tmp/backup_temp.dump curia
    docker cp curia-postgres:/tmp/backup_temp.dump $ArchivoLocal
    docker exec curia-postgres rm -f /tmp/backup_temp.dump

    if (Test-Path $ArchivoLocal) {
        $TamanoKB = [math]::Round((Get-Item $ArchivoLocal).Length / 1KB, 2)
        Write-Host "[OK] Respaldo 1 Semanal (Local): $ArchivoLocal ($TamanoKB KB)" -ForegroundColor Green

        # B. Copia hacia Google Drive
        if ($DriveSemanales -ne "") {
            $ArchivoDrive = Join-Path $DriveSemanales $NombreArchivo
            Copy-Item -Path $ArchivoLocal -Destination $ArchivoDrive -Force
            Write-Host "[OK] Respaldo 2 Semanal (Nube) : $ArchivoDrive (Sincronizando con Drive)" -ForegroundColor Green
        }

        # C. Limpieza de temporales diarios (en Local y en Drive)
        Write-Host "[3/4] Limpiando respaldos diarios temporales locales..." -ForegroundColor Yellow
        $ViejosLocal = Get-ChildItem -Path $LocalDiarios -Filter "*.dump"
        $ViejosLocal | Remove-Item -Force
        Write-Host "[OK] Temporales locales eliminados ($($ViejosLocal.Count) archivos)." -ForegroundColor Green

        if ($DriveDiarios -ne "") {
            Write-Host "[4/4] Limpiando respaldos diarios temporales en Google Drive..." -ForegroundColor Yellow
            $ViejosDrive = Get-ChildItem -Path $DriveDiarios -Filter "*.dump"
            $ViejosDrive | Remove-Item -Force
            Write-Host "[OK] Temporales de Drive eliminados ($($ViejosDrive.Count) archivos)." -ForegroundColor Green
        }
    } else {
        Write-Host "[ERROR] No se pudo generar el respaldo semanal consolidado." -ForegroundColor Red
    }
}
