using MediatR;

namespace HubAdminPanel.Core.Features.Users.Commands
{
    /// <summary>
    /// Represents a command to create a new user role within the system.
    /// This request is handled by the <see cref="CreateRoleCommandHandler"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="Name"/> property must be unique; otherwise, 
    /// the handler will throw a validation exception.
    /// </remarks>
    public class CreateRoleCommand : IRequest<bool>
    {
        public string Name { get; set; } = string.Empty;
    }
}
