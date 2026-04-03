using HubAdminPanel.Core.DTOs;
using MediatR;

namespace HubAdminPanel.Core.Features.Roles.Queries
{
    public class GetAllRolesQuery : IRequest<List<RoleDto>>
    {

    }
}
