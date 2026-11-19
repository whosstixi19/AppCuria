# =============================================================================
# AUTO-ACTUALIZADOR LOCAL: ECLESOFT 2.0 (AplicacionCuria)
# Verifica si hay cambios en GitHub (origin/main) y actualiza Docker solo si es necesario
# =============================================================================

$ErrorActionPreference = "Continue"
$ProyectoDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ProyectoDir

$LogFile = Join-Path $ProyectoDir "actualizaciones.log"
function Write-Log([string]$Mensaje) {
    $Fecha = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $Linea = "[$Fecha] $Mensaje"
    Write-Host $Linea
    Add-Content -Path $LogFile -Value $Linea
}

Write-Log "=== Iniciando verificacion de estado y actualizaciones ==="

# 1. Esperar hasta 90 segundos a que el motor de Docker Desktop este listo
$Intentos = 0
$DockerListo = $false
while ($Intentos -lt 18) {
    docker info > $null 2>&1
    if ($LASTEXITCODE -eq 0) {
        $DockerListo = $true
        break
    }
    Start-Sleep -Seconds 5
    $Intentos++
}

if (-not $DockerListo) {
    Write-Log "[ERROR] Docker Desktop no esta en ejecucion tras 90s. Abortando."
    exit 1
}

# 2. Asegurar que los contenedores actuales esten encendidos
docker compose up -d > $null 2>&1

# 3. Consultar si hay commits nuevos en GitHub (origin/main)
git fetch origin main > $null 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Log "[AVISO] Sin conexion a GitHub o repositorio inaccesible. El sistema sigue operando con la version actual."
    exit 0
}

$LocalHash = (git rev-parse HEAD).Trim()
$RemoteHash = (git rev-parse origin/main).Trim()

if ($LocalHash -eq $RemoteHash) {
    Write-Log "[OK] El sistema ya esta en la ultima version ($($LocalHash.Substring(0,7))). No requiere recompilar."
    exit 0
}

# 4. Descargar cambios y reconstruir contenedor web sin tocar el volumen de base de datos
Write-Log "[ACTUALIZANDO] Nueva version detectada ($($LocalHash.Substring(0,7)) -> $($RemoteHash.Substring(0,7))). Descargando cambios..."
git pull origin main
if ($LASTEXITCODE -eq 0) {
    $Count = (git rev-list --count HEAD).Trim()
    $Short = (git rev-parse --short HEAD).Trim()
    $env:APP_VERSION = "2.0.$Count+$Short"
    Write-Log "[DOCKER] Reconstruyendo contenedor web (curia-web) en version $($env:APP_VERSION)..."
    docker compose up -d --build
    Write-Log "[OK] Actualizacion completada exitosamente a v$($env:APP_VERSION)."
} else {
    Write-Log "[ERROR] Fallo git pull. Revise si hay cambios locales sin confirmar."
}
