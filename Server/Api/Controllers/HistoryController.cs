using Api.Services.Interfaces;
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

    
}