using Api.Models;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Classes;

public class BoardService(MyDbContext context, ILogger<BoardService> logger, IHistoryService historyService) : IBoardService
{
    public decimal CalculateBoardPrice(int numberOfFields)
    {
        return numberOfFields switch
        {
            5 => 20m,
            6 => 40m,
            7 => 80m,
            8 => 160m,
            _ => throw new ArgumentException("Please select atleast 5 Numbers")
        };
    }
    
    
    //todo hvorfor ikke bare bruge async? hvorfor har vi 2?
    private bool ValidateBoard(List<int>? selectedNumbers, out string errorMessage)
    {
        errorMessage = string.Empty;
        
        if (selectedNumbers == null || selectedNumbers.Count < 5 || selectedNumbers.Count > 8)
        {
            errorMessage = "Please select atleast 5 Numbers and Max 8 Numbers";
            return false;
        }

        if (selectedNumbers.Any(n => n < 1 || n > 16))
        {
            errorMessage = "Numbers must be between 1 and 16";
            return false;
        }

        if (selectedNumbers.Distinct().Count() != selectedNumbers.Count)
        {
            errorMessage = "You cant select the same number twice";
            return false;
        }
        
        return true;
    }

    public async Task<Board> CreateBoardAsync(CreateBoardDTO dto)
    {
        logger.LogInformation("Creating board for user {UserId}", dto.UserId);
        
        if (!ValidateBoard(dto.SelectedNumbers, out string validationError))
        {
            throw new ArgumentException(validationError);
        }

        // Get current active game
        var currentGame = await context.Games
            .FirstOrDefaultAsync(g => g.Isactive);

        if (currentGame == null)
        {
            throw new InvalidOperationException("No active game available. Please contact administrator.");
        }

        // Check if winning numbers already drawn
        if (currentGame.Winningnumbers != null && currentGame.Winningnumbers.Any())
        {
            throw new InvalidOperationException("Cannot purchase boards after winning numbers have been drawn.");
        }
        
        var user = await context.Users.FindAsync(dto.UserId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (!user.Isactive)
    	{
        throw new InvalidOperationException("User account is inactive. Please contact administrator to activate your account.");
   		}

        // MATH!!
        decimal boardPrice = CalculateBoardPrice(dto.SelectedNumbers.Count);

        if (user.Balance < boardPrice)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }
        

            var board = new Board
            {
                Id = Guid.NewGuid().ToString(),
                Userid = dto.UserId,
                Selectednumbers = new List<int>(dto.SelectedNumbers),
                Timestamp = DateTime.Now,
                Winner = false,
                Gameid = currentGame.Id,
                Repeat = dto.Repeat
            };
            
            context.Boards.Add(board);

        user.Balance -= boardPrice;

        await context.SaveChangesAsync();
        
        logger.LogInformation("Created board for user {UserId}. Total was {Total} DKK",
            dto.UserId, boardPrice);
        await historyService.CreateLog("Successfully created new board (ID: " + board.Id + ", Total: " + boardPrice + " DKK)");

        return board;
    }

    public async Task<List<Board>> GetBoardsByUserAsync(string userId)
    {
        logger.LogInformation("Getting boards for user {UserId}", userId);

        return await context.Boards
            .Where(b => b.Userid == userId)
            .OrderByDescending(b => b.Timestamp)
            .ToListAsync();
    }

    public async Task<List<Board>> GetAllBoardsAsync()
    {
        logger.LogInformation("Getting all boards");

        return await context.Boards
            .Include(b => b.User)
            .OrderByDescending(b => b.Timestamp)
            .ToListAsync();
    }

    public async Task<Board?> GetBoardByIdAsync(string boardId)
    {
        logger.LogInformation("Getting board by id {BoardId}", boardId);
        
        return await context.Boards.FindAsync(boardId);
    }

    public async Task<bool> DeleteBoardAsync(string boardId)
    {
        logger.LogInformation("Deleting board {BoardId}", boardId);
        
        var board = await context.Boards.FindAsync(boardId);
        if (board == null)
        {
            throw new KeyNotFoundException("Board not found");
        }

        var user = await context.Users.FindAsync(board.Userid);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }
        
        // Refund TODO: ???????
        decimal refundAmount = CalculateBoardPrice(board.Selectednumbers.Count);
        user.Balance += refundAmount;

        context.Boards.Remove(board);
        await context.SaveChangesAsync();
        
        logger.LogInformation("Deleted board {BoardId} and refunded {Amount} DKK", boardId, refundAmount);
        await historyService.CreateLog("Successfully deleted board (ID: " + boardId + ", Refund: " + refundAmount + " DKK)");
        
        return true;
    }

    public Task<object> ValidateBoardAsync(List<int>? selectedNumbers)
    {
        logger.LogInformation("Validating board with {Count} numbers", selectedNumbers?.Count ?? 0);

        if (!ValidateBoard(selectedNumbers, out string errorMessage))
        {
            return Task.FromResult<object>(new { isValid = false, errorMessage });
        }

        decimal price = CalculateBoardPrice(selectedNumbers!.Count);
        return Task.FromResult<object>(new
        {
            isValid = true, 
            Price = price, 
            NumberOfFields = selectedNumbers.Count
        });
    }

    public Task<List<Board>> GetBoardsForGameAsync(string gameId)
    {
        logger.LogInformation("Finding board for the game: {game}", gameId);
        return context.Boards.Where(b => b.Gameid == gameId).ToListAsync();
    }
    
    public Task<List<Board>> GetRepeatingBoardForGameAsync(string gameId)
    {
        logger.LogInformation("Finding repeating board for the game: {game}", gameId);
        return context.Boards.Where(b => b.Gameid == gameId && b.Repeat).ToListAsync();
    }
}
