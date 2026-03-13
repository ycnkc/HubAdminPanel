using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AIService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;

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


}