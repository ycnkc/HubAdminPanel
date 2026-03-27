using MediatR;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    /// <summary>
    /// Represents a command to register a new user account through the public registration flow.
    /// This request is processed by the <see cref="RegisterUserCommandHandler"/>.
    /// </summary>
    /// <remarks>
    /// Unlike the administrative user creation, this command typically assigns 
    /// a default user role (e.g., Guest or Standard User) automatically.
    /// </remarks>
    public class RegisterUserCommand : IRequest<bool>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
