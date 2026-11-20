# =============================================================================
# AUTO-ACTUALIZADOR LOCAL: ECLESOFT 2.0 (AplicacionCuria)
# Verifica si hay cambios en GitHub (origin/main) y actualiza Docker solo si es necesario
# =============================================================================

param(
    [switch]$ModoManual
)

$ErrorActionPreference = "Continue"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProyectoDir = Split-Path -Parent $ScriptDir
Set-Location $ProyectoDir

$LogFile = Join-Path $ProyectoDir "actualizaciones.log"
function Write-Log([string]$Mensaje, [string]$Color = "White") {
    $Fecha = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $Linea = "[$Fecha] $Mensaje"
    Write-Host $Linea -ForegroundColor $Color
    Add-Content -Path $LogFile -Value $Linea
}

Write-Log "=== Iniciando verificacion de estado y actualizaciones ===" "Cyan"

# 1. Esperar hasta 120 segundos a que el motor de Docker Desktop este listo y desahogado
$Intentos = 0
$DockerListo = $false
while ($Intentos -lt 24) {
    docker info > $null 2>&1
    if ($LASTEXITCODE -eq 0) {
        $DockerListo = $true
        break
    }
    Start-Sleep -Seconds 5
    $Intentos++
}

if (-not $DockerListo) {
    Write-Log "[ERROR] Docker Desktop no esta en ejecucion tras 120s. Abortando." "Red"
    if ($ModoManual) { Write-Host "`nDocker no esta listo. Por favor abre Docker Desktop y reintenta." -ForegroundColor Yellow }
    exit 1
}

# 2. Asegurar que los contenedores actuales esten encendidos
docker compose up -d > $null 2>&1

# 3. Consultar si hay commits nuevos en GitHub (origin/main)
Write-Log "Comprobando si hay actualizaciones disponibles en el repositorio..." "Gray"
git fetch origin main > $null 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Log "[AVISO] Sin conexion a GitHub o repositorio inaccesible. El sistema sigue operando con la version actual." "Yellow"
    if ($ModoManual) { Write-Host "`nNo se pudo conectar a GitHub (sin internet). Se mantiene la version actual." -ForegroundColor Yellow }
    exit 0
}

$LocalHash = (git rev-parse HEAD).Trim()
$RemoteHash = (git rev-parse origin/main).Trim()

if ($LocalHash -eq $RemoteHash) {
    Write-Log "[OK] El sistema ya esta en la ultima version ($($LocalHash.Substring(0,7))). No requiere recompilar." "Green"
    if ($ModoManual) { Write-Host "`nTu sistema ya cuenta con la version mas reciente. No hay cambios pendientes." -ForegroundColor Green }
    exit 0
}

# 4. Descargar cambios y reconstruir contenedor web sin tocar el volumen de base de datos
Write-Log "[ACTUALIZANDO] Nueva version detectada ($($LocalHash.Substring(0,7)) -> $($RemoteHash.Substring(0,7))). Descargando cambios..." "Yellow"
git checkout -- scripts/backup_curia.ps1 > $null 2>&1
git pull origin main
if ($LASTEXITCODE -eq 0) {
    $Count = (git rev-list --count HEAD).Trim()
    $Short = (git rev-parse --short HEAD).Trim()
    $env:APP_VERSION = "2.0.$Count+$Short"
    Write-Log "[DOCKER] Reconstruyendo contenedor web (app-curia) en version $($env:APP_VERSION)..." "Cyan"
    docker compose up -d --build app-curia
    Write-Log "[OK] Actualizacion completada exitosamente a v$($env:APP_VERSION)." "Green"
    if ($ModoManual) { Write-Host "`nActualizacion completada exitosamente. Ya puedes usar el sistema en el navegador." -ForegroundColor Green }
} else {
    Write-Log "[ERROR] Fallo git pull. Revise si hay cambios locales sin confirmar." "Red"
    if ($ModoManual) { Write-Host "`nOcurrio un problema al descargar los cambios. Revisa la conexion o cambios locales." -ForegroundColor Red }
}
