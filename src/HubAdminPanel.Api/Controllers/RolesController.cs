using HubAdminPanel.Core.Features.Roles.Commands;
using HubAdminPanel.Core.Features.Roles.Queries;
using HubAdminPanel.Core.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubAdminPanel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        [Authorize(Policy = "UserView")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _mediator.Send(new GetAllRolesQuery());
            return Ok(roles);
        }

        [HttpPost]
        [Authorize(Policy = "RoleManage")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var roleId = await _mediator.Send(command);
            return Ok(roleId);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RoleManage")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleCommand command)
        {
            if (id != command.Id) return BadRequest("ID uyuşmazlığı.");

            var result = await _mediator.Send(command);
            return result ? Ok() : NotFound();
        }

        [Authorize(Policy = "RoleManage")]
        [HttpGet("{id}")] 
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));
            return Ok(result);
        }
    }
}
