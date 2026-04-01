namespace HubAdminPanel.Core.Entities
{
    /// <summary>
    /// Represents an atomic permission or access right within the system.
    /// This is the most granular unit of the authorization logic.
    /// </summary>
    /// <remarks>
    /// Permissions are mapped to roles via <see cref="RolePermission"/>. 
    /// Policies typically check against the <see cref="Key"/> property to grant or deny access.
    /// </remarks>
    public class Permission
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        public virtual ICollection<EndpointPermissionMapping> EndpointPermissionMappings { get; set; }
    }
}
