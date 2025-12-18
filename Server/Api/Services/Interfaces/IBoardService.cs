using Api.Models;
using DataAccess;

namespace Api.Services.Interfaces;

public interface IBoardService
{
    Task<Board> CreateBoardAsync(CreateBoardDTO Dto);
    Task<List<Board>> GetBoardsByUserAsync(string userId);
    Task<List<Board>> GetActiveBoardsByUserAsync(string userId);
    Task<List<Board>> GetAllBoardsAsync();
    Task<Board?> GetBoardByIdAsync(string boardId);
    Task<bool> DeleteBoardAsync(string boardId);
    Task<object> ValidateBoardAsync(List<int> selectedNumbers);
    decimal CalculateBoardPrice(int numberOfFields);
    Task<Board> ToggleRepeatForBoardAsync(string boardId, bool repeat);
    Task<List<BoardHistoryResponseDTO>> GetBoardsForGameAsync(string gameId);
}