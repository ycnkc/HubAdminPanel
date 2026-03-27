using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    /// <summary>
    /// Handles the logout process by invalidating the user's current session on the server side.
    /// This ensures that the refresh token can no longer be used to obtain new access tokens.
    /// </summary>
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, bool>
    {
        private readonly IAppDbContext _context;

        /// <summary>
        /// Initializes the handler with the application database context.
        /// </summary>
        /// <param name="context">The database context for managing user sessions.</param>
        public LogoutUserCommandHandler(IAppDbContext context) => _context = context;

        /// <summary>
        /// Executes the logout logic by clearing session-related fields for a specific user.
        /// </summary>
        /// <param name="request">The command containing the ID of the user logging out.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
        /// <returns>True if the user was found and session data was cleared; otherwise, false.</returns>
        public async Task<bool> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}