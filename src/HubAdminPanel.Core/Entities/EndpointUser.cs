using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Entities
{
    public class EndpointUser
    {
        public int Id { get; set; }
        public int EndpointId { get; set; }
        public int UserId { get; set; }

        public Endpoint Endpoint { get; set; }
        public User User { get; set; }
    }
}
