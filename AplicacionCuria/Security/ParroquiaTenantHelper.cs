using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AplicacionCuria.Security
{
    /// <summary>
    /// Helper centralizado para aislamiento multi-parroquia (multi-tenancy) y resolución de contexto activo.
    /// </summary>
    public static class ParroquiaTenantHelper
    {
        public const string ClaimParroquiaId = "parroquia_id";
        public const string CookieSuperAdminParroquia = "FiltroParroquiaSuperAdmin";

        /// <summary>
        /// Determina si el usuario autenticado tiene perfil de Superadministrador (Curia Central / sin parroquia fija).
        /// </summary>
        public static bool EsSuperAdmin(ClaimsPrincipal? user)
        {
            if (user?.Identity?.IsAuthenticated != true) return false;
            return user.IsInRole("admin") && user.FindFirst(ClaimParroquiaId) == null;
        }

        /// <summary>
        /// Obtiene el ID de la parroquia asignada fijamente al usuario (si aplica).
        /// </summary>
        public static int? ObtenerParroquiaAsignada(ClaimsPrincipal? user)
        {
            if (user?.Identity?.IsAuthenticated != true) return null;
            var claim = user.FindFirst(ClaimParroquiaId);
            if (claim != null && int.TryParse(claim.Value, out var pid) && pid > 0)
            {
                return pid;
            }
            return null;
        }

        /// <summary>
        /// Obtiene la parroquia activa para filtrado de datos:
        /// 1. Si es usuario de parroquia: su parroquia fija.
        /// 2. Si es superadmin: la parroquia seleccionada en la cookie del navbar (o null para 'Todas').
        /// </summary>
        public static int? ObtenerParroquiaActivaId(ClaimsPrincipal? user, HttpContext? context)
        {
            if (user?.Identity?.IsAuthenticated != true) return null;

            // 1. Si el usuario pertenece a una parroquia específica, su contexto es inmutable
            var parroquiaFija = ObtenerParroquiaAsignada(user);
            if (parroquiaFija.HasValue)
            {
                return parroquiaFija.Value;
            }

            // 2. Si es superadmin, revisar si seleccionó una parroquia específica en su sesión/cookie
            if (context != null && context.Request.Cookies.TryGetValue(CookieSuperAdminParroquia, out var cookieVal)
                && int.TryParse(cookieVal, out var spid) && spid > 0)
            {
                return spid;
            }

            // Superadmin con vista global (Todas las parroquias)
            return null;
        }
    }
}
