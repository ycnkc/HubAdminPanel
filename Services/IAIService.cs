public interface IAIService
{
    Task<string> GetAIResponseAsync(string userPrompt);

    Task<string> GetSmartAiAnswer(string userQuestion, int userId);

}