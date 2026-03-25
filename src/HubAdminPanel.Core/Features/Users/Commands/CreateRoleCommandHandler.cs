using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Features.Users.Commands
{
    public class CreateRoleCommandHandler
    {
        private readonly IAppDbContext _context;

         public CreateRoleCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (await _context.Roles.AnyAsync(r => r.Name == request.Name))
                throw new Exception("This role already exists.");

            _context.Roles.Add(new Role { Name = request.Name });
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
