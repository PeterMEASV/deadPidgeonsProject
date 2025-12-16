using DataAccess;

namespace Api.Services.Interfaces;

public interface IHistoryService
{
    Task<Historylog> CreateLog(string content);
    Task<List<Historylog>> GetAllLogsAsync();
    Task DeleteLog(string logId);
    
}