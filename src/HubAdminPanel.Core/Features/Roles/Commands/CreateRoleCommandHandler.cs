using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using MediatR;

namespace HubAdminPanel.Core.Features.Roles.Commands
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, int>
    {
        private readonly IAppDbContext _context;
        public CreateRoleCommandHandler(IAppDbContext context) => _context = context;

        public async Task<int> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = new Role { Name = request.Name };
            _context.Roles.Add(role);

            await _context.SaveChangesAsync(cancellationToken);

            if (request.PermissionIds != null && request.PermissionIds.Any())
            {
                var rolePermissions = request.PermissionIds.Select(pId => new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = pId
                }).ToList();

                _context.RolePermissions.AddRange(rolePermissions);

                await _context.SaveChangesAsync(cancellationToken);
            }

            return role.Id;
        }
    }
}
