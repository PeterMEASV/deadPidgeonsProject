using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public record CreateBoardDTO(
    [Required]
    string UserId,
    [Required]
    [Length(5,8)]
    List<int> SelectedNumbers,
    Boolean Repeat
);

//kun sendes til client, ingen brug for validation.
public record BoardResponseDTO(
    string Id,
    string UserId,
    List<int> SelectedNumbers,
    DateTime Timestamp,
    bool Winner,
    decimal Price,
    Boolean repeat
);

public record ValidateBoardDTO(
    List<int> SelectedNumbers
);

public record BoardHistoryDTO(
	string BoardId,	
	string UserId,
	List<int> SelectedNumbers,
	bool Winner,
	decimal Price,
	string Weeknumber,
	List<int> WinningNumbers,
	DateTime DrawDate
);
	
