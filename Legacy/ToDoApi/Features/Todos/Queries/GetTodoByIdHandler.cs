using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.DTOs;


namespace ToDoApi.Features.Todos.Queries;

public class GetTodoByIdHandler : IRequestHandler<GetTodoByIdQuery, TodoResponseDto?>
{
    private readonly AppDbContext _context;

    public GetTodoByIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TodoResponseDto?> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var todo = await _context.TodoItems
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (todo == null) return null;

        if (request.UserRole != "Admin" && todo.UserId != request.UserId)
        {
            return null;
        }

        return new TodoResponseDto
        {
            Id = todo.Id,
            Title = todo.Title,
            IsCompleted = todo.IsCompleted
        };
    }
}
