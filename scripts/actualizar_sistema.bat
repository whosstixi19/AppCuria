@echo off
chcp 65001 > nul
cd /d "%~dp0"

echo ============================================================================
echo   ACTUALIZADOR: ECLESOFT 2.0 (AplicacionCuria)
echo ============================================================================
echo.
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0auto_actualizar_curia.ps1" -ModoManual
echo.
echo Presione cualquier tecla para cerrar esta ventana...
pause > nul
