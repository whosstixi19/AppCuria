@echo off
chcp 65001 > nul
cd /d "%~dp0.."
echo ============================================================================
echo  CONFIGURACIÓN DE CORREO LOCAL (SMTP AGENDA): ECLESOFT 2.0
echo  (Se guarda en .env local, ignorado por Git para no perderse en git pull)
echo ============================================================================
echo.

set /p "SMTP_EMAIL=Ingrese el correo Gmail de la Parroquia (ej. parroquiasansebastian@gmail.com): "
set /p "SMTP_PASSWORD=Ingrese la Contraseña de Aplicación de 16 letras de Google: "

(
    echo SMTP_SERVER=smtp.gmail.com
    echo SMTP_PORT=587
    echo SMTP_EMAIL=%SMTP_EMAIL%
    echo SMTP_PASSWORD=%SMTP_PASSWORD%
) > ".env"

echo.
echo [OK] Credenciales guardadas en el archivo local .env (protegido contra git pull).
echo Aplicando configuración en el contenedor curia-web...
docker compose up -d
echo.
echo [LISTO] El contenedor web ya tiene configurado el correo de la parroquia.
pause
