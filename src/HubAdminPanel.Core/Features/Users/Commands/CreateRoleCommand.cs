using MediatR;

namespace HubAdminPanel.Core.Features.Users.Commands
{
    public class CreateRoleCommand : IRequest<bool>
    {
        public string Name { get; set; } = string.Empty;
    }
}
