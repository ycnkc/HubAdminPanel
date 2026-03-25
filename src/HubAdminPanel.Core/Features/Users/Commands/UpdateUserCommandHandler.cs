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
    public class UpdateUserCommandHandler
    {

        private readonly IAppDbContext _context;

        public UpdateUserCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == request.Id);

            if (user == null) return false;

            user.Email = request.Email;
            user.IsActive = request.IsActive;

            // Mevcut rolleri sil ve yenilerini ekle (En basit güncelleme mantığı)
            _context.UserRoles.RemoveRange(user.UserRoles);
            foreach (var roleId in request.RoleIds)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });
            }

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
