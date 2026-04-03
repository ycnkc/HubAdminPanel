using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Roles.Commands
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly IAppDbContext _context;

        public DeleteRoleCommandHandler(IAppDbContext context)
        {
            _context = context;
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
            return true;
        }
    }
}