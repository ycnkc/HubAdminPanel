using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Features.Roles.Queries
{
    public class GetAllPermissionsQuery : IRequest<List<PermissionDto>> { }

    public class PermissionDto
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public string Name { get; internal set; }
    }
}
