namespace HubAdminPanel.Core.Entities
{
    /// <summary>
    /// Represents a security role within the system.
    /// Roles act as a bridge between users and specific granular permissions.
    /// </summary>
    /// <remarks>
    /// This entity is a central part of the RBAC (Role-Based Access Control) architecture,
    /// enabling the grouping of permissions for easier management.
    /// </remarks>
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<EndpointRoleMapping> EndpointRoleMappings { get; set; }

    }
}
