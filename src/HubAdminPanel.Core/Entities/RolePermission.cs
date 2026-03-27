namespace HubAdminPanel.Core.Entities
{
    /// <summary>
    /// Represents the join entity for the many-to-many relationship between Roles and Permissions.
    /// This entity maps specific access rights (Permissions) to a given <see cref="Role"/>.
    /// </summary>
    /// <remarks>
    /// This is a key component of the Role-Based Access Control (RBAC) system, 
    /// allowing for granular permission management.
    /// </remarks>
    public class RolePermission
    {
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
    }
}
