using HubAdminPanel.Core.Entities;

namespace HubAdminPanel.Core.DTOs
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<EndpointRoleMappingDto> EndpointRoleMappings { get; set; } = new();
        public List<int> AssignedEndpointIds { get; set; }
    }

}
