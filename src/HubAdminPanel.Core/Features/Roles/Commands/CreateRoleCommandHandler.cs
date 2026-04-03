using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace HubAdminPanel.Core.Features.Roles.Commands
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, int>
    {
        private readonly IAppDbContext _context;
        private readonly IMemoryCache _cache; 

        public CreateRoleCommandHandler(IAppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<int> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = new Role { Name = request.Name };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync(cancellationToken);

            if (request.EndpointIds != null && request.EndpointIds.Any())
            {
                var mappings = request.EndpointIds.Select(epId => new EndpointRoleMapping
                {
                    RoleId = role.Id,
                    EndpointId = epId
                });

                await _context.EndpointRoleMappings.AddRangeAsync(mappings, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            _cache.Remove("AllEndpoints");

            return role.Id;
        }
    }
}