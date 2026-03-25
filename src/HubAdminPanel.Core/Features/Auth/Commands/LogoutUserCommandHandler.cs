using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, bool>
    {
        private readonly IAppDbContext _context;

        public LogoutUserCommandHandler(IAppDbContext context) => _context = context;

        public async Task<bool> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}