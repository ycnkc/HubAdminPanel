using HubAdminPanel.Core.DTOs;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Users.Queries
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
    {
        private readonly IAppDbContext _context;
        public GetAllRolesQueryHandler(IAppDbContext context) => _context = context;

        public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {

            return await _context.Roles
                .Select(r => new RoleDto { Id = r.Id, Name = r.Name })
                .ToListAsync(cancellationToken);
        }
    }
}
