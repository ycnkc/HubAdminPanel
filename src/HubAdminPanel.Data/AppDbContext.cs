using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Data
{
    /// <summary>
    /// The primary database context for the application.
    /// Implements <see cref="IAppDbContext"/> to allow for dependency injection and testing.
    /// Manages the persistence of user identity and authorization data.
    /// </summary>
    public class AppDbContext : DbContext, IAppDbContext
    {

        /// <summary>
        /// Initializes a new instance of the context with the specified options.
        /// </summary>
        /// <param name="options">The options to be used by the <see cref="DbContext"/>.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Endpoint> Endpoints { get; set; }
        public DbSet<EndpointRoleMapping> EndpointRoleMappings { get; set; }



        /// <summary>
        /// Configures the database schema and defines entity relationships using Fluent API.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Data Seeding -- Pre-populates the Roles table.
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
                );



            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<EndpointRoleMapping>(entity =>
            {
                entity.HasKey(er => new { er.EndpointId, er.RoleId });

                entity.HasOne(er => er.Endpoint)
                      .WithMany(e => e.EndpointRoleMappings)
                      .HasForeignKey(er => er.EndpointId);

                entity.HasOne(er => er.Role)
                      .WithMany(r => r.EndpointRoleMappings) 
                      .HasForeignKey(er => er.RoleId);
            });


        }

    }
}
