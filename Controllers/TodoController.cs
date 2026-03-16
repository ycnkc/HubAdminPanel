using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApi.DTOs;
using System.Security.Claims;
using ToDoApi.Models;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using Microsoft.AspNetCore.RateLimiting;
using ToDoApi.Services;


[EnableRateLimiting("fixed")]
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")] 
public class TodoController : ControllerBase
{
    private readonly ITodoService _service;
    private readonly AppDbContext _context;
    private readonly EmbeddingService _embeddingService;
    private readonly PineconeService _pineconeService;

    public TodoController(ITodoService service, AppDbContext context, EmbeddingService embeddingService, PineconeService pineconeService)
    {
        _service = service;
        _context = context;
        _embeddingService = embeddingService;
        _pineconeService = pineconeService;
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
        var query = _context.TodoItems.AsQueryable();

        if (CurrentUserRole != "Admin")
        {
            query = query.Where(x => x.UserId == CurrentUserId);
        }

        if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
        {
            query = query.Where(testc => testc.Title.Contains(queryParameters.SearchTerm));
        }

        if (queryParameters.IsCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == queryParameters.IsCompleted.Value);
        }

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(queryParameters.SortBy))
        {
            query = queryParameters.SortBy.ToLower() switch
            {
                "title" => queryParameters.IsDescending
                    ? query.OrderByDescending(t => t.Title)
                    : query.OrderBy(t => t.Title),
                "iscompleted" => queryParameters.IsDescending
                    ? query.OrderByDescending(t => t.IsCompleted)
                    : query.OrderBy(t => t.IsCompleted),
                _ => queryParameters.IsDescending
                    ? query.OrderByDescending(t => t.Id)
                    : query.OrderBy(t => t.Id)
            };
        }
        else
        {
            query = query.OrderBy(t => t.Id);
        }

        var items = await query
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .Select(x => new TodoResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    IsCompleted = x.IsCompleted
                })
                .ToListAsync();

        return Ok(new
        {
            TotalCount = totalCount,
            PageNumber = queryParameters.PageNumber,
            PageSize = queryParameters.PageSize,
            Items = items
        });
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
        var todo = await _service.GetByIdAsync(id);

        if (todo == null)
            return NotFound();

        if (CurrentUserRole != "Admin" && todo.UserId != CurrentUserId)
        {
            return Forbid();
        }

        var response = new TodoResponseDto
        {
            Id = todo.Id,
            Title = todo.Title,
            IsCompleted = todo.IsCompleted
        };

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
        var item = await _service.CreateAsync(dto.Title, CurrentUserId);

        try
        {
            float[] vector = await _embeddingService.GetEmbeddingAsync(item.Title);
            await _pineconeService.UpsertVectorAsync(item.Id, vector, CurrentUserId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Saving vectors unsuccessful. {ex.Message}");
        }

        var response = new TodoResponseDto {Id = item.Id, Title = item.Title};
        return CreatedAtAction(nameof(GetById), new {id = item.Id}, response);
    }

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <response code="204">If the update was successful.</response>
    /// <response code="404">If the item to update does not exist.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] TodoResponseDto dto)
    {
        var todo = await _service.GetByIdAsync(id);

        if (todo == null)
            return NotFound();
        if (CurrentUserRole != "Admin" && todo.UserId != CurrentUserId)
        {
            return Forbid();
        }

        var result = await _service.UpdateAsync(id, dto.Title, dto.IsCompleted);

        if (!result)
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
        var todo = await _service.GetByIdAsync(id);

        if (todo == null)
            return NotFound();

        if (CurrentUserRole != "Admin" && todo.UserId != CurrentUserId)
        {
            return Forbid();
        }

        var result = await _service.DeleteAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }




    public int CurrentUserId => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
    public string CurrentUserRole => User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;
}