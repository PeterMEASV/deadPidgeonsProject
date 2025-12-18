using Api.Models;
using Api.Security;
using Api.Services.Interfaces;
using DataAccess;

namespace xunittests;

public class BoardServiceTest(IBoardService boardService, IGameService gameService, MyDbContext context, ITestOutputHelper outputHelper, KonciousArgon2idPasswordHasher hasher)
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

    [Fact]
    public async Task CalculatePrices_ValidInput()
    {
        Assert.Equal(20m, boardService.CalculateBoardPrice(5));
        Assert.Equal(40m, boardService.CalculateBoardPrice(6));
        Assert.Equal(80m, boardService.CalculateBoardPrice(7));
        Assert.Equal(160m, boardService.CalculateBoardPrice(8));
    }
    
    [Fact]
    public async Task CalculatePrices_InvalidInput()
    {
        Assert.Throws<ArgumentException>(() => boardService.CalculateBoardPrice(4));
        Assert.Throws<ArgumentException>(() => boardService.CalculateBoardPrice(9));
        Assert.Throws<ArgumentException>(() => boardService.CalculateBoardPrice(0));
    }

    [Fact]
    public async Task CreateBoard_ValidData()
    {
        User testUser = await CreateTestUser();
        await gameService.CreateGameAsync();
        
        var board = await boardService.CreateBoardAsync(new CreateBoardDTO(testUser.Id, new List<int> { 1, 2, 3, 4, 5 }, false), true);
        
        Assert.NotNull(board);
        var allBoards = await boardService.GetAllBoardsAsync();
        Assert.Single(allBoards);
        
        var updatedUser = await context.Users.FindAsync(testUser.Id);
        Assert.Equal(99979m, updatedUser.Balance);
    }

    [Fact]
    public async Task CreateBoard_InsufficientFunds()
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = "poor@test.com",
            Phonenumber = "12345678",
            Balance = 5m,
            Isactive = true,
            Password = "hashed",
            Firstname = "Poor",
            Lastname = "User"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        await gameService.CreateGameAsync();

        var dto = new CreateBoardDTO(user.Id, new List<int> { 1, 2, 3, 4, 5 }, false);
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            boardService.CreateBoardAsync(dto, true));
    }

    [Fact]
    public async Task CreateBoard_InvalidNumbers()
    {
        User testUser = await CreateTestUser();
        await gameService.CreateGameAsync();

        // Too few numbers
        var dto = new CreateBoardDTO(testUser.Id, new List<int> { 1, 2, 3 }, false);
        await Assert.ThrowsAsync<ArgumentException>(() => boardService.CreateBoardAsync(dto, true));

        // Duplicate numbers
        var dto2 = new CreateBoardDTO(testUser.Id, new List<int> { 1, 1, 2, 3, 4 }, false);
        await Assert.ThrowsAsync<ArgumentException>(() => boardService.CreateBoardAsync(dto2, true));

        // Numbers out of range
        var dto3 = new CreateBoardDTO(testUser.Id, new List<int> { 1, 2, 3, 4, 99 }, false);
        await Assert.ThrowsAsync<ArgumentException>(() => boardService.CreateBoardAsync(dto3, true));
    }

    [Fact]
    public async Task CreateBoard_NoActiveGame()
    {
        User testUser = await CreateTestUser();

        var dto = new CreateBoardDTO(testUser.Id, new List<int> { 1, 2, 3, 4, 5 }, false);
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => boardService.CreateBoardAsync(dto, true));
    }

    [Fact]
    public async Task ToggleRepeat_UpdatesStatus()
    {
        User testUser = await CreateTestUser();
        await gameService.CreateGameAsync();
        var board = await boardService.CreateBoardAsync(new CreateBoardDTO(testUser.Id, new List<int> { 1, 2, 3, 4, 5 }, false), true);
        
        Assert.False(board.Repeat);
        
        await boardService.ToggleRepeatForBoardAsync(board.Id, true);
        var updatedBoard = await boardService.GetBoardByIdAsync(board.Id);
        
        Assert.True(updatedBoard.Repeat);
    }
}