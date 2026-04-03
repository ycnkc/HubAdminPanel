using HubAdminPanel.Core.DTOs;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Roles.Queries
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
    {
        private readonly IAppDbContext _context;
        public GetAllRolesQueryHandler(IAppDbContext context) => _context = context;

        public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var rolesFromDb = await _context.Roles
                .Include(r => r.EndpointRoleMappings)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var result = rolesFromDb.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name ?? "İsimsiz",
                EndpointRoleMappings = r.EndpointRoleMappings?
                    .Select(m => new EndpointRoleMappingDto
                    {
                        EndpointId = m.EndpointId,
                        RoleId = m.RoleId
                    }).ToList() ?? new List<EndpointRoleMappingDto>()
            }).ToList();

            return result;
        }
    }
}