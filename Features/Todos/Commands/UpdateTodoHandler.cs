using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;

namespace ToDoApi.Features.Todos.Commands;

public class UpdateTodoHandler : IRequestHandler<UpdateTodoCommand, bool>
{
    private readonly AppDbContext _context;

    public UpdateTodoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _context.TodoItems
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (todo == null) return false;

        if (request.UserRole != "Admin" && todo.UserId != request.UserId)
        {
            return false; 
        }

        todo.Title = request.Title;
        todo.IsCompleted = request.IsCompleted;

        _context.TodoItems.Update(todo);
        await _context.SaveChangesAsync(cancellationToken);

        return true; 
    }
}