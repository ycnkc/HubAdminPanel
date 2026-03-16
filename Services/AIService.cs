using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using ToDoApi.Data;
using Microsoft.EntityFrameworkCore;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    private readonly EmbeddingService _embeddingService;
    private readonly PineconeService _pineconeService;
    private readonly AppDbContext _context;

    public AIService(HttpClient httpClient, IConfiguration config, AppDbContext context, EmbeddingService embeddingService, PineconeService pineconeService)
    {
        _httpClient = httpClient;
        _config = config;
        _embeddingService = embeddingService;
        _pineconeService = pineconeService;
        _context = context;

    }

    public async Task<string> GetAIResponseAsync(string userPrompt)
{
    var baseUrl = "http://localhost:11434/api/chat";

    var requestBody = new
    {
        model = "phi3", 
        messages = new[]
        {
            new { role = "system", content = "You are a to-do assistant." },
            new { role = "user", content = userPrompt }
        },
        stream = false 
    };

    var response = await _httpClient.PostAsJsonAsync(baseUrl, requestBody);
    
    if (response.IsSuccessStatusCode)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        
        using var doc = JsonDocument.Parse(responseString);
        var root = doc.RootElement;

        if (root.TryGetProperty("message", out var messageElement))
        {
            return messageElement.GetProperty("content").GetString() ?? "Answer null.";
        }
        
        return "Message not found.";
    }

    return $"Error: {response.StatusCode}";
}
    
    public async Task<string> GetSmartAiAnswer(string userQuestion, int userId)
    {
        var queryVector = await _embeddingService.GetEmbeddingAsync(userQuestion);

        var relatedIds = await _pineconeService.QuerySimilarVectorsAsync(queryVector, userId);

        var todos = await _context.TodoItems
        .Where(t => relatedIds.Contains(t.Id.ToString()))
        .ToListAsync();

        string context = string.Join("\n", todos.Select(t => $"- {t.Title}"));

        string finalPrompt = $@"You are a helper to-do assistant.
        Related tasks in the user's database:
        {context}
        
        Question: {userQuestion}
        
        Based on the tasks above, generate a short, professional and helping answer please.";

        return await GetAIResponseAsync(finalPrompt);

    }



}