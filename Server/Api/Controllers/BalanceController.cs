using Api.Models;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class BalanceController : ControllerBase
{
    private readonly IBalanceService _balanceService;
    private readonly ILogger<BalanceController> _logger;
    
    public BalanceController(IBalanceService balanceService, ILogger<BalanceController> logger)
    {
        _balanceService = balanceService;
        _logger = logger;
    }


    [HttpPost("deposit")]
    public async Task<ActionResult<BalanceTransactionResponseDTO>> SubmitDeposit([FromBody] SubmitDepositDTO dto)
    {
        try
        {
            var transaction = await _balanceService.SubmitDepositAsync(dto);

            var response = new BalanceTransactionResponseDTO(
                transaction.Id,
                transaction.Userid,
                transaction.Amount,
                transaction.Transactionnumber,
                transaction.Timestamp);
            
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting deposit");
            return StatusCode(500, "An error occurred while submitting the deposit");
        }
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<List<BalanceTransactionResponseDTO>>> GetAllTransactions()
    {
        try
        {
            var transactions = await _balanceService.GetAllTransactionsAsync();

            var response = transactions.Select(t => new BalanceTransactionResponseDTO(
                t.Id,
                t.Userid,
                t.Amount,
                t.Transactionnumber,
                t.Timestamp)).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all transactions");
            return StatusCode(500, "An error occurred while getting all transactions");
        }
    }
    
    [HttpGet("user/{userId}/transactions")]
    public async Task<ActionResult<List<BalanceTransactionResponseDTO>>> GetUserTransactions(string userId)
    {
        try
        {
            var transactions = await _balanceService.GetUserTransactionsAsync(userId);

            var response = transactions.Select(t => new BalanceTransactionResponseDTO(
                t.Id,
                t.Userid,
                t.Amount,
                t.Transactionnumber,
                t.Timestamp
            )).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user transactions");
            return StatusCode(500, "An error occurred while getting transactions");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<object>> GetUserBalance(string userId)
    {
        try
        {
            var balance = await _balanceService.GetUserBalanceAsync(userId);
            return Ok(balance);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user balance");
            return StatusCode(500, "An error occurred while getting user balance");
        }
    }
}