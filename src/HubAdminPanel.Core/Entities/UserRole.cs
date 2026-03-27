namespace HubAdminPanel.Core.Entities
{
    /// <summary>
    /// Represents the join entity for the many-to-many relationship between Users and Roles.
    /// This entity links a specific <see cref="User"/> to a specific <see cref="Role"/>.
    /// </summary>
    public class UserRole
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

    }
}
