using Api.Models;
using DataAccess;

namespace Api.Services.Interfaces;

public interface IBoardService
{
    Task<Board> CreateBoardAsync(CreateBoardDTO Dto);
    Task<List<Board>> GetBoardsByUserAsync(string userId);
    Task<List<Board>> GetAllBoardsAsync();
    Task<Board?> GetBoardByIdAsync(string boardId);
    Task<bool> DeleteBoardAsync(string boardId);
    Task<object> ValidateBoardAsync(List<int> selectedNumbers);
    decimal CalculateBoardPrice(int numberOfFields);
}