using HubAdminPanel.Core.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Features.Roles.Queries
{
    public record GetRoleByIdQuery(int Id) : IRequest<RoleDto>;
}
