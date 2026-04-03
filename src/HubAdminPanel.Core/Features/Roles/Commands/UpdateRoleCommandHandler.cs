using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Roles.Commands
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly IAppDbContext _context;

        public UpdateRoleCommandHandler(IAppDbContext context) => _context = context;

        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (role == null) return false;


            role.Name = request.Name;




            

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
