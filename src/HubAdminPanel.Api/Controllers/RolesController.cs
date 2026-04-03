using HubAdminPanel.Core.Features.Roles.Commands;
using HubAdminPanel.Core.Features.Roles.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HubAdminPanel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;

        public RolesController(IMediator mediator, IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _mediator.Send(new GetAllRolesQuery());
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var roleId = await _mediator.Send(command);

            _cache.Remove("AllEndpoints");

            return Ok(roleId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleCommand command)
        {
            if (id != command.Id) return BadRequest("ID uyuşmazlığı.");

            var result = await _mediator.Send(command);

            if (result)
            {
                _cache.Remove("AllEndpoints");
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id));

            if (!result)
                return NotFound(new { message = "Rol bulunamadı." });

            _cache.Remove("AllEndpoints");

            return Ok();
        }
    }
}