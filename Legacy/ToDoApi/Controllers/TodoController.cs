using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApi.DTOs;
using System.Security.Claims;
using ToDoApi.Models;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using Microsoft.AspNetCore.RateLimiting;
using ToDoApi.Services;
using MediatR;
using ToDoApi.Features.Todos.Queries;
using ToDoApi.Features.Todos.Commands;


[EnableRateLimiting("fixed")]
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")] 
public class TodoController : ControllerBase
{
    private readonly IMediator _mediator;

    public TodoController(IMediator mediator)
    {
        _mediator = mediator;
    }


    /// <summary>
    /// Updates an existing todo item. (Admin and User)
    /// </summary>
    /// <response code="204">If the update was successful.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have permission to update this item.</response>
    /// <response code="404">If the item to update does not exist.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll([FromQuery] TodoQueryParameters queryParameters)
    {
        var query = new GetAllTodosQuery(queryParameters, CurrentUserId, CurrentUserRole);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific todo item by id.
    /// </summary>
    /// <param name="id">The unique identifier of the todo item.</param>
    /// <response code="200">Returns the requested todo item.</response>
    /// <response code="404">If the item is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetTodoByIdQuery(id, CurrentUserId, CurrentUserRole);
        var response = await _mediator.Send(query);

        if (response == null)
            return NotFound(); 

        return Ok(response);
    }

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Todo
    ///     {
    ///        "title": "Learn Swagger Documentation",
    ///        "isCompleted": false
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created item.</response>
    /// <response code="400">If the input data is invalid.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] TodoCreateDto dto)
    {
        var command = new CreateTodoCommand(dto.Title, CurrentUserId);
        var response = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <response code="204">If the update was successful.</response>
    /// <response code="404">If the item to update does not exist.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] TodoResponseDto dto)
    {
        var command = new UpdateTodoCommand(
            id, 
            dto.Title, 
            dto.IsCompleted, 
            CurrentUserId, 
            CurrentUserRole
        );

        var success = await _mediator.Send(command);

        if (!success)
            return NotFound(); 

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific todo item.
    /// </summary>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="404">If the item to delete does not exist.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteTodoCommand(id, CurrentUserId, CurrentUserRole);
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound();

        return NoContent();
    }




    private int CurrentUserId => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
    private string CurrentUserRole => User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;
}