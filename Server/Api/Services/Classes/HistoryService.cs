using Api.Services.Interfaces;
using Api.Models;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Classes;

public class HistoryService(MyDbContext context, ILogger<HistoryService> logger) : IHistoryService
{

    public async Task<Historylog> CreateLog(string content)
    {
        logger.LogInformation("Creating history log");

        var createdLog = new Historylog
        {
            Id = Guid.NewGuid().ToString(),
            Content = content,
            Timestamp = DateTime.Now
        };
        context.Historylogs.Add(createdLog);
        await context.SaveChangesAsync();
        return createdLog;
    }

    public async Task<List<Historylog>> GetAllLogsAsync()
    {
        logger.LogInformation("Getting all history logs");

        return await context.Historylogs.OrderByDescending(b => b.Timestamp).ToListAsync();
    }

    public async Task DeleteLog(string logId)
    {
        logger.LogInformation("Deleting history log {LogId}", logId);

        var log = await context.Historylogs.FindAsync(logId);
        if (log == null) return;
        context.Historylogs.Remove(log);
        await context.SaveChangesAsync();
    }

    public async Task<List<BoardHistoryDTO>> GetUserBoardHistoryAsync(string userId)
    {
        logger.LogInformation("Getting board history for user {UserId}", userId);
        
        var boards = await context.Boards
            .Include(b => b.Game)
            .Where(b => b.Userid == userId)
            .OrderByDescending(b => b.Timestamp)
            .ToListAsync();

        var boardHistory = boards.Select(board => new BoardHistoryDTO(
            BoardId: board.Id,
            UserId: board.Userid,
            SelectedNumbers: board.Selectednumbers,
            Winner: board.Winner,
            Price: CalculateBoardPrice(board.Selectednumbers.Count),
            Weeknumber: board.Game?.Weeknumber ?? "N/A",
            WinningNumbers: board.Game?.Winningnumbers ?? new List<int>(),
            DrawDate: board.Game?.Drawdate ?? DateTime.MinValue
        )).ToList();

        return boardHistory;
    }

    private decimal CalculateBoardPrice(int numberOfFields)
    {
        return numberOfFields switch
        {
            5 => 20m,
            6 => 40m,
            7 => 80m,
            8 => 160m,
            _ => 0m
        };
    }



}