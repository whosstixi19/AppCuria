@echo off
chcp 65001 > nul

:: Solicitar permisos de Administrador automáticamente si se abrió con doble clic normal
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Solicitando permisos de Administrador...
    powershell -NoProfile -Command "Start-Process -FilePath '%~f0' -Verb RunAs"
    exit /b
)

cd /d "%~dp0"
echo ============================================================================
echo  CONFIGURACIÓN DE RESPALDOS AUTOMÁTICOS (LOCAL + GOOGLE DRIVE): ECLESOFT 2.0
echo ============================================================================
echo.

set "TEMPLATE_PATH=%~dp0backup_curia.ps1"
set "LOCAL_SCRIPT_PATH=%~dp0backup_curia.local.ps1"

if not exist "%TEMPLATE_PATH%" (
    echo [ERROR] No se encontró el archivo plantilla backup_curia.ps1 en esta carpeta.
    pause
    exit /b 1
)

if not exist "%LOCAL_SCRIPT_PATH%" (
    echo Creando copia local estática backup_curia.local.ps1 ^(protegida contra git pull^)...
    copy /Y "%TEMPLATE_PATH%" "%LOCAL_SCRIPT_PATH%" > nul
)

set /p "PARROQUIA=Ingrese el nombre de la Parroquia (sin espacios, ej. Parroquia_Cristo_Salvador): "
if "%PARROQUIA%"=="" set "PARROQUIA=Parroquia_Principal"

echo.
echo [1/3] Registrando respaldos diarios (10:00, 14:00 y 18:00)...
schtasks /Create /TN "AplicacionCuria_Backup_1000" /TR "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"%LOCAL_SCRIPT_PATH%\" -Parroquia \"%PARROQUIA%\" -Tipo diario" /SC DAILY /ST 10:00 /F
schtasks /Create /TN "AplicacionCuria_Backup_1400" /TR "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"%LOCAL_SCRIPT_PATH%\" -Parroquia \"%PARROQUIA%\" -Tipo diario" /SC DAILY /ST 14:00 /F
schtasks /Create /TN "AplicacionCuria_Backup_1800" /TR "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"%LOCAL_SCRIPT_PATH%\" -Parroquia \"%PARROQUIA%\" -Tipo diario" /SC DAILY /ST 18:00 /F

echo.
echo [2/3] Registrando respaldo semanal consolidado (Domingos 23:00)...
schtasks /Create /TN "AplicacionCuria_Backup_Semanal" /TR "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"%LOCAL_SCRIPT_PATH%\" -Parroquia \"%PARROQUIA%\" -Tipo semanal" /SC WEEKLY /D SUN /ST 23:00 /F

echo.
echo [3/3] Ejecutando primer respaldo de verificación ahora mismo...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%LOCAL_SCRIPT_PATH%" -Parroquia "%PARROQUIA%" -Tipo diario

echo.
echo [OK] Los respaldos automáticos para "%PARROQUIA%" quedaron configurados usando backup_curia.local.ps1.
echo (Si necesita personalizar rutas manualmente en esta PC, edite backup_curia.local.ps1).
pause
