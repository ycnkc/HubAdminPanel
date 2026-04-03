using HubAdminPanel.Core.DTOs;
using MediatR;

namespace HubAdminPanel.Core.Features.Roles.Queries
{
    public record GetRoleByIdQuery(int Id) : IRequest<RoleDto>;
}
