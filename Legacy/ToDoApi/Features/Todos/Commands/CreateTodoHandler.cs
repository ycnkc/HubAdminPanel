using MediatR;
using Microsoft.AspNetCore.SignalR;
using ToDoApi.Data;
using ToDoApi.DTOs;
using ToDoApi.Hubs;
using ToDoApi.Models;

    public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, TodoResponseDto>{
    private readonly AppDbContext _context;
    private readonly EmbeddingService _embeddingService;
    private readonly PineconeService _pineconeService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public CreateTodoHandler(AppDbContext context, EmbeddingService embeddingService, PineconeService pineconeService, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _embeddingService = embeddingService;
        _pineconeService = pineconeService;
        _hubContext = hubContext;
    }

    public async Task<TodoResponseDto> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new TodoItem
        {
            Title = request.Title,
            UserId = request.UserId,
            IsCompleted = false
        };

        _context.TodoItems.Add(todo);
        await _context.SaveChangesAsync();

        try
        {
            float[] vector = await _embeddingService.GetEmbeddingAsync(todo.Title);
            await _pineconeService.UpsertVectorAsync(todo.Id, vector, request.UserId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Pinecone error: {ex.Message}");
        }

        await _hubContext.Clients.User(request.UserId.ToString()).SendAsync("ReceiveNotification", $"New task added: {todo.Title}");


        return new TodoResponseDto
        {
            Id = todo.Id, 
            Title = todo.Title, 
            IsCompleted = todo.IsCompleted 
        };

    
    }
    }