using HubAdminPanel.Core.Features.Users.Commands;
using HubAdminPanel.Core.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HubAdminPanel.Api.Controllers
{
    /// <summary>
    /// Provides endpoints for managing system users.
    /// Acts as an entry point for administrative user operations like creation, updates, and deletion.
    /// </summary>
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes the controller with the MediatR mediator.
        /// </summary>
        /// <param name="mediator">The mediator instance to dispatch commands and queries.</param>
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new system user.
        /// </summary>
        /// <param name="command">The details of the user to be created.</param>
        /// <returns>A success message and the newly created User ID, or a Bad Request error.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var userId = await _mediator.Send(command);

            if (userId > 0)
            {
                return Ok(new { Message = "User created successfully.", UserId = userId });
            }

            return BadRequest("An error occurred while creating the user.");
        }


        /// <summary>
        /// Updates an existing user's information.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="command">The updated user data.</param>
        /// <returns>A success message if updated; otherwise, a Not Found response.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;

            var result = await _mediator.Send(command);

            if (!result) return NotFound("User not found.");

            return Ok("User updated successfully.");
        }

        /// <summary>
        /// Retrieves a comprehensive list of all system users.
        /// </summary>
        /// <returns>A collection of User data.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Permanently removes a user from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>A success message if deleted; otherwise, a Not Found response.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteUserCommand { Id = id });

            if (!result) return NotFound("Kullanıcı bulunamadı.");

            return Ok("Kullanıcı başarıyla silindi.");
        }
    }
}
