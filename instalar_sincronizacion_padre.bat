@echo off
chcp 65001 > nul

:: Solicitar permisos de Administrador automáticamente
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Solicitando permisos de Administrador...
    powershell -NoProfile -Command "Start-Process -FilePath '%~f0' -Verb RunAs"
    exit /b
)

cd /d "%~dp0"
echo ============================================================================
echo  CONFIGURACIÓN AUTOMÁTICA PARA LAPTOP DEL PADRE (SINCRONIZACIÓN CON SECRETARÍA)
echo ============================================================================
echo.

set "TEMPLATE_SYNC=%~dp0sincronizar_padre.ps1"
set "LOCAL_SYNC=%~dp0sincronizar_padre.local.ps1"
set "TEMPLATE_BACKUP=%~dp0backup_curia.ps1"
set "LOCAL_BACKUP=%~dp0backup_curia.local.ps1"

if not exist "%LOCAL_BACKUP%" (
    copy /Y "%TEMPLATE_BACKUP%" "%LOCAL_BACKUP%" > nul
)
if not exist "%LOCAL_SYNC%" (
    copy /Y "%TEMPLATE_SYNC%" "%LOCAL_SYNC%" > nul
)

set /p "CARPETA_SEC=Nombre de la carpeta de la SECRETARIA en Google Drive (ej. Cristo_Salvador_Secretaria o Secretaria): "
if "%CARPETA_SEC%"=="" set "CARPETA_SEC=Secretaria"

set /p "CARPETA_PADRE=Nombre de la carpeta del PADRE en Google Drive (ej. Cristo_Salvador_Padre o Padre): "
if "%CARPETA_PADRE%"=="" set "CARPETA_PADRE=Padre"

echo.
echo [1/3] Registrando sincronización silenciosa al iniciar sesión en la laptop del Padre...
set "STARTUP_DIR=%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup"
set "VBS_SYNC=%STARTUP_DIR%\AplicacionCuria_SyncPadre.vbs"
(
    echo Set WshShell = CreateObject^("WScript.Shell"^)
    echo WshShell.Run "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File ""%LOCAL_SYNC%"" -CarpetaSecretaria ""%CARPETA_SEC%"" -CarpetaPadre ""%CARPETA_PADRE%""", 0, False
) > "%VBS_SYNC%"

echo [2/3] Registrando sincronización automática 3 veces al día (10:15, 14:15 y 18:15)...
schtasks /Create /TN "AplicacionCuria_SyncPadre_1015" /TR "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"%LOCAL_SYNC%\" -CarpetaSecretaria \"%CARPETA_SEC%\" -CarpetaPadre \"%CARPETA_PADRE%\"" /SC DAILY /ST 10:15 /F
schtasks /Create /TN "AplicacionCuria_SyncPadre_1415" /TR "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"%LOCAL_SYNC%\" -CarpetaSecretaria \"%CARPETA_SEC%\" -CarpetaPadre \"%CARPETA_PADRE%\"" /SC DAILY /ST 14:15 /F
schtasks /Create /TN "AplicacionCuria_SyncPadre_1815" /TR "powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"%LOCAL_SYNC%\" -CarpetaSecretaria \"%CARPETA_SEC%\" -CarpetaPadre \"%CARPETA_PADRE%\"" /SC DAILY /ST 18:15 /F

echo.
echo [3/3] Ejecutando primera sincronización de prueba ahora mismo...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%LOCAL_SYNC%" -CarpetaSecretaria "%CARPETA_SEC%" -CarpetaPadre "%CARPETA_PADRE%"

echo.
echo [LISTO] La laptop del Padre extraerá automáticamente los datos de "%CARPETA_SEC%"
echo y guardará su propio respaldo en "%CARPETA_PADRE%" en segundo plano.
pause
