using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;

namespace HubAdminPanel.Core.Features.Users.Commands
{
    public class CreateUserCommandHandler
    {

        private readonly IAppDbContext _context;

        public CreateUserCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            // Seçilen rolleri kullanıcıya bağla
            foreach (var roleId in request.RoleIds)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });
            }

            await _context.SaveChangesAsync(cancellationToken);
            return user.Id;
        }
    }
}
