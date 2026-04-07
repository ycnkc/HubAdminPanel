using HubAdminPanel.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace HubAdminPanel.Api.Middleware
{
    public class DynamicAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        private static readonly ConcurrentDictionary<string, Regex> _regexCache = new();

        public DynamicAuthorizationMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            var path = context.Request.Path;

            if (context.User.IsInRole("Admin"))
            {
                await _next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var method = context.Request.Method;
            var pathValue = path.Value!;

            var allEndpoints = await _cache.GetOrCreateAsync("AllEndpoints", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await dbContext.Endpoints
                    .Include(e => e.EndpointRoleMappings) 
                        .ThenInclude(m => m.Role)
                    .AsNoTracking()
                    .ToListAsync();
            });

            var matchedEndpoint = allEndpoints?.FirstOrDefault(e =>
                e.Method == method && IsPathMatch(e.Path, pathValue));

            if (matchedEndpoint == null)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { message = "Bu endpoint sisteme kayıtlı değil veya erişime kapalı." });
                return;
            }

            var requiredRoles = matchedEndpoint.EndpointRoleMappings
                .Select(m => m.Role.Name) 
                .ToList();

            if (!requiredRoles.Any())
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { message = "Bu işlem için herhangi bir rol tanımlanmamış!" });
                return;
            }

            var userRoles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            bool isAuthorized = requiredRoles.Intersect(userRoles).Any();

            if (isAuthorized)
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
            var regex = _regexCache.GetOrAdd(template, t =>
            {
                var escaped = Regex.Escape(t);

                var pattern = "^" + Regex.Replace(escaped, "\\{[^/]+\\}", "[^/]+") + "$";
                return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            });

            return regex.IsMatch(actualPath);
        }
    }
}