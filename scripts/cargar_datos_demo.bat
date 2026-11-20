@echo off
chcp 65001 > nul
cd /d "%~dp0.."
echo ============================================================================
echo  CARGAR DATOS SEMILLA DE DEMOSTRACIÓN / CAPACITACIÓN: ECLESOFT 2.0
echo ============================================================================
echo.

set "SEED_FILE=%~dp0..\AplicacionCuria\DatosSemilla.sql"

if not exist "%SEED_FILE%" (
    echo [ERROR] No se encontró el archivo AplicacionCuria\DatosSemilla.sql
    pause
    exit /b 1
)

echo Inyectando datos de prueba en la base de datos (curia-postgres)...
docker cp "%SEED_FILE%" curia-postgres:/tmp/DatosSemilla.sql
docker exec curia-postgres psql -U postgres -d curia -f /tmp/DatosSemilla.sql
docker exec curia-postgres rm -f /tmp/DatosSemilla.sql

echo.
echo [OK] Datos semilla cargados correctamente.
echo Recargue el navegador (F5) para explorar todos los módulos con datos de ejemplo.
echo (Al finalizar la demostración, ejecute limpiar_datos_demo.bat para dejar el sistema en cero).
echo.
pause
