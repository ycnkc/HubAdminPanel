using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HubAdminPanel.Core.Features.Roles.Commands
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly IAppDbContext _context;
        private readonly IMemoryCache _cache; 

        public UpdateRoleCommandHandler(IAppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.Roles
                .Include(r => r.EndpointRoleMappings)
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (role == null) return false;

            role.Name = request.Name;

            if (role.EndpointRoleMappings.Any())
            {
                _context.EndpointRoleMappings.RemoveRange(role.EndpointRoleMappings);
            }

            if (request.EndpointIds != null && request.EndpointIds.Any())
            {
                var newMappings = request.EndpointIds.Select(epId => new EndpointRoleMapping
                {
                    RoleId = role.Id,
                    EndpointId = epId
                }).ToList();

                _context.EndpointRoleMappings.AddRange(newMappings);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _cache.Remove("AllEndpoints");

            return true;
        }
    }
}