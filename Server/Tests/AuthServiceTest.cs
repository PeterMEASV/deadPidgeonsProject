
using System.Security.Authentication;
using Api.Models;
using Api.Security;
using Api.Services.Interfaces;
using DataAccess;

namespace xunittests;

public class AuthServiceTest(IAuthService authService, MyDbContext context, ITestOutputHelper outputHelper, KonciousArgon2idPasswordHasher hasher)
{

    [Fact]
    public async Task Login_ValidCredentials()
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
            Isactive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var result = await authService.LoginAsync(new LoginDTO(user.Email, password));
        
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task Login_InvalidCredentials()
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
            Isactive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        await Assert.ThrowsAsync<InvalidCredentialException>(() => authService.LoginAsync(new LoginDTO("private@test.com", "password")));

    }

}