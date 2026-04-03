using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Entities
{
    public class EndpointRoleMapping
    {
        public int EndpointId { get; set; }
        public Endpoint Endpoint { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
