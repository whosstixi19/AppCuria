using System.Security.Claims;
using AplicacionCuria.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace AplicacionCuria.Tests
{
    public class ParroquiaTenantHelperTests
    {
        [Fact]
        public void EsSuperAdmin_UserIsAdminWithoutParroquia_ReturnsTrue()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Superadmin"),
                new Claim(ClaimTypes.Role, "admin")
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies"));

            var result = ParroquiaTenantHelper.EsSuperAdmin(user);

            Assert.True(result);
        }

        [Fact]
        public void EsSuperAdmin_UserIsAdminWithParroquia_ReturnsFalse()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Parroco"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ParroquiaTenantHelper.ClaimParroquiaId, "5")
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies"));

            var result = ParroquiaTenantHelper.EsSuperAdmin(user);

            Assert.False(result);
        }

        [Fact]
        public void ObtenerParroquiaAsignada_WhenClaimExists_ReturnsCorrectId()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Secretaria"),
                new Claim(ParroquiaTenantHelper.ClaimParroquiaId, "3")
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies"));

            var result = ParroquiaTenantHelper.ObtenerParroquiaAsignada(user);

            Assert.Equal(3, result);
        }

        [Fact]
        public void ObtenerParroquiaActivaId_UserWithFixedParroquia_IgnoresCookieAndReturnsFixed()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Secretaria"),
                new Claim(ParroquiaTenantHelper.ClaimParroquiaId, "3")
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies"));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Cookie"] = $"{ParroquiaTenantHelper.CookieSuperAdminParroquia}=9";

            var result = ParroquiaTenantHelper.ObtenerParroquiaActivaId(user, httpContext);

            Assert.Equal(3, result);
        }

        [Fact]
        public void ObtenerParroquiaActivaId_SuperAdminWithCookie_ReturnsCookieId()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Superadmin"),
                new Claim(ClaimTypes.Role, "admin")
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies"));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Cookie"] = $"{ParroquiaTenantHelper.CookieSuperAdminParroquia}=7";

            var result = ParroquiaTenantHelper.ObtenerParroquiaActivaId(user, httpContext);

            Assert.Equal(7, result);
        }

        [Fact]
        public void ObtenerParroquiaActivaId_SuperAdminWithoutCookie_ReturnsNullForAll()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Superadmin"),
                new Claim(ClaimTypes.Role, "admin")
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies"));

            var httpContext = new DefaultHttpContext();

            var result = ParroquiaTenantHelper.ObtenerParroquiaActivaId(user, httpContext);

            Assert.Null(result);
        }
    }
}
