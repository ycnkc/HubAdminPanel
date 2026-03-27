using MediatR;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    /// <summary>
    /// Represents a command to sign out a user by invalidating their server-side session.
    /// This request is processed by the <see cref="LogoutUserCommandHandler"/>.
    /// </summary>
    /// <remarks>
    /// Primarily used to clear Refresh Tokens from the database, effectively 
    /// preventing any further token renewals for the specified user.
    /// </remarks>
    public class LogoutUserCommand : IRequest<bool>
    {
        public int UserId { get; set; }
    }
}