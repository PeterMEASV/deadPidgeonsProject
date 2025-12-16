using Api.Services.Interfaces;
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
    
}