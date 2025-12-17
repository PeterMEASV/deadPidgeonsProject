using System.ComponentModel.DataAnnotations;

namespace Api.Models;
    
public record DrawWinningNumbersDTO(
    [Length(3, 3)]
    List<int> WinningNumbers);

//kun sendt til client, ingen brug for validation.
public record GameResponseDTO(
    string Id,
    string Weeknumber,
    List<int> WinningNumbers,
    DateTime DrawDate,
    bool IsActive,
    int TotalBoards,
    int TotalWinners);

//kun sendt til client, ingen brug for validation.
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
        