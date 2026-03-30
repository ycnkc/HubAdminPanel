using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Features.Roles.Commands
{
    public class CreateRoleCommand : IRequest<int>
    {
        public string Name { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>(); // Seçilen yetkiler
    }
}
