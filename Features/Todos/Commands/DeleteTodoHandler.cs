using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;

namespace ToDoApi.Features.Todos.Commands;

public class DeleteTodoHandler : IRequestHandler<DeleteTodoCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteTodoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _context.TodoItems
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (todo == null) return false;

        if (request.UserRole != "Admin" && todo.UserId != request.UserId)
        {
            return false; 
        }

        _context.TodoItems.Remove(todo);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}