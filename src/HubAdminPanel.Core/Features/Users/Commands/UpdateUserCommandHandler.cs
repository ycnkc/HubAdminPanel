using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Users.Commands
{
    /// <summary>
    /// Handles the logic for updating an existing user's profile information and their assigned roles.
    /// </summary>
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {

        private readonly IAppDbContext _context;

        /// <summary>
        /// Initializes the handler with the database context.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public UpdateUserCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates user details and synchronizes user roles by replacing the old ones with the new set.
        /// </summary>
        /// <param name="request">The command containing updated user data and a list of role IDs.</param>
        /// <param name="cancellationToken">Cancellation token to abort the operation if necessary.</param>
        /// <returns>True if the database update was successful (at least one row affected); otherwise, false.</returns>
        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var isDuplicate = await _context.Users
                .AnyAsync(u => u.Email == request.Email && u.Id != request.Id, cancellationToken);

            if (isDuplicate)
            {
                throw new Exception("Hata: Bu e-posta adresi zaten başka bir kullanıcı tarafından kullanılıyor.");
            }

            // Retrieve user along with existing roles to ensure we have the full object graph for updating
            var user = await _context.Users.Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == request.Id);

            if (user == null) return false;

            // Mapping basic properties from request to the entity
            user.Username = request.Username;
            user.Email = request.Email;
            user.IsActive = request.IsActive;

            // Role Synchronization Logic:
            // 1. Remove all existing roles associated with this user
            _context.UserRoles.RemoveRange(user.UserRoles);

            // 2. Add the new set of roles provided in the request
            foreach (var roleId in request.RoleIds)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });
            }

            // Persist all changes to the database in a single transaction
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
