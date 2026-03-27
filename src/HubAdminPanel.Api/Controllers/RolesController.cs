using HubAdminPanel.Core.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubAdminPanel.Api.Controllers
{
    /// <summary>
    /// Provides administrative endpoints for managing system roles and authorization levels.
    /// Access is restricted to users with the "Admin" role.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase 
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes the RolesController with the MediatR mediator.
        /// </summary>
        /// <param name="mediator">The mediator instance for dispatching role-related commands.</param>
        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new security role in the system.
        /// </summary>
        /// <param name="command">The command containing the role name and details.</param>
        /// <returns>A boolean result indicating whether the role was successfully created.</returns>
        /// <response code="200">Returns true if the role was created.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have 'Admin' privileges.</response>
        [HttpPost("create-role")]
        public async Task<IActionResult> Create(CreateRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
