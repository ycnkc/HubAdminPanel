using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Users.Commands
{
    /// <summary>
    /// Handles the creation of a new user in the system, including secure password hashing 
    /// and initial role assignments.
    /// </summary>
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
    {

        private readonly IAppDbContext _context;

        /// <summary>
        /// Initializes the handler with the application database context.
        /// </summary>
        /// <param name="context">The interface for database operations.</param>
        public CreateUserCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Processes the user creation request.
        /// </summary>
        /// <param name="request">The command containing registration details such as username, password, and roles.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>The newly created user's unique identifier (Id).</returns>
        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var isEmailExists = await _context.Users
                .AnyAsync(u => u.Email == request.Email, cancellationToken);

            if (isEmailExists)
            {
                throw new Exception("Bu e-posta adresi zaten bir kullanıcı tarafından kullanılıyor.");
            }

            // Create the main user entity. 
            // Note: Password is hashed using BCrypt to ensure security before persistence.
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true
            };

            // Step 1: Add the user to the context and save to generate the User.Id (Primary Key)
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            // Step 2: Assign roles to the newly created user using the generated Id
            foreach (var roleId in request.RoleIds)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });
            }

            // Step 3: Finalize the transaction by saving the role assignments
            await _context.SaveChangesAsync(cancellationToken);
            return user.Id;
        }
    }
}
