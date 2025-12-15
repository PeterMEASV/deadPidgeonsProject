using Api.Models;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardController : ControllerBase
{
    private readonly IBoardService _boardService;
	private readonly ILogger<BoardController> _logger;

	public BoardController(IBoardService boardService, ILogger<BoardController> logger)
	{
		_boardService = boardService;
		_logger = logger;
	}

	[HttpPost("create")]
	public async Task<ActionResult<BoardResponseDTO>> CreateBoard([FromBody] CreateBoardDTO dto)
	{
		try
		{
			var board = await _boardService.CreateBoardAsync(dto);
			
			var response = new BoardResponseDTO(
			board.Id,
			board.Userid,
			board.Selectednumbers,
			board.Timestamp,
			board.Winner,
			_boardService.CalculateBoardPrice(board.Selectednumbers.Count));

			return CreatedAtAction(nameof(GetBoardById), new { boardId = board.Id }, response);
		}
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating board");
            return StatusCode(500, "An error occurred while creating the board");
        }
	}

	[HttpGet("user/{userId}")]
	public async Task<ActionResult<List<BoardResponseDTO>>> GetBoardsByUser(string userId)
    {
        try
        {
            var boards = await _boardService.GetBoardsByUserAsync(userId);

            var response = boards.Select(b => new BoardResponseDTO(
                b.Id,
                b.Userid,
                b.Selectednumbers,
                b.Timestamp,
                b.Winner,
                _boardService.CalculateBoardPrice(b.Selectednumbers.Count)
            )).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting boards for user {UserId}", userId);
            return StatusCode(500, "An error occurred while getting boards");
        }
    }

	[HttpGet("all")]
	[Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<BoardResponseDTO>>> GetAllBoards()
    {
        try
        {
            var boards = await _boardService.GetAllBoardsAsync();

            var response = boards.Select(b => new BoardResponseDTO(
                b.Id,
                b.Userid,
                b.Selectednumbers,
                b.Timestamp,
                b.Winner,
                _boardService.CalculateBoardPrice(b.Selectednumbers.Count)
            )).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all boards");
            return StatusCode(500, "An error occurred while getting boards");
        }
    }
	
	[HttpGet("{boardId}")]
	public async Task<ActionResult<BoardResponseDTO>> GetBoardById(string boardId)
	{
		try
		{
			var board = await _boardService.GetBoardByIdAsync(boardId);
			
			if (board == null)
			{
				return NotFound("Board not found");
			}
			var response = new BoardResponseDTO(
                board.Id,
                board.Userid,
                board.Selectednumbers,
                board.Timestamp,
                board.Winner,
                _boardService.CalculateBoardPrice(board.Selectednumbers.Count));

            return Ok(response);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting board {BoardId}", boardId);
            return StatusCode(500, "An error occurred while getting the board");
		}
	}
	
	[HttpDelete("{boardId}")]
    public async Task<ActionResult> DeleteBoard(string boardId)
    {
        try
        {
            await _boardService.DeleteBoardAsync(boardId);
            return Ok(new { Message = "Board deleted successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting board {BoardId}", boardId);
            return StatusCode(500, "An error occurred while deleting the board");
        }
	}
	
	[HttpPost("validate")]
    public async Task<ActionResult<object>> ValidateBoard([FromBody] ValidateBoardDTO dto)
    {
        try
        {
            var result = await _boardService.ValidateBoardAsync(dto.SelectedNumbers);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating board");
            return StatusCode(500, "An error occurred while validating the board");
        }
    }

}


