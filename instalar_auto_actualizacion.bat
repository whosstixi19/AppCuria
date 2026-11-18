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
echo  CONFIGURACIÓN DE AUTO-ACTUALIZACIÓN: ECLESOFT 2.0 (AplicacionCuria)
echo ============================================================================

set "SCRIPT_PATH=%~dp0auto_actualizar_curia.ps1"

if not exist "%SCRIPT_PATH%" (
    echo [ERROR] No se encontró el archivo auto_actualizar_curia.ps1 en esta carpeta.
    pause
    exit /b 1
)

echo.
echo [1/3] Registrando inicio automático en la carpeta Startup del usuario...
set "STARTUP_DIR=%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup"
set "VBS_LAUNCHER=%STARTUP_DIR%\AplicacionCuria_AutoUpdate.vbs"
(
    echo Set WshShell = CreateObject^("WScript.Shell"^)
    echo WshShell.Run "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File ""%SCRIPT_PATH%""", 0, False
) > "%VBS_LAUNCHER%"

echo [2/3] Registrando verificación diaria automática a las 08:30 AM...
schtasks /Create /TN "AplicacionCuria_AutoUpdate_Diario" /TR "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"%SCRIPT_PATH%\"" /SC DAILY /ST 08:30 /F

echo.
echo [3/3] Ejecutando verificación de actualización ahora mismo...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_PATH%"

echo.
echo [OK] Auto-actualización configurada correctamente (al iniciar sesión y diario 08:30 AM).
pause
