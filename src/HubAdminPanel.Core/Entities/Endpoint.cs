using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Entities
{
    public class Endpoint
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }

        public virtual ICollection<EndpointPermissionMapping> EndpointPermissionMappings { get; set; }
    }
}
