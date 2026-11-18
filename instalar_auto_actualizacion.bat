@echo off
chcp 65001 > nul
echo ============================================================================
echo  CONFIGURACIÓN DE AUTO-ACTUALIZACIÓN: ECLESOFT 2.0 (AplicacionCuria)
echo ============================================================================

set "SCRIPT_PATH=%~dp0auto_actualizar_curia.ps1"

if not exist "%SCRIPT_PATH%" (
    echo [ERROR] No se encontró el archivo auto_actualizar_curia.ps1 en esta carpeta.
    pause
    exit /b 1
)

echo Registrando tarea programada al iniciar sesión en Windows...
schtasks /Create /TN "AplicacionCuria_AutoUpdate_Logon" /TR "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"%SCRIPT_PATH%\"" /SC ONLOGON /F

echo Registrando verificación diaria automática a las 08:30 AM...
schtasks /Create /TN "AplicacionCuria_AutoUpdate_Diario" /TR "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"%SCRIPT_PATH%\"" /SC DAILY /ST 08:30 /F

echo.
echo [OK] Las tareas quedaron registradas correctamente.
echo El sistema verificará y aplicará actualizaciones automáticamente al iniciar sesión y cada día a las 08:30 AM.
pause
