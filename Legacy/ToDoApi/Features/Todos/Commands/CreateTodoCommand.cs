using MediatR;
using ToDoApi.DTOs;

public record CreateTodoCommand(string Title, int UserId) : IRequest<TodoResponseDto>;