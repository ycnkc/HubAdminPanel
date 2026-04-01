using HubAdminPanel.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        DbSet<RolePermission> RolePermissions { get; }
        DbSet<Permission> Permissions { get; }
        public DbSet<Token> Tokens { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
