using HubAdminPanel.Core.DTOs;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    /// <summary>
    /// Handles the user authentication process.
    /// Validates credentials, checks account status, and issues a fresh set of security tokens.
    /// </summary>
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponseDto>
    {
        private readonly IAppDbContext _context;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initializes the handler with database context and token generation services.
        /// </summary>
        /// <param name="context">The application database context interface.</param>
        /// <param name="tokenService">Service used to generate JWT and Refresh tokens.</param>
        public LoginUserCommandHandler(IAppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Orchestrates the login workflow: Identity verification -> Policy checks -> Token issuance.
        /// </summary>
        /// <param name="request">The command containing user credentials (Username and Password).</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>An <see cref="AuthResponseDto"/> containing the access token, refresh token, and expiration info.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when credentials do not match or user is not found.</exception>
        /// <exception cref="Exception">Thrown when the user account is disabled/deactivated.</exception>
        public async Task<AuthResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Kullanıcı adı veya şifre hatalı!");
            }

            if (!user.IsActive)
            {
                throw new Exception("Hesabınız dondurulmuştur. Lütfen admin ile iletişime geçin.");
            }

            var accessToken = _tokenService.CreateToken(user);

            var refreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _context.SaveChangesAsync(cancellationToken);



            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.Now.AddMinutes(15)
            };
        }
    }
}
