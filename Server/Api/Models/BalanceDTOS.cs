namespace Api.Models;

public record SubmitDepositDTO(
    string UserId,
    decimal Amount,
    string TransactionNumber
);

public record BalanceTransactionResponseDTO(
    int Id,
    string UserId,
    decimal Amount,
    string TransactionNumber,
    DateTime Timestamp
);

public record ApproveTransactionDTO(
    int TransactionId
);