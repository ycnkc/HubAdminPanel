using HubAdminPanel.Core.Features.Users.Commands;
using HubAdminPanel.Core.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubAdminPanel.Api.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController] 
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

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


        [HttpPut("{id}")] 
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;

            var result = await _mediator.Send(command);

            if (!result) return NotFound("User not found.");

            return Ok("User updated successfully.");
        }

        [HttpGet] 
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteUserCommand { Id = id });

            if (!result) return NotFound("Kullanıcı bulunamadı.");

            return Ok("Kullanıcı başarıyla silindi.");
        }
    }
}
