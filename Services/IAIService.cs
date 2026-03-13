public interface IAIService
{
    Task<string> GetAIResponseAsync(string userPrompt);

}