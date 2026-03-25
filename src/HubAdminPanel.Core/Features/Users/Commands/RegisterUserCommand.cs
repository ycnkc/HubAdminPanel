using MediatR;

namespace HubAdminPanel.Core.Features.Users.Commands
{
    public class RegisterUserCommand : IRequest<bool>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
