using System.Security.Claims;
using AplicacionCuria.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace AplicacionCuria.Tests;

public class DebeCambiarClaveFilterTests
{
    private static ActionExecutingContext CreateContext(ClaimsPrincipal user, string requestPath)
    {
        var httpContext = new DefaultHttpContext
        {
            User = user
        };
        httpContext.Request.Path = requestPath;

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new object());
    }

    [Fact]
    public void OnActionExecuting_UserWithTemporaryPasswordOnProtectedPath_ShouldRedirectToCambiarClave()
    {
        var filter = new DebeCambiarClaveFilter();
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Admin"),
            new Claim("debe_cambiar_clave", "true")
        };
        var identity = new ClaimsIdentity(claims, "Cookies");
        var user = new ClaimsPrincipal(identity);
        var context = CreateContext(user, "/Home/Index");

        filter.OnActionExecuting(context);

        var redirect = Assert.IsType<RedirectResult>(context.Result);
        Assert.Equal("/Auth/CambiarClave", redirect.Url);
    }

    [Fact]
    public void OnActionExecuting_UserWithTemporaryPasswordOnCambiarClave_ShouldNotRedirect()
    {
        var filter = new DebeCambiarClaveFilter();
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Admin"),
            new Claim("debe_cambiar_clave", "true")
        };
        var identity = new ClaimsIdentity(claims, "Cookies");
        var user = new ClaimsPrincipal(identity);
        var context = CreateContext(user, "/Auth/CambiarClave");

        filter.OnActionExecuting(context);

        Assert.Null(context.Result);
    }

    [Fact]
    public void OnActionExecuting_NormalUserWithoutFlag_ShouldNotRedirect()
    {
        var filter = new DebeCambiarClaveFilter();
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "Cookies");
        var user = new ClaimsPrincipal(identity);
        var context = CreateContext(user, "/Home/Index");

        filter.OnActionExecuting(context);

        Assert.Null(context.Result);
    }
}
