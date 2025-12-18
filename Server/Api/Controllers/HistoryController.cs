using Api.Services.Interfaces;
using Api.Models;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HistoryController : ControllerBase
{
    
    private readonly ILogger<HistoryController> _logger;
    private readonly IHistoryService _historyService;
    public HistoryController(ILogger<HistoryController> logger, IHistoryService historyService)
    {
        _logger = logger;
        _historyService = historyService;
    }
    
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<Historylog>>> GetAllLogs()
    {
        return await _historyService.GetAllLogsAsync();
    }
    
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<BoardHistoryDTO>>> GetUserBoardHistory(string userId)
    {
        try
        {
            var history = await _historyService.GetUserBoardHistoryAsync(userId);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting board history for user {UserId}", userId);
            return StatusCode(500, "An error occurred while getting board history");
        }
    }


    
}