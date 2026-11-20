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
echo  CONFIGURACIÓN DE AUTO-ACTUALIZACIÓN INTELIGENTE: ECLESOFT 2.0
echo ============================================================================

set "SCRIPT_PATH=%~dp0auto_actualizar_curia.ps1"

if not exist "%SCRIPT_PATH%" (
    echo [ERROR] No se encontró el archivo auto_actualizar_curia.ps1 en esta carpeta.
    pause
    exit /b 1
)

echo.
echo [1/3] Eliminando tareas anteriores con horario fijo (08:30 AM)...
schtasks /Delete /TN "AplicacionCuria_AutoUpdate_Diario" /F >nul 2>&1
set "STARTUP_DIR=%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup"
if exist "%STARTUP_DIR%\AplicacionCuria_AutoUpdate.vbs" (
    del /f /q "%STARTUP_DIR%\AplicacionCuria_AutoUpdate.vbs" >nul 2>&1
)

echo [2/3] Registrando tarea al iniciar sesión con espera de 3 minutos (computadora tranquila)...
schtasks /Create /TN "AplicacionCuria_AutoUpdate_Inicio" /TR "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"%SCRIPT_PATH%\"" /SC ONLOGON /DELAY 0003:00 /F

echo [3/3] Creando acceso directo en el Escritorio: "Actualizar ECLESOFT"...
copy /y "%~dp0actualizar_sistema.bat" "%USERPROFILE%\Desktop\Actualizar ECLESOFT.bat" >nul

echo.
echo ============================================================================
echo [OK] Auto-actualización configurada con éxito:
echo  1. Al encender la PC: Se actualizará sola 3 minutos después (en segundo plano).
echo  2. En el Escritorio: Tienes el archivo "Actualizar ECLESOFT.bat" para cuando
echo     quieras actualizar manualmente con un solo clic.
echo ============================================================================
pause
