using HubAdminPanel.Core.DTOs;
using MediatR;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    /// <summary>
    /// Represents a request to exchange a valid Refresh Token for a new pair of 
    /// Access and Refresh Tokens.
    /// This command is handled by the <see cref="RefreshTokenCommandHandler"/>.
    /// </summary>
    /// <remarks>
    /// This mechanism implements "Token Rotation" to enhance security by 
    /// invalidating the old refresh token once a new one is issued.
    /// </remarks>
    public class RefreshTokenCommand : IRequest<AuthResponseDto>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
