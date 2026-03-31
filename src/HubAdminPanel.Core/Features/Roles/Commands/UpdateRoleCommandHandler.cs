using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Features.Roles.Commands
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly IAppDbContext _context;

        public UpdateRoleCommandHandler(IAppDbContext context) => _context = context;

        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (role == null) return false;

            // 1. Rol ismini güncelle
            role.Name = request.Name;

            // 2. Eski yetkileri temizle (En garanti yol budur)
            _context.RolePermissions.RemoveRange(role.RolePermissions);

            // 3. Yeni yetkileri ekle
            if (request.PermissionIds != null && request.PermissionIds.Any())
            {
                var newPermissions = request.PermissionIds.Select(pId => new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = pId
                });
                _context.RolePermissions.AddRange(newPermissions);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
