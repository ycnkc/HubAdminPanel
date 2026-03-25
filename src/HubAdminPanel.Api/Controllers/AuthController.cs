using HubAdminPanel.Core.DTOs;
using HubAdminPanel.Core.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HubAdminPanel.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok("Registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.StartsWith("ey"))
            {
                return Ok(new { Token = result, Message = "Login successful." });
            }

            return Unauthorized(result);
        }
    }
}
