using Api.Models;
using Api.Security;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace xunittests;

public class GameServiceTest(
    IGameService gameService,
    IBoardService boardService,
    MyDbContext context,
    KonciousArgon2idPasswordHasher hasher
)
{
    public async Task<User> CreateTestUser()
    {
        var password = "EASV2025";
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = "easv@test.com",
            Phonenumber = "12345678",
            Password = hasher.HashPassword(null, password),
            Firstname = "EASV",
            Lastname = "Test",
            Balance = 99999,
            Isactive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
        
    }


    // ---------------------------
    // CREATE GAME
    // ---------------------------

    [Fact]
    public async Task CreateGame_FirstGame_CreatesActiveGame()
    {
        var game = await gameService.CreateGameAsync();

        Assert.NotNull(game);
        Assert.True(game.Isactive);
        Assert.Equal("1", game.Weeknumber);
    }

    [Fact]
    public async Task CreateGame_DeactivatesPreviousGame()
    {
        var firstGame = await gameService.CreateGameAsync();
        var secondGame = await gameService.CreateGameAsync();

        var games = await context.Games.ToListAsync();

        Assert.False(games.First(g => g.Id == firstGame.Id).Isactive);
        Assert.True(games.First(g => g.Id == secondGame.Id).Isactive);
        Assert.Equal("2", secondGame.Weeknumber);
    }

    // ---------------------------
    // DRAW WINNING NUMBERS
    // ---------------------------

    [Fact]
    public async Task DrawWinningNumbers_ValidNumbers_SetsWinnersCorrectly()
    {
        var user = await CreateTestUser();
        await gameService.CreateGameAsync();

        await boardService.CreateBoardAsync(new CreateBoardDTO(
            user.Id,
            new List<int> { 1, 2, 3, 4, 5 },
            false
        ),true);

        var dto = new DrawWinningNumbersDTO(new List<int> { 1, 2, 3 });

        var game = await gameService.DrawWinningNumbersAsync(dto);

        var board = game.Boards.First();

        Assert.True(board.Winner);
        Assert.Equal(dto.WinningNumbers, game.Winningnumbers);
    }

    [Fact]
    public async Task DrawWinningNumbers_NoMatch_SetsLoser()
    {
        var user = await CreateTestUser();
        await gameService.CreateGameAsync();

        await boardService.CreateBoardAsync(new CreateBoardDTO(
            user.Id,
            new List<int> { 10, 11, 12, 13, 14 },
            false
        ),true);

        var dto = new DrawWinningNumbersDTO(new List<int> { 1, 2, 3 });

        var game = await gameService.DrawWinningNumbersAsync(dto);

        Assert.False(game.Boards.First().Winner);
    }

    // ---------------------------
    // INVALID INPUT
    // ---------------------------

    [Fact]
    public async Task DrawWinningNumbers_InvalidCount_ThrowsException()
    {
        await gameService.CreateGameAsync();

        var dto = new DrawWinningNumbersDTO(new List<int> { 1, 2 });

        await Assert.ThrowsAsync<ArgumentException>(
            () => gameService.DrawWinningNumbersAsync(dto)
        );
    }

    [Fact]
    public async Task DrawWinningNumbers_DuplicateNumbers_ThrowsException()
    {
        await gameService.CreateGameAsync();

        var dto = new DrawWinningNumbersDTO(new List<int> { 1, 1, 2 });

        await Assert.ThrowsAsync<ArgumentException>(
            () => gameService.DrawWinningNumbersAsync(dto)
        );
    }

    [Fact]
    public async Task DrawWinningNumbers_Twice_ThrowsException()
    {
        await gameService.CreateGameAsync();

        var dto = new DrawWinningNumbersDTO(new List<int> { 1, 2, 3 });

        await gameService.DrawWinningNumbersAsync(dto);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => gameService.DrawWinningNumbersAsync(dto)
        );
    }
}
