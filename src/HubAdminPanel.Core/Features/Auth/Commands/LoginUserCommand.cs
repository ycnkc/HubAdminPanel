using HubAdminPanel.Core.DTOs;
using MediatR;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    /// <summary>
    /// Represents a command to authenticate a user using their credentials.
    /// This request is processed by the <see cref="LoginUserCommandHandler"/>.
    /// </summary>
    /// <remarks>
    /// On successful authentication, this command returns an <see cref="AuthResponseDto"/> 
    /// containing JWT Access and Refresh tokens to manage the user session.
    /// </remarks>
    public class LoginUserCommand : IRequest<AuthResponseDto>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
