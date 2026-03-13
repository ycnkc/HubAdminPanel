using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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
    public async Task<IActionResult> Chat(string message)
    {
        var response = await _aiService.GetAIResponseAsync($"{message}");
        return Ok(new {Chat = response});
    }
}