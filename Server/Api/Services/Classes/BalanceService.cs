using Api.Models;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Classes;

public class BalanceService(MyDbContext context, ILogger<BalanceService> logger, IHistoryService historyService) : IBalanceService
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
            Timestamp = DateTime.Now,
            Approved = false 
        };

        context.Balancelogs.Add(transaction);
        
        await context.SaveChangesAsync();

        logger.LogInformation("Deposit submitted (PENDING): {TransactionId} for {Amount} DKK", 
            transaction.Id, dto.Amount);
        await historyService.CreateLog("User "+ dto.UserId + "Successfully submitted new deposit (ID: " + transaction.Id + ", Amount: " + dto.Amount + " DKK)");

        return transaction;
    }

    public async Task<Balancelog> ApproveTransactionAsync(int transactionId)
    {
        logger.LogInformation("Approving transaction {TransactionId}", transactionId);

        var transaction = await context.Balancelogs
            .Include(bl => bl.User)
            .FirstOrDefaultAsync(bl => bl.Id == transactionId);

        if (transaction == null)
        {
            throw new KeyNotFoundException("Transaction not found");
        }

        if (transaction.Approved)
        {
            throw new InvalidOperationException("Transaction already approved");
        }

        // Approve transaction
        transaction.Approved = true;

        // Add balance to user
        transaction.User.Balance += transaction.Amount;

        await context.SaveChangesAsync();

        logger.LogInformation("Transaction {TransactionId} approved. Added {Amount} DKK to user {UserId}",
            transactionId, transaction.Amount, transaction.Userid);
        await historyService.CreateLog("Successfully approved deposit (ID: " + transaction.Id + ", Amount: " + transaction.Amount + " DKK) for user " + transaction.Userid);

        return transaction;
    }

    public async Task<List<Balancelog>> GetPendingTransactionsAsync()
    {
        logger.LogInformation("Getting pending transactions");

        return await context.Balancelogs
            .Include(bl => bl.User)
            .Where(bl => !bl.Approved)
            .OrderBy(bl => bl.Timestamp)
            .ToListAsync();
    }

    public async Task<List<Balancelog>> GetApprovedTransactionsAsync()
    {
        logger.LogInformation("Getting approved transactions");

        return await context.Balancelogs
            .Include(bl => bl.User)
            .Where(bl => bl.Approved)
            .OrderByDescending(bl => bl.Timestamp)
            .ToListAsync();
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
            TotalDeposits = user.Balancelogs.Where(bl => bl.Approved).Sum(bl => bl.Amount),
            PendingDeposits = user.Balancelogs.Where(bl => !bl.Approved).Sum(bl => bl.Amount),
            TransactionCount = user.Balancelogs.Count,
            ApprovedCount = user.Balancelogs.Count(bl => bl.Approved),
            PendingCount = user.Balancelogs.Count(bl => !bl.Approved),
            RecentTransactions = user.Balancelogs
                .OrderByDescending(bl => bl.Timestamp)
                .Take(10)
                .Select(bl => new
                {
                    bl.Id,
                    bl.Amount,
                    bl.Transactionnumber,
                    bl.Timestamp,
                    bl.Approved,
                    Status = bl.Approved ? "Approved" : "Pending"
                })
                .ToList()
        };
    }
}
