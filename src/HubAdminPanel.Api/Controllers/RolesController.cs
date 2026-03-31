using HubAdminPanel.Core.Features.Users.Commands;
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
    }
}
