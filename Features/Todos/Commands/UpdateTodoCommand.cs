using MediatR;
using ToDoApi.DTOs;

namespace ToDoApi.Features.Todos.Commands;

public record UpdateTodoCommand(
    int Id, 
    string Title, 
    bool IsCompleted, 
    int UserId, 
    string UserRole
) : IRequest<bool>;