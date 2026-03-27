using HubAdminPanel.Core.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubAdminPanel.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase 
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-role")]
        public async Task<IActionResult> Create(CreateRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
