using HubAdminPanel.Core.DTOs;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    /// <summary>
    /// Handles the re-authentication process using a Refresh Token.
    /// This allows users to obtain a new Access Token without re-entering credentials.
    /// </summary>
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly IAppDbContext _context;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initializes the handler with database context and token management services.
        /// </summary>
        /// <param name="context">The database context for user session validation.</param>
        /// <param name="tokenService">Service responsible for generating JWT and Refresh tokens.</param>
        public RefreshTokenCommandHandler(IAppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Validates the existing refresh token and issues a new token pair if valid.
        /// </summary>
        /// <param name="request">The command containing the current Refresh Token.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A new <see cref="AuthResponseDto"/> containing the refreshed tokens.</returns>
        /// <exception cref="Exception">Thrown when the refresh token is invalid or has expired.</exception>
        public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);


            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new Exception("Session expired. Please log in again.");
            }

            var newAccessToken = _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _context.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Expiration = DateTime.Now.AddMinutes(15)
            };
        }
    }
}
