using Api.Models;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Api.Services.Classes;

public class GameService(MyDbContext context, ILogger<GameService> logger) : IGameService
{
    public async Task<Game> CreateGameAsync(CreateGameDTO dto)
    {
        logger.LogInformation("Creating new game for Week {WeekNumber}", dto.Weeknumber);

        // Deactivate any active games
        var activeGames = await context.Games
            .Where(g => g.Isactive)
            .ToListAsync();

        foreach (var game in activeGames)
        {
            game.Isactive = false;
            logger.LogInformation("Deactivated game {GameId}", game.Id);
        }
        
        // Its Creating time
        var newGame = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Weeknumber = dto.Weeknumber,
            Winningnumbers = new List<int>(),
            Drawdate = DateTime.Now,
            Isactive = true,
            Timestamp = DateTime.Now
        };
        
        context.Games.Add(newGame);
        await context.SaveChangesAsync();

        logger.LogInformation("Created new game {GameId} for Week {WeekNumber}", newGame.Id, newGame.Weeknumber);

        return newGame;
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
    private string GetWeekOfYear(DateTime date)
    {
        var calendar = CultureInfo.CurrentCulture.Calendar;
        var week = calendar.GetWeekOfYear(date,
            CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday);
        return $"{date.Year}-W{week:00}";
    }
    
}