namespace HubAdminPanel.Core.Entities
{
    public class Endpoint
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }

        public string ControllerName { get; set; }
        public ICollection<EndpointRoleMapping> EndpointRoleMappings { get; set; }

    }
}
