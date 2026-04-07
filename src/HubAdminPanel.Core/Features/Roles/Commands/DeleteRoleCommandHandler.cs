using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HubAdminPanel.Core.Features.Roles.Commands
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly IAppDbContext _context;
        private readonly IMemoryCache _cache;

        public DeleteRoleCommandHandler(IAppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.Roles
                .Include(r => r.EndpointRoleMappings)
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (role == null) return false;

            if (role.EndpointRoleMappings.Any())
            {
                _context.EndpointRoleMappings.RemoveRange(role.EndpointRoleMappings);
            }

            _context.Roles.Remove(role);

            await _context.SaveChangesAsync(cancellationToken);
            _cache.Remove("AllEndpoints");
            return true;
        }
    }
}