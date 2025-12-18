using Api.Models;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Api.Services.Classes;

public class GameService(MyDbContext context, ILogger<GameService> logger, IHistoryService historyService, IBoardService boardService) : IGameService
{
    public async Task<Game> CreateGameAsync()
    {
        //tilføjet transactions for at fixe selectednumber komme på nye spil.
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var nextWeekNumber = "1";
            logger.LogInformation("Creating new game for the Week");

            // Deactivate any active games
            var activeGame = await context.Games
                .Include(g => g.Boards)
                .FirstOrDefaultAsync(g => g.Isactive);

            if (activeGame != null)
            {
                if (!int.TryParse(activeGame.Weeknumber, out var currentWeekNumber))
                {
                    throw new InvalidOperationException(
                        $"Active game's Weeknumber ('{activeGame.Weeknumber}') is not a valid integer, cannot increment."
                    );
                }

                nextWeekNumber = (currentWeekNumber + 1).ToString();
                activeGame.Isactive = false;
                logger.LogInformation("Deactivated game {GameId}", activeGame.Id);
            }

            // Its Creating time
            var newGame = new Game
            {
                Id = Guid.NewGuid().ToString(),
                Weeknumber = nextWeekNumber,
                Winningnumbers = new List<int>(),
                Drawdate = DateTime.Now,
                Isactive = true,
                Timestamp = DateTime.Now
            };

            context.Games.Add(newGame);
            await context.SaveChangesAsync();
            
            
            if (activeGame != null)
            {
                var boardsToRepeat = activeGame.Boards.Where(b => b.Repeat).ToList();
                foreach (var repeatingBoard in boardsToRepeat)
                {
            var newBoard = new CreateBoardDTO
            (
               repeatingBoard.Userid,
                repeatingBoard.Selectednumbers,
                true
            );
            
                    await boardService.CreateBoardAsync(newBoard, true);
                }
            }

            await transaction.CommitAsync();
            
            logger.LogInformation("Created new game {GameId} for Week {WeekNumber}", newGame.Id, newGame.Weeknumber);
            await historyService.CreateLog("Successfully created new game (ID: " + newGame.Id + ", Week: " + newGame.Weeknumber + ")");

            return newGame;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Game?> GetCurrentGameAsync()
    {
        logger.LogInformation("Getting Current Game");

        return await context.Games
            .Include(g => g.Boards)
            .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(g => g.Isactive);
    }

    public async Task<Game> DrawWinningNumbersAsync(DrawWinningNumbersDTO dto)
    {
        logger.LogInformation("Drawing winning numbers: {Numbers}", string.Join(",", dto.WinningNumbers));

        if (dto.WinningNumbers == null || dto.WinningNumbers.Count != 3)
        {
            throw new ArgumentException("Pick 3 numbers");
        }
        
        if (dto.WinningNumbers.Distinct().Count() != 3)
        {
            throw new ArgumentException("Winning numbers must be unique");
        }

        var currentGame = await context.Games
            .Include(g => g.Boards)
            .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(g => g.Isactive);

        if (currentGame == null)
        {
            throw new InvalidOperationException("No active game found");
        }
        
        if (currentGame.Winningnumbers.Any())
        {
            throw new InvalidOperationException("Winning numbers have already been drawn for this game");
        }

        currentGame.Winningnumbers = dto.WinningNumbers;
        currentGame.Drawdate = DateTime.Now;

        int winnerCount = 0;
        foreach (var board in currentGame.Boards)
        {
            bool IsWinner = dto.WinningNumbers.All(wn => board.Selectednumbers.Contains(wn));

            if (IsWinner)
            {
                board.Winner = true;
                winnerCount++;
                logger.LogInformation("Board {BoardId} is a winner! User:{UserId}", board.Id, board.Userid);
            }
            else
            {
                board.Winner = false;
            }
        }
        
        await context.SaveChangesAsync();

        logger.LogInformation("Drew winning numbers for game {GameId}. Total winners: {WinnerCount}", currentGame.Id, winnerCount);
        await historyService.CreateLog("Successfully drew winning numbers for game (ID: " + currentGame.Id + ", total winners: " + winnerCount + ")");

        return currentGame;
    }

    public async Task<object> GetCurrentGameDetailsAsync()
    {
        logger.LogInformation("Getting current game details");

        var currentGame = await context.Games
            .Include(g => g.Boards)
            .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(g => g.Isactive);
        
        if (currentGame == null)
        {
            return new { Message = "No active game found" };
        }

        var boards = currentGame.Boards.Select(b => new
        {
            b.Id,
            b.Userid,
            UserName = b.User.Firstname + " " + b.User.Lastname,
            b.Selectednumbers,
            b.Timestamp,
            b.Winner
        }).ToList();
        
        return new
        {
            currentGame.Id,
            currentGame.Weeknumber,
            currentGame.Winningnumbers,
            HasWinningNumbers = currentGame.Winningnumbers.Any(),
            currentGame.Drawdate,
            currentGame.Isactive,
            TotalBoards = boards.Count,
            TotalWinners = boards.Count(b => b.Winner),
            Boards = boards
        };
    }
    
    public async Task<List<Game>> GetGameHistoryAsync()
    {
        logger.LogInformation("Getting game history");

        return await context.Games
            .Include(g => g.Boards)
            .ThenInclude(b => b.User)
            .OrderByDescending(g => g.Timestamp)
            .ToListAsync();
    }
    
    public async Task<object> GetGameByIdAsync(string gameId)
    {
        logger.LogInformation("Getting game {GameId}", gameId);

        var game = await context.Games
            .Include(g => g.Boards)
            .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null)
        {
            throw new KeyNotFoundException("Game not found");
        }

        var winningBoards = game.Boards
            .Where(b => b.Winner)
            .Select(b => new
            {
                b.Id,
                b.Userid,
                UserName = b.User.Firstname + " " + b.User.Lastname,
                b.Selectednumbers,
                b.Timestamp
            }).ToList();

        return new
        {
            game.Id,
            game.Weeknumber,
            game.Winningnumbers,
            game.Drawdate,
            game.Isactive,
            TotalBoards = game.Boards.Count,
            TotalWinners = winningBoards.Count,
            WinningBoards = winningBoards,
            AllBoards = game.Boards.Select(b => new
            {
                b.Id,
                b.Userid,
                UserName = b.User.Firstname + " " + b.User.Lastname,
                b.Selectednumbers,
                b.Winner,
                b.Timestamp
            }).ToList()
        };
    }
    
    /*
     //todo: Skal vi bruge dette eller skal vi bare bruge nuværende (+1) implementation?
    private string GetWeekOfYear(DateTime date)
    {
        var calendar = CultureInfo.CurrentCulture.Calendar;
        var week = calendar.GetWeekOfYear(date,
            CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday);
        return $"{date.Year}-W{week:00}";
    }
    */
    
}