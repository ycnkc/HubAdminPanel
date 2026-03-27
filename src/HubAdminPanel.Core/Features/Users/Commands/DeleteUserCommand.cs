using MediatR;

namespace HubAdminPanel.Core.Features.Users.Commands
{
    /// <summary>
    /// Represents a command to permanently delete a user from the system.
    /// This request triggers the <see cref="DeleteUserCommandHandler"/>.
    /// </summary>
    /// <remarks>
    /// Use this command with caution as it performs a hard delete, 
    /// affecting the user record and its related role assignments.
    /// </remarks>
    public class DeleteUserCommand : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user to be deleted.
        /// </summary>
        /// <value>The primary key (Id) of the User entity.</value>
        public int Id { get; set; }
    }
}