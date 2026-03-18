using MediatR;
using ToDoApi.DTOs;

namespace ToDoApi.Features.Todos.Queries;

public record GetTodoByIdQuery(int Id, int UserId, string UserRole) : IRequest<TodoResponseDto?>;