@echo off
chcp 65001 > nul
cd /d "%~dp0"
echo ============================================================================
echo  LIMPIAR DATOS DE DEMOSTRACIÓN Y DEJAR SISTEMA EN CERO: ECLESOFT 2.0
echo ============================================================================
echo.
echo ATENCIÓN: Esta acción borrará todos los registros de demostración y prueba,
echo reiniciará los contadores de ID a 1 y dejará únicamente al Superadmin
echo (admin@curia.com) para iniciar la carga de datos reales de la parroquia.
echo.

set /p "CONFIRM=Escriba SI (en mayúsculas) para confirmar la limpieza total: "
if not "%CONFIRM%"=="SI" (
    echo Operación cancelada.
    pause
    exit /b 0
)

echo.
echo Limpiando todas las tablas y reiniciando contadores a 1...
docker exec curia-postgres psql -U postgres -d curia -c "BEGIN; TRUNCATE TABLE pagos, agendas, personas_evento, confirmaciones, primerascomuniones, faltas, alumnoscatequesis, periodos, catequistas, matrimoniosantiguos, actasdematrimonio, actasprematrimoniales, cursillosprematrimoniales, bautismosantiguos, actasbautismo, cursillosprebautismales, feligreses, barrios, misas, iglesias, parroquias, ministros, sacerdotes RESTART IDENTITY CASCADE; DELETE FROM usuarios WHERE rol <> 'admin' AND correo <> 'admin@curia.com'; UPDATE usuarios SET parroquia_id = NULL WHERE rol = 'admin'; INSERT INTO usuarios (correo, nombre, apellido, rol, estado, pass, salt, iterations, parroquia_id) SELECT 'admin@curia.com', 'Administrador', 'Curia', 'admin', 'ACTIVO', '65kDeHXTZytW/nl6iNieLWVt/Pub4KjYg9WBYuOjyUE=', 'AQIDBAUGBwgJCgsMDQ4PEA==', 310000, NULL WHERE NOT EXISTS (SELECT 1 FROM usuarios WHERE rol = 'admin'); COMMIT;"

echo.
echo [LISTO] La base de datos quedó completamente limpia (en cero) y lista para producción.
echo.
pause
