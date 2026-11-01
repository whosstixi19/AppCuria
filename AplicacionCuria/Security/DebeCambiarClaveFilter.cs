using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AplicacionCuria.Security
{
    /// <summary>
    /// Filtro global: si el usuario inició sesión con una contraseña temporal
    /// (la semilla del sistema o la clave inicial que le entregó el admin,
    /// marcada con la claim "debe_cambiar_clave"), se le obliga a definirla
    /// nueva antes de poder usar la aplicación.
    /// </summary>
    public class DebeCambiarClaveFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var usuario = context.HttpContext.User;

            if (usuario.Identity?.IsAuthenticated != true) return;
            if (!usuario.HasClaim("debe_cambiar_clave", "true")) return;

            // Solo se permite cambiar la clave (o cerrar sesión)
            var ruta = context.HttpContext.Request.Path;
            if (ruta.StartsWithSegments("/Auth/CambiarClave") || ruta.StartsWithSegments("/Auth/Logout")) return;

            context.Result = new RedirectResult("/Auth/CambiarClave");
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
