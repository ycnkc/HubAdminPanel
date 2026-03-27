using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Users.Commands
{
    /// <summary>
    /// Handles the permanent removal of a user and their associated data from the system.
    /// </summary>
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IAppDbContext _context;

        /// <summary>
        /// Initializes the delete handler with the necessary database context.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public DeleteUserCommandHandler(IAppDbContext context) => _context = context;

        /// <summary>
        /// Deletes a specific user by their ID. 
        /// Manually cleans up related user-role assignments before deleting the user entity.
        /// </summary>
        /// <param name="request">The command containing the unique ID of the user to be deleted.</param>
        /// <param name="cancellationToken">Cancellation token to abort the process if needed.</param>
        /// <returns>True if the user was successfully found and deleted; otherwise, false.</returns>
        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Fetch the user including their roles to ensure related records are tracked for deletion
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            // Return false if the user doesn't exist (Idempotency: or handle as a 'Not Found' error)
            if (user == null) return false;

            // Manual cleanup of the UserRoles table to maintain referential integrity
            // especially if Cascade Delete is not configured at the database level.
            _context.UserRoles.RemoveRange(user.UserRoles);

            // Remove the primary user record
            _context.Users.Remove(user);

            // Execute the deletion within a single transaction
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}