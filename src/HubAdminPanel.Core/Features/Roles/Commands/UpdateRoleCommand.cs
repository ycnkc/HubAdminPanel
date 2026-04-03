using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Features.Roles.Commands
{
    public class UpdateRoleCommand : IRequest<bool>
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public List<int> EndpointIds { get; set; }
    }
}
