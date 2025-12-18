using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public record SubmitDepositDTO(
    [Required]
    string UserId,
    [Range(1,10000)]
    decimal Amount,
    [Required]
    [Length(1, 50)]
    string TransactionNumber
);

// bliver kun sendt til client, behøver ikke validation.
public record BalanceTransactionResponseDTO(
    int Id,
    string UserId,
    decimal Amount,
    string TransactionNumber,
    DateTime Timestamp
);

public record ApproveTransactionDTO(
    [Required]
    int TransactionId
);

public record UserBalanceResponseDTO(
    string UserId,
    string UserName,
    decimal CurrentBalance,
    decimal TotalDeposits,
    decimal PendingDeposits,
    int TransactionCount,
    int ApprovedCount,
    int PendingCount,
    List<BalanceTransactionResponseDTO> RecentTransactions
);
