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
        /// Authenticates a user and generates a JWT Token.
        /// </summary>
        /// <remarks>
        /// Upon successful authentication, an access token is returned to be used for authorized requests.
        /// </remarks>
        /// <param name="loginUser">The credentials (username and password) of the user.</param>
        /// <response code="200">Returns the JWT access token if credentials are valid.</response>
        /// <response code="401">Returns if the username or password is incorrect.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] User loginUser)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginUser.Username);

            if (dbUser == null || !_authService.VerifyPassword(loginUser.PasswordHash, dbUser.PasswordHash))
            {
                return Unauthorized("Kullanıcı adı veya şifre hatalı!");
            }

            var token = _authService.GenerateToken(dbUser, _configuration);
            return Ok(new { Token = token });
        }
    }
}
