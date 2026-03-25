using MediatR;

namespace HubAdminPanel.Core.Features.Users.Commands
{
    public class LoginUserCommand : IRequest<string>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
