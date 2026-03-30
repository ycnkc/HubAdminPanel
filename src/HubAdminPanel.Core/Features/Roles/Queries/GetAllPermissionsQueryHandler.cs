using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Roles.Queries
{
    public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, List<PermissionDto>>
    {
        private readonly IAppDbContext _context;
        public GetAllPermissionsQueryHandler(IAppDbContext context) => _context = context;

        public async Task<List<PermissionDto>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Permissions
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Key = p.Key,
                    Description = p.Description
                })
                .ToListAsync(cancellationToken);
        }
    }
}
