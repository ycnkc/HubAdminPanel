using ToDoApi.Models;

public class EmbeddingService {
    private readonly HttpClient _httpClient;

    public EmbeddingService(HttpClient httpClient)
    {
    _httpClient = httpClient;
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var requestBody = new
        {
            model = "nomic-embed-text",
            prompt = text
        };

        var response = await _httpClient.PostAsJsonAsync("http://localhost:11434/api/embeddings", requestBody);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();
        return jsonResponse.Embedding;
    }
}

