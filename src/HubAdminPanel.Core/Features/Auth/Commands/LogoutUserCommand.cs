using MediatR;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    public class LogoutUserCommand : IRequest<bool>
    {
        public int UserId { get; set; }
    }
}