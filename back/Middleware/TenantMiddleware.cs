using System.Security.Claims;
using VentifyAPI.Services;

namespace VentifyAPI.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly string[] PublicPaths = new[]
        {
            "/",
            "/api/auth/login",
            "/api/auth/register",
            "/swagger",
            "/swagger/index.html",
            "/swagger/v1/swagger.json",
            "/health"
        };

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext ctx, ITenantContext tenant)
        {
            if (ctx.User?.Identity?.IsAuthenticated == true)
            {
                var negocioClaim = ctx.User.FindFirst("negocioId")?.Value;
                if (int.TryParse(negocioClaim, out var nId)) tenant.NegocioId = nId;

                var userClaim = ctx.User.FindFirst("userId")?.Value
                    ?? ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userClaim, out var uId)) tenant.UserId = uId;

                tenant.Rol = ctx.User.FindFirst("rol")?.Value
                    ?? ctx.User.FindFirst(ClaimTypes.Role)?.Value;
            }

            var path = ctx.Request.Path.HasValue ? ctx.Request.Path.Value! : string.Empty;
            var isPublic = PublicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));

            if (!isPublic && tenant.NegocioId == null)
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await ctx.Response.WriteAsync("Falta claim negocioId");
                return;
            }

            await _next(ctx);
        }
    }
}
