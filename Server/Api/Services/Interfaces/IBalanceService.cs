using Api.Models;
using DataAccess;

namespace Api.Services.Interfaces;

public interface IBalanceService
{
    Task<Balancelog> SubmitDepositAsync(SubmitDepositDTO dto);
    Task<Balancelog> ApproveTransactionAsync(int transactionId);
    Task<List<Balancelog>> GetPendingTransactionsAsync();
    Task<List<Balancelog>> GetApprovedTransactionsAsync();
    Task<List<Balancelog>> GetAllTransactionsAsync();
    Task<List<Balancelog>> GetUserTransactionsAsync(string userId);
    Task<UserBalanceResponseDTO> GetUserBalanceAsync(string userId);
}