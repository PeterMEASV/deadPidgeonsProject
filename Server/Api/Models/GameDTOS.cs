namespace Api.Models;

public record CreateGameDTO(
    string Weeknumber);
    
public record DrawWinningNumbersDTO(
    List<int> WinningNumbers);

public record GameResponseDTO(
    string Id,
    string Weeknumber,
    List<int> WinningNumbers,
    DateTime DrawDate,
    bool IsActive,
    int TotalBoards,
    int TotalWinners);

public record GameHistoryDTO(
    string Id,
    string Weeknumber,
    List<int> WinningNumbers,
    DateTime DrawDate,
    int TotalBoards,
    int TotalWinners,
    List<WinningBoardDTO> WinningBoards);

public record WinningBoardDTO(
    string BoardId,
    string UserId,
    string UserName,
    List<int> SelectedNumbers);
        