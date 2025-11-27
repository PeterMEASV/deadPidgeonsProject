using Api.Models;
using DataAccess;

namespace Api.Services.Interfaces;

public interface IBalanceService
{
    Task<Balancelog> SubmitDepositAsync(SubmitDepositDTO dto);
    Task<List<Balancelog>> GetAllTransactionsAsync();
    Task<List<Balancelog>> GetUserTransactionsAsync(string userId);
    Task<object> GetUserBalanceAsync(string userId);
}