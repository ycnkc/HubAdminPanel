using HubAdminPanel.Core.DTOs;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Roles.Queries
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IAppDbContext _context; // Kendi DbContext ismin

        public GetRoleByIdQueryHandler(IAppDbContext context) => _context = context;

        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Permissions = r.RolePermissions.Select(rp => new HubAdminPanel.Core.DTOs.PermissionDto
                    {
                        Id = rp.PermissionId,
                        Name = rp.Permission.Key
                    }).ToList()
                })
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        }
    }
}
