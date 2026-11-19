@echo off
chcp 65001 > nul

:: Solicitar permisos de Administrador automáticamente
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Solicitando permisos de Administrador...
    powershell -NoProfile -Command "Start-Process -FilePath '%~f0' -Verb RunAs"
    exit /b
)

echo ============================================================================
echo  CONFIGURACIÓN DE RED LOCAL Y DOMINIO INTERNO (LAN): ECLESOFT 2.0
echo ============================================================================
echo.

echo [1/2] Abriendo puertos 80 (HTTP directo) y 5155 en el Firewall de Windows...
powershell -NoProfile -Command "New-NetFirewallRule -DisplayName 'AplicacionCuria HTTP 80' -Direction Inbound -LocalPort 80 -Protocol TCP -Action Allow -ErrorAction SilentlyContinue | Out-Null"
powershell -NoProfile -Command "New-NetFirewallRule -DisplayName 'AplicacionCuria 5155' -Direction Inbound -LocalPort 5155 -Protocol TCP -Action Allow -ErrorAction SilentlyContinue | Out-Null"
echo [OK] Puertos 80 y 5155 habilitados en el Firewall.
echo.

for /f "delims=" %%i in ('hostname') do set "CURRENT_HOST=%%i"
echo El nombre actual de esta computadora en la red es: %CURRENT_HOST%
echo Con este nombre, los demas equipos pueden entrar a: http://%CURRENT_HOST%
echo.

set /p "NEW_HOST=Si desea cambiarlo (ej. CRISTOSALVADOR), escribalo aqui (o presione ENTER para mantener %CURRENT_HOST%): "

if not "%NEW_HOST%"=="" (
    echo.
    echo [2/2] Cambiando el nombre del equipo a %NEW_HOST%...
    powershell -NoProfile -Command "Rename-Computer -NewName '%NEW_HOST%' -Force"
    echo.
    echo [OK] Nombre cambiado a %NEW_HOST%.
    echo IMPORTANTE: Reinicie la computadora para que todas las PCs de la red puedan entrar escribiendo:
    echo   http://%NEW_HOST%
) else (
    echo.
    echo [LISTO] Cualquier computadora o dispositivo conectado al mismo Wi-Fi/Red de la parroquia puede entrar a:
    echo   http://%CURRENT_HOST%
    echo   o tambien: http://%CURRENT_HOST%:5155
)

echo.
pause
