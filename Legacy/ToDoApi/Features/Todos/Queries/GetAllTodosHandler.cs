using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.DTOs;

namespace ToDoApi.Features.Todos.Queries;

public class GetAllTodosHandler : IRequestHandler<GetAllTodosQuery, object>
{
    private readonly AppDbContext _context;

    public GetAllTodosHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<object> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TodoItems.AsQueryable();
        var p = request.Parameters;

        if (request.UserRole != "Admin")
        {
            query = query.Where(x => x.UserId == request.UserId);
        }

        if (!string.IsNullOrWhiteSpace(p.SearchTerm))
        {
            query = query.Where(t => t.Title.Contains(p.SearchTerm));
        }

        if (p.IsCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == p.IsCompleted.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(p.SortBy))
        {
            query = p.SortBy.ToLower() switch
            {
                "title" => p.IsDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "iscompleted" => p.IsDescending ? query.OrderByDescending(t => t.IsCompleted) : query.OrderBy(t => t.IsCompleted),
                _ => p.IsDescending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id)
            };
        }
        else
        {
            query = query.OrderBy(t => t.Id);
        }

        var items = await query
                .Skip((p.PageNumber - 1) * p.PageSize)
                .Take(p.PageSize)
                .Select(x => new TodoResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    IsCompleted = x.IsCompleted
                })
                .ToListAsync(cancellationToken);

        return new
        {
            TotalCount = totalCount,
            PageNumber = p.PageNumber,
            PageSize = p.PageSize,
            Items = items
        };
    }
}