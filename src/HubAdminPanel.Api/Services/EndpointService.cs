using HubAdminPanel.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Api.Services
{
    public class EndpointService
    {
        private readonly AppDbContext _context;

        public EndpointService(AppDbContext context)
        {
            _context = context;
        }

        public async Task DiscoverEndpointsAsync()
        {
            var controllers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract);

            foreach (var controller in controllers)
            {
                var controllerName = controller.Name.Replace("Controller", "");
                var controllerRouteAttr = controller.GetCustomAttribute<RouteAttribute>();
                var controllerRoute = controllerRouteAttr?.Template ?? "";

                controllerRoute = controllerRoute.Replace("[controller]", controllerName);

                var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(m => m.GetCustomAttributes<HttpMethodAttribute>().Any());

                foreach (var method in methods)
                {
                    var httpMethodAttr = method.GetCustomAttribute<HttpMethodAttribute>();
                    var methodRoute = httpMethodAttr?.Template ?? method.GetCustomAttribute<RouteAttribute>()?.Template ?? "";

                    methodRoute = methodRoute.Replace("[action]", method.Name);

                    var combinedPath = (controllerRoute + "/" + methodRoute).Replace("//", "/").Trim('/');
                    var fullPath = $"/{combinedPath}".ToLowerInvariant();
                    var httpMethod = httpMethodAttr?.HttpMethods.FirstOrDefault()?.ToUpperInvariant() ?? "GET";

                    var existingEndpoint = await _context.Endpoints
                        .FirstOrDefaultAsync(e => e.Path.ToLower() == fullPath && e.Method.ToUpper() == httpMethod);

                    if (existingEndpoint == null)
                    {
                        _context.Endpoints.Add(new Core.Entities.Endpoint
                        {
                            Path = fullPath,
                            Method = httpMethod,
                            ControllerName = controllerName,
                            Description = $"{controllerName} - {method.Name}"
                        });
                    }
                    else
                    {
                        existingEndpoint.ControllerName = controllerName;
                        existingEndpoint.Description = $"{controllerName} - {method.Name}";
                        existingEndpoint.Path = fullPath;
                        existingEndpoint.Method = httpMethod;

                        _context.Endpoints.Update(existingEndpoint);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}