using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;
using ToDoApi.Services;

namespace ToDoApi.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, AuthService authService, IConfiguration configuration)
        {
            _context = context;
            _authService = authService;
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <remarks>
        /// The password provided in the request is automatically hashed before being stored in the database.
        /// </remarks>
        /// <param name="user">The user object containing username, password, and role.</param>
        /// <response code="200">Returns a success message if the user is created successfully.</response>
        /// <response code="400">Returns if the input data is invalid or the username already exists.</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            user.PasswordHash = _authService.HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User created succesfully");
        }


        /// <summary>
        /// Authenticates a user and generates both Access and Refresh tokens.
        /// </summary>
        /// <param name="loginUser">User credentials.</param>
        /// <returns>A pair of tokens for authentication and session renewal.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (dbUser == null || !_authService.VerifyPassword(request.PasswordHash, dbUser.PasswordHash))
            {
                return Unauthorized("Invalid username or password!");
            }

            var accessToken = _authService.GenerateToken(dbUser, _configuration);

            var refreshToken = _authService.GenerateRefreshToken();

            dbUser.RefreshToken = refreshToken;
            dbUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            await _authService.RemoveRefreshTokenAsync(userId);

            return Ok(new { message = "Logout succesful. Bye!"});
        }

        /// <summary>
        /// Refreshes the expired access token using a valid refresh token.
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenModel tokenModel)
        {
            if (tokenModel is null) return BadRequest("Invalid client request");

            var principal = _authService.GetPrincipalFromExpiredToken(tokenModel.AccessToken, _configuration);
            var username = principal.Identity.Name;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid refresh token request");
            }

            var newAccessToken = _authService.GenerateToken(user, _configuration);
            var newRefreshToken = _authService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _context.SaveChangesAsync();

            return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }


    }
}
