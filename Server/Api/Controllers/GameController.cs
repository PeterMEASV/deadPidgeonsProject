using Api.Models;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameController> _logger;

    public GameController(IGameService gameService, ILogger<GameController> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }
    
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> CreateGame([FromBody] DrawWinningNumbersDTO dto)
    {
        try
        {
            await _gameService.DrawWinningNumbersAsync(dto);
            var game = await _gameService.CreateGameAsync();

            return Ok(new
            {
                game.Id,
                game.Weeknumber,
                game.Isactive,
                game.Timestamp,
                Message = "New game created successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating game");
            return StatusCode(500, "An error occurred while creating the game");
        }
    }

    [HttpGet("current")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> GetCurrentGame()
    {
        try
        {
            var game = await _gameService.GetCurrentGameAsync();

            if (game == null)
            {
                return NotFound("No active game found");
            }

            return Ok(new
            {
                game.Id,
                game.Weeknumber,
                game.Winningnumbers,
                HasWinningNumbers = game.Winningnumbers.Any(),
                game.Drawdate,
                game.Isactive,
                TotalBoards = game.Boards.Count,
                TotalWinners = game.Boards.Count(b => b.Winner)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current game");
            return StatusCode(500, "An error occurred while getting the current game");
        }
    }

    [HttpGet("current/details")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> GetCurrentGameDetails()
    {
        try
        {
            var details = await _gameService.GetCurrentGameDetailsAsync();
            return Ok(details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current game details");
            return StatusCode(500, "An error occurred while getting game details");
        }
    }

    [HttpGet("history")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<object>>> GetGameHistory()
    {
        try
        {
            var games = await _gameService.GetGameHistoryAsync();

            var history = games.Select(g => new
            {
                g.Id,
                g.Weeknumber,
                g.Winningnumbers,
                HasWinningNumbers = g.Winningnumbers.Any(),
                g.Drawdate,
                g.Isactive,
                TotalBoards = g.Boards.Count,
                TotalWinners = g.Boards.Count(b => b.Winner)
            }).ToList();

            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting game history");
            return StatusCode(500, "An error occurred while getting game history");
        }
    }

    [HttpGet("{gameId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> GetGameById(string gameId)
    {
        try
        {
            var game = await _gameService.GetGameByIdAsync(gameId);
            return Ok(game);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting game {GameId}", gameId);
            return StatusCode(500, "An error occurred while getting the game");
        }
    }


}