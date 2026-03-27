using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Users.Commands
{
    /// <summary>
    /// Handles the creation of new system roles.
    /// Ensures that role names remain unique across the application.
    /// </summary>
    public class CreateRoleCommandHandler
    {
        private readonly IAppDbContext _context;

        /// <summary>
        /// Initializes the handler with the database context.
        /// </summary>
        /// <param name="context">The application database context interface.</param>
        public CreateRoleCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Processes the request to create a new role after validating its uniqueness.
        /// </summary>
        /// <param name="request">The command containing the name of the new role.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>True if the role was successfully created; otherwise, false.</returns>
        /// <exception cref="Exception">Thrown when a role with the same name already exists in the database.</exception>
        public async Task<bool> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (await _context.Roles.AnyAsync(r => r.Name == request.Name))
                throw new Exception("This role already exists.");

            _context.Roles.Add(new Role { Name = request.Name });
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
