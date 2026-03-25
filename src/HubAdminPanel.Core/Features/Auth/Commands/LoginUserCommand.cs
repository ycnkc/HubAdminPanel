using HubAdminPanel.Core.DTOs;
using MediatR;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    public class LoginUserCommand : IRequest<AuthResponseDto>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
