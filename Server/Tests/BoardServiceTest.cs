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
    
}