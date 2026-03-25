using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using MediatR;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly IAppDbContext _context;

        public RegisterUserCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, salt);

            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            newUser.UserRoles.Add(new UserRole { RoleId = 2 });

            _context.Users.Add(newUser);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}