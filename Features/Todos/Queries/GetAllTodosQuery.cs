using MediatR;
using ToDoApi.Models;

namespace ToDoApi.Features.Todos.Queries;

public record GetAllTodosQuery(
    TodoQueryParameters Parameters, 
    int UserId, 
    string UserRole
) : IRequest<object>; 