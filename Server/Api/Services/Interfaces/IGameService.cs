using Api.Models;
using DataAccess;

namespace Api.Services.Interfaces;
public interface IGameService
{
    Task<Game> CreateGameAsync(CreateGameDTO dto);
    Task<Game?> GetCurrentGameAsync();
    Task<Game> DrawWinningNumbersAsync(DrawWinningNumbersDTO dto);
    Task<object> GetCurrentGameDetailsAsync();
    Task<List<Game>> GetGameHistoryAsync();
    Task<object> GetGameByIdAsync(string gameId);
}