using HubAdminPanel.Core.DTOs;
using HubAdminPanel.Core.Features.Auth.Commands;
using HubAdminPanel.Core.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HubAdminPanel.Api.Controllers
{
    /// <summary>
    /// Manages authentication and session-related operations.
    /// Provides endpoints for user registration, login, token refresh, and secure logout.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes the AuthController with the MediatR mediator.
        /// </summary>
        /// <param name="mediator">The mediator instance to dispatch authentication commands.</param>
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Registers a new user into the system with default permissions.
        /// </summary>
        /// <param name="command">Registration details including username, email, and password.</param>
        /// <returns>A success message upon successful registration.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok("Registered successfully.");
        }

        /// <summary>
        /// Authenticates a user and issues a set of JWT Access and Refresh tokens.
        /// </summary>
        /// <param name="command">The user's login credentials.</param>
        /// <returns>An AuthResponseDto containing tokens if successful; otherwise, Unauthorized.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (result != null && !string.IsNullOrEmpty(result.AccessToken))
            {
                return Ok(result);
            }

            return Unauthorized(result);
        }

        /// <summary>
        /// Exchanges a valid Refresh Token for a new pair of Access and Refresh tokens.
        /// Requires an active authorization header to initiate the refresh flow.
        /// </summary>
        /// <param name="command">The current refresh token to be validated.</param>
        /// <returns>A new set of tokens or a Bad Request if the refresh token is invalid/expired.</returns>
        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Terminates the current user session by invalidating the refresh token in the database.
        /// Identity is extracted from the 'NameIdentifier' claim of the authenticated user.
        /// </summary>
        /// <returns>A success message if the session was successfully terminated.</returns>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var result = await _mediator.Send(new LogoutUserCommand { UserId = int.Parse(userIdClaim) });

            return result ? Ok("Logout successful.") : BadRequest("Logout unsuccessful.");
        }

        [Authorize] 
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateToken([FromBody] TokenRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var command = new GenerateTokenCommand
            {
                Name = request.Name,
                ExpireDays = request.ExpireDays,
                UserId = int.Parse(userIdClaim)
            };

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Forbid(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("tokens")]
        public async Task<IActionResult> GetTokens()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            
            var result = await _mediator.Send(new GetTokensQuery { UserId = int.Parse(userIdClaim) });

            return Ok(result);
        }
    }
}
