using HubAdminPanel.Core.Common;
using HubAdminPanel.Core.DTOs;
using Mapster;
using MediatR;
using HubAdminPanel.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Users.Queries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagedResult<UserDto>>
    {
        private readonly IAppDbContext _context;

        public GetAllUsersQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                query = query.Where(u => u.Username.ToLower().Contains(term) ||
                                         u.Email.ToLower().Contains(term));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            if (request.RoleId.HasValue)
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == request.RoleId.Value));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var activeCount = await query.CountAsync(u => u.IsActive, cancellationToken);
            var adminCount = await query.CountAsync(u => u.UserRoles.Any(ur => ur.Role.Name == "Admin"), cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var roleCounts = await _context.UserRoles
                .GroupBy(ur => ur.Role.Name)
                .Select(g => new { RoleName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RoleName, x => x.Count, cancellationToken);

            var dtos = items.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                IsActive = u.IsActive,
                RoleId = u.UserRoles.Select(ur => ur.RoleId).FirstOrDefault(),
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            }).ToList();

            return new PagedResult<UserDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                ActiveCount = activeCount,
                AdminCount = adminCount,
                RoleCounts = roleCounts
            };
        }
    }
}