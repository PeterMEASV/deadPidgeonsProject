namespace Api.Models;

public record CreateBoardDTO(
    string UserId,
    List<int> SelectedNumbers,
    int RepeatForWeeks = 1
);

public record BoardResponseDTO(
    string Id,
    string UserId,
    List<int> SelectedNumbers,
    DateTime Timestamp,
    bool Winner,
    decimal Price
);

public record ValidateBoardDTO(
    List<int> SelectedNumbers
);