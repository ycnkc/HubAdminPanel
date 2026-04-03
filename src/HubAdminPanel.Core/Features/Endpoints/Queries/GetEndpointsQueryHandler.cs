using HubAdminPanel.Core.DTOs;
using HubAdminPanel.Core.Features.Management.Queries;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Management.Queries
{
    public class GetEndpointsQueryHandler : IRequestHandler<GetEndpointsQuery, List<EndpointDto>>
    {
        private readonly IAppDbContext _context;

        public GetEndpointsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EndpointDto>> Handle(GetEndpointsQuery request, CancellationToken cancellationToken)
        {
            var endpoints = await _context.Endpoints
                .Include(e => e.EndpointRoleMappings)
                    .ThenInclude(m => m.Role)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return endpoints.Select(e => new EndpointDto
            {
                Id = e.Id,
                Path = e.Path,
                Method = e.Method,
                Description = e.Description,
                EndpointRoleMappings = e.EndpointRoleMappings?.Select(m => new EndpointRoleMappingDto
                {
                    RoleId = m.RoleId,
                    Role = new RoleDto { Name = m.Role?.Name ?? "Bilinmiyor" }
                }).ToList() ?? new List<EndpointRoleMappingDto>()
            }).ToList();
        }
    }
}