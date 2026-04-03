using HubAdminPanel.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.DTOs
{
    public class EndpointRoleMappingDto
    {
        public int EndpointId { get; set; }
        public int RoleId { get; set; }
        public RoleDto Role { get; set; }
    }
}
