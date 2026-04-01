using HubAdminPanel.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace HubAdminPanel.Api.Middleware
{
    public class DynamicAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public DynamicAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            if (context.Request.Path.StartsWithSegments("/api/Roles") || context.Request.Path.StartsWithSegments("/api/Auth"))
            {
                await _next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var path = context.Request.Path.Value;
            var method = context.Request.Method;
            var userId = int.Parse(context.User.FindFirst("userId")?.Value ?? "0");

            var allEndpoints = await dbContext.Endpoints.ToListAsync();
            var matchedEndpoint = allEndpoints.FirstOrDefault(e =>
                e.Method == method && IsPathMatch(e.Path, path));

            if (matchedEndpoint == null)
            {
                await _next(context);
                return;
            }

            var userRoles = context.User.FindAll(System.Security.Claims.ClaimTypes.Role)
                            .Select(r => r.Value).ToList();

            if (userRoles.Contains("Admin"))
            {
                await _next(context);
                return;
            }

            var hasDirectAccess = await dbContext.EndpointUsers
                .AnyAsync(x => x.UserId == userId && x.EndpointId == matchedEndpoint.Id);

            if (hasDirectAccess)
            {
                await _next(context);
                return;
            }

            var requiredPermissions = await dbContext.EndpointPermissionMappings
                .Where(m => m.EndpointId == matchedEndpoint.Id)
                .Select(m => m.Permission.Key)
                .ToListAsync();

            if (!requiredPermissions.Any())
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { message = "Bu işlem için herhangi bir yetki tanımlanmamış!" });
                return;
            }

            var rolePermissions = context.User.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value);

            var extraPermissions = await dbContext.UserExtraPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.Permission.Key)
                .ToListAsync();

            var allUserPermissions = rolePermissions.Concat(extraPermissions).Distinct();

            if (requiredPermissions.Any(rp => allUserPermissions.Contains(rp)))
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { message = "Bu işlem için yetkiniz yok!" });
            }
        }

        private bool IsPathMatch(string template, string actualPath)
        {
            var pattern = "^" + Regex.Replace(template, "{[a-zA-Z0-9]+}", "[a-zA-Z0-9-]+") + "$";
            return Regex.IsMatch(actualPath, pattern, RegexOptions.IgnoreCase);
        }
    }
}