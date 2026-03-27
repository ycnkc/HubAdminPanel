using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using MediatR;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    /// <summary>
    /// Handles the self-registration process for new users.
    /// This handler manages password security and assigns initial default access levels.
    /// </summary>
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly IAppDbContext _context;

        /// <summary>
        /// Initializes the registration handler with the application's database context.
        /// </summary>
        /// <param name="context">The database context used for persisting user data.</param>
        public RegisterUserCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Processes the registration request by hashing the password and creating a new user entity.
        /// </summary>
        /// <param name="request">The command containing registration details (username, email, password).</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
        /// <returns>True if the user was successfully created; otherwise, false.</returns>
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