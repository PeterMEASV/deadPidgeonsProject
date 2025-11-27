
using Api.Models;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Classes;

public class BalanceService(MyDbContext context, ILogger<BalanceService> logger) : IBalanceService
{
    public async Task<Balancelog> SubmitDepositAsync(SubmitDepositDTO dto)
    {
        logger.LogInformation("Submitting deposit for user {UserId}: {Amount} DKK", dto.UserId, dto.Amount);

        // Validate
        var user = await context.Users.FindAsync(dto.UserId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (dto.Amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than 0");
        }

        if (string.IsNullOrWhiteSpace(dto.TransactionNumber))
        {
            throw new ArgumentException("Transaction number is required");
        }

        // Create transaction log
        var transaction = new Balancelog
        {
            Userid = dto.UserId,
            Amount = dto.Amount,
            Transactionnumber = dto.TransactionNumber,
            Timestamp = DateTime.Now
        };

        context.Balancelogs.Add(transaction);
        
        // Add balance immediately (since we don't have approval system yet)
        user.Balance += dto.Amount;
        
        await context.SaveChangesAsync();

        logger.LogInformation("Deposit submitted: {TransactionId} for {Amount} DKK", 
            transaction.Id, dto.Amount);

        return transaction;
    }

    public async Task<List<Balancelog>> GetAllTransactionsAsync()
    {
        logger.LogInformation("Getting all balance transactions");

        return await context.Balancelogs
            .Include(bl => bl.User)
            .OrderByDescending(bl => bl.Timestamp)
            .ToListAsync();
    }

    public async Task<List<Balancelog>> GetUserTransactionsAsync(string userId)
    {
        logger.LogInformation("Getting transactions for user {UserId}", userId);

        return await context.Balancelogs
            .Where(bl => bl.Userid == userId)
            .OrderByDescending(bl => bl.Timestamp)
            .ToListAsync();
    }

    public async Task<object> GetUserBalanceAsync(string userId)
    {
        logger.LogInformation("Getting balance for user {UserId}", userId);

        var user = await context.Users
            .Include(u => u.Balancelogs)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return new
        {
            UserId = user.Id,
            UserName = user.Firstname + " " + user.Lastname,
            CurrentBalance = user.Balance,
            TotalDeposits = user.Balancelogs.Sum(bl => bl.Amount),
            TransactionCount = user.Balancelogs.Count,
            RecentTransactions = user.Balancelogs
                .OrderByDescending(bl => bl.Timestamp)
                .Take(10)
                .Select(bl => new
                {
                    bl.Id,
                    bl.Amount,
                    bl.Transactionnumber,
                    bl.Timestamp
                })
                .ToList()
        };
    }
}
