using HubAdminPanel.Core.Features.Roles.Commands;
using HubAdminPanel.Core.Features.Roles.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HubAdminPanel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _mediator.Send(new GetAllRolesQuery());
            return Ok(roles);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var roleId = await _mediator.Send(command);
            return Ok(roleId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleCommand command)
        {
            if (id != command.Id) return BadRequest("ID uyuşmazlığı.");

            var result = await _mediator.Send(command);
            return result ? Ok() : NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));
            return Ok(result);
        }
    }
}
