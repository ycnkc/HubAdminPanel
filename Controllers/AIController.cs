using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApi.Services;
using ToDoApi.Models;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;

    
    public AIController(IAIService aiService)
    {
        _aiService = aiService;
    }

    [HttpGet("suggest")]
    public async Task<IActionResult> GetSuggestion(string task)
    {
        var response = await _aiService.GetAIResponseAsync($"Give me an advice for this task: {task}");
        return Ok(new { Suggestion = response });
    }


    /// <summary>
    /// You can chat here.
    /// </summary>
    [HttpGet("chat")]
    public async Task<IActionResult> Chat([FromQuery] string prompt)
    {
        var answer = await _aiService.GetSmartAiAnswer(prompt, CurrentUserId);
        return Ok(new {response = answer});
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("analyze-error-log")]
    public async Task<IActionResult> AnalyzeLog(IFormFile logFile)
    {
        if (logFile == null || logFile.Length == 0)
            return BadRequest("Please upload a valid .txt file.");

        using var reader = new StreamReader(logFile.OpenReadStream());
        string fullLogContent = await reader.ReadToEndAsync();

        var analysisResult = await _aiService.SummarizeExceptionAsync(fullLogContent);

        _aiService.LogToTerminal(analysisResult);

        return Ok(new
        {
            Analysis = analysisResult,
            ProcessedAt = DateTime.Now
        });
    }

    private int CurrentUserId => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
    private string CurrentUserRole => User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;

}



