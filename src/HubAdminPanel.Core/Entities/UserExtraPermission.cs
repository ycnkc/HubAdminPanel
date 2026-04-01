using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Entities
{
    public class UserExtraPermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PermissionId { get; set; }
        public bool IsAllowed { get; set; }

        public virtual Endpoint Endpoint { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
