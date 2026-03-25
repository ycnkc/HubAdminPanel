using HubAdminPanel.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
