namespace HubAdminPanel.Core.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
