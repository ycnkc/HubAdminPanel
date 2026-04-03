using MediatR;

namespace HubAdminPanel.Core.Features.Roles.Commands
{
    public record DeleteRoleCommand(int Id) : IRequest<bool>;
}