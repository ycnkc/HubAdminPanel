public interface IAIService
{
    Task<string> GetAIResponseAsync(string userPrompt);

    Task<string> GetSmartAiAnswer(string userQuestion, int userId);

    Task<string> SummarizeExceptionAsync(string logContent);
    
    void LogToTerminal(string aiResponse);

}