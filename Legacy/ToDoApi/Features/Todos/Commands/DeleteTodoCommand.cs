using MediatR;

namespace ToDoApi.Features.Todos.Commands;


public record DeleteTodoCommand(int Id, int UserId, string UserRole) : IRequest<bool>;