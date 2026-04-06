using HubAdminPanel.Core.DTOs;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq; 

namespace HubAdminPanel.Core.Features.Roles.Queries
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IAppDbContext _context;

        public GetRoleByIdQueryHandler(IAppDbContext context) => _context = context;

        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Roles
                .Where(r => r.Id == request.Id)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,

                    EndpointRoleMappings = r.EndpointRoleMappings.Select(erm => new EndpointRoleMappingDto
                    {
                        EndpointId = erm.EndpointId,
                        RoleId = erm.RoleId
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}