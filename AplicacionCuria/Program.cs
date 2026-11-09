using AplicacionCuria.Conexion;
using AplicacionCuria.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Cargar configuración adicional para entornos
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Servicios principales
// Filtro global anti-CSRF: valida el token en todas las peticiones POST/PUT/DELETE/PATCH
builder.Services
    .AddControllersWithViews(options =>
    {
        options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());

        // Contraseña temporal del primer login: obligar a cambiarla
        options.Filters.Add<AplicacionCuria.Security.DebeCambiarClaveFilter>();
    });
builder.Services.AddAntiforgery();

// Base de datos PostgreSQL
builder.Services.AddDbContext<ConexionPostgreSQL>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de Email
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);
builder.Services.AddSingleton<EmailService>();

// Notificaciones: servicio de recordatorios (controlador + proceso de fondo)
builder.Services.AddScoped<RecordatorioService>();

// BackgroundService
builder.Services.AddHostedService<RecordatorioBackgroundService>();

// Autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Migración automática idempotente para entornos existentes (evita errores 500 si la BD ya existía)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ConexionPostgreSQL>();
        db.Database.ExecuteSqlRaw(@"
            ALTER TABLE IF EXISTS parroquias ADD COLUMN IF NOT EXISTS logo_base64 TEXT;
            ALTER TABLE IF EXISTS usuarios ADD COLUMN IF NOT EXISTS parroquia_id INT;
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 FROM pg_constraint WHERE conname = 'fk_usuarios_parroquia'
                ) THEN
                    ALTER TABLE usuarios 
                    ADD CONSTRAINT fk_usuarios_parroquia 
                    FOREIGN KEY (parroquia_id) REFERENCES parroquias(id) ON DELETE SET NULL;
                END IF;
            END $$;
        ");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "No se pudo aplicar la migración automática del esquema.");
    }
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Configuración de Rotativa (compatible con Windows y Linux/Docker)
IWebHostEnvironment env = app.Environment;
if (OperatingSystem.IsWindows())
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "../Rotativa/Windows");
}
else
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("/usr", "bin");
}

app.Run();
