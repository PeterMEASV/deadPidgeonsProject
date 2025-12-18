using Api.Models;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
                transaction.Timestamp
            );

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

   
    [HttpPost("approve")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BalanceTransactionResponseDTO>> ApproveTransaction([FromBody] ApproveTransactionDTO dto)
    {
        try
        {
            var transaction = await _balanceService.ApproveTransactionAsync(dto.TransactionId);

            var response = new BalanceTransactionResponseDTO(
                transaction.Id,
                transaction.Userid,
                transaction.Amount,
                transaction.Transactionnumber,
                transaction.Timestamp
            );

            return Ok(response);
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
            _logger.LogError(ex, "Error approving transaction");
            return StatusCode(500, "An error occurred while approving the transaction");
        }
    }
    
    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<object>>> GetPendingTransactions()
    {
        try
        {
            var transactions = await _balanceService.GetPendingTransactionsAsync();

            var response = transactions.Select(t => new
            {
                t.Id,
                t.Userid,
                UserName = t.User.Firstname + " " + t.User.Lastname,
                t.Amount,
                t.Transactionnumber,
                t.Timestamp,
                Status = "Pending"
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending transactions");
            return StatusCode(500, "An error occurred while getting pending transactions");
        }
    }
    
    [HttpGet("approved")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<object>>> GetApprovedTransactions()
    {
        try
        {
            var transactions = await _balanceService.GetApprovedTransactionsAsync();

            var response = transactions.Select(t => new
            {
                t.Id,
                t.Userid,
                UserName = t.User.Firstname + " " + t.User.Lastname,
                t.Amount,
                t.Transactionnumber,
                t.Timestamp,
                Status = "Approved"
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting approved transactions");
            return StatusCode(500, "An error occurred while getting approved transactions");
        }
    }
    
    [HttpGet("transactions")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<object>>> GetAllTransactions()
    {
        try
        {
            var transactions = await _balanceService.GetAllTransactionsAsync();

            var response = transactions.Select(t => new
            {
                t.Id,
                t.Userid,
                UserName = t.User.Firstname + " " + t.User.Lastname,
                t.Amount,
                t.Transactionnumber,
                t.Timestamp,
                t.Approved,
                Status = t.Approved ? "Approved" : "Pending"
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all transactions");
            return StatusCode(500, "An error occurred while getting transactions");
        }
    }
    
    [HttpGet("user/{userId}/transactions")]
    public async Task<ActionResult<List<object>>> GetUserTransactions(string userId)
    {
        try
        {
            var transactions = await _balanceService.GetUserTransactionsAsync(userId);

            var response = transactions.Select(t => new
            {
                t.Id,
                t.Amount,
                t.Transactionnumber,
                t.Timestamp,
                t.Approved,
                Status = t.Approved ? "Approved" : "Pending"
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user transactions");
            return StatusCode(500, "An error occurred while getting transactions");
        }
    }
    
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<UserBalanceResponseDTO>> GetUserBalance(string userId)
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
            return StatusCode(500, "An error occurred while getting balance");
        }
    }
}
