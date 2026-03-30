using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Features.Roles.Commands
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, int>
    {
        private readonly IAppDbContext _context;
        public CreateRoleCommandHandler(IAppDbContext context) => _context = context;

        public async Task<int> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            // 1. Rolü oluştur
            var role = new Role { Name = request.Name };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync(cancellationToken);

            // 2. Seçilen yetkileri RolePermission tablosuna ekle
            if (request.PermissionIds.Any())
            {
                var rolePermissions = request.PermissionIds.Select(pId => new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = pId
                });

                _context.RolePermissions.AddRange(rolePermissions);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return role.Id;
        }
    }
}
