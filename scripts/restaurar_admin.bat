@echo off
chcp 65001 > nul
cd /d "%~dp0.."
echo ============================================================================
echo  RESTAURAR ADMINISTRADOR SUPREMO: ECLESOFT 2.0
echo ============================================================================
echo.
echo Restaurando usuario Superadministrador en PostgreSQL...
echo.

docker exec curia-postgres psql -U postgres -d curia -c "INSERT INTO usuarios (correo, nombre, apellido, rol, estado, pass, salt, iterations, parroquia_id) VALUES ('admin@curia.com', 'Administrador', 'Curia', 'admin', 'ACTIVO', '65kDeHXTZytW/nl6iNieLWVt/Pub4KjYg9WBYuOjyUE=', 'AQIDBAUGBwgJCgsMDQ4PEA==', 310000, NULL) ON CONFLICT (correo) DO UPDATE SET rol = 'admin', estado = 'ACTIVO', parroquia_id = NULL, pass = '65kDeHXTZytW/nl6iNieLWVt/Pub4KjYg9WBYuOjyUE=', salt = 'AQIDBAUGBwgJCgsMDQ4PEA==', iterations = 310000; UPDATE usuarios SET parroquia_id = NULL WHERE rol = 'admin';"

if %errorlevel% neq 0 (
    echo.
    echo [ERROR] No se pudo conectar con el contenedor curia-postgres.
    echo Verifique que Docker esté iniciado (docker compose up -d).
    echo.
    pause
    exit /b 1
)

echo.
echo ============================================================================
echo  [LISTO] ADMINISTRADOR SUPREMO ACTIVO
echo ============================================================================
echo.
echo  Correo:     admin@curia.com
echo  Contraseña: Clave_curia
echo  Rol:        admin (Superadministrador - todas las parroquias)
echo.
echo Ya puede iniciar sesión en http://localhost:5155
echo ============================================================================
echo.
pause
