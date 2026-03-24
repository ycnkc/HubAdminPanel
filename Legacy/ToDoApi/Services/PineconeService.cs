using ToDoApi.Models;

public class PineconeService {
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public PineconeService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["PineconeSettings:ApiKey"];
        _baseUrl = configuration["PineconeSettings:BaseUrl"];
    }

    public async Task UpsertVectorAsync(int todoId, float[] vector, int userId)
    {
        var requestBody = new
        {
            vectors = new[]
            {
                new
                {
                    id = todoId.ToString(),
                    values = vector,
                    metadata = new {UserId = userId}
                }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/vectors/upsert");
        request.Headers.Add("Api-Key", _apiKey);
        request.Content = JsonContent.Create(requestBody);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<string>> QuerySimilarVectorsAsync(float[] queryVector, int userId)
    {
        var requestBody = new
        {
            vector = queryVector,
            topK = 3,
            includeMetadata = false,
            filter = new {UserId = userId}
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/query");
        request.Headers.Add("Api-Key", _apiKey);
        request.Content = JsonContent.Create(requestBody);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PineconeQueryResponse>();
        return result?.Matches.Select(m => m.Id).ToList() ?? new List<string>();

    }
}