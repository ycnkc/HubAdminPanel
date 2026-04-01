using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Entities
{
    public class EndpointPermissionMapping
    {
        public int EndpointId { get; set; }
        public int PermissionId { get; set; }

        public virtual Endpoint Endpoint { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
