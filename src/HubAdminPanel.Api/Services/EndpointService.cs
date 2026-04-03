using HubAdminPanel.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

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
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type));

            foreach (var controller in controllers)
            {
                var controllerRoute = controller.GetCustomAttribute<RouteAttribute>()?.Template ?? "";

                var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(m => m.GetCustomAttributes<HttpMethodAttribute>().Any());

                foreach (var method in methods)
                {
                    var httpMethodAttr = method.GetCustomAttribute<HttpMethodAttribute>();
                    var methodRoute = method.GetCustomAttribute<RouteAttribute>()?.Template ?? "";

                    var fullPath = $"/{controllerRoute}/{methodRoute}".Replace("//", "/").TrimEnd('/');
                    var httpMethod = httpMethodAttr?.HttpMethods.First() ?? "GET";

                    var exists = _context.Endpoints.Any(e => e.Path == fullPath && e.Method == httpMethod);

                    if (!exists)
                    {
                        _context.Endpoints.Add(new Core.Entities.Endpoint
                        {
                            Path = fullPath,
                            Method = httpMethod,
                            Description = $"{controller.Name.Replace("Controller", "")} - {method.Name}"
                        });
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}