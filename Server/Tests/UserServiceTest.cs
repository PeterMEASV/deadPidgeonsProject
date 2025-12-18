using Api.Models;
using Api.Security;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace xunittests;

public class UserServiceTest(
    IUserService userService,
    MyDbContext context,
    ITestOutputHelper outputHelper,
    KonciousArgon2idPasswordHasher hasher)
{
    private async Task<User> CreateTestUser(string email = "test@example.com", bool isActive = true, bool isAdmin = false)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            Phonenumber = "12345678",
            Password = hasher.HashPassword(null!, "TestPassword123"),
            Firstname = "Test",
            Lastname = "User",
            Balance = 100,
            Isactive = isActive,
            Isadmin = isAdmin,
            Timestamp = DateTime.UtcNow
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task CreateUserAsync_ValidInput_CreatesUser()
    {
        var createDto = new CreateUserDTO(
            "John",
            "Doe",
            "john.doe@example.com",
            "87654321",
            "Password123"
        );
        
        var result = await userService.CreateUserAsync(createDto);
        
        Assert.NotNull(result);
        Assert.Equal(createDto.firstname, result.Firstname);
        Assert.Equal(createDto.lastname, result.Lastname);
        Assert.Equal(createDto.email, result.Email);
        Assert.Equal(createDto.phonenumber, result.Phonenumber);
        Assert.Equal(0, result.Balance);
        Assert.True(result.Isactive);
        Assert.False(result.Isadmin);
        
        outputHelper.WriteLine($"Created user: {result.Id} - {result.Email}");
    }

    [Fact]
    public async Task CreateUserAsync_DuplicateEmail_ThrowsException()
    {
        await CreateTestUser("duplicate@example.com");
        var createDto = new CreateUserDTO(
            "Jane",
            "Doe",
            "duplicate@example.com",
            "87654321",
            "Password123"
        );
        
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => userService.CreateUserAsync(createDto)
        );
    }

    [Fact]
    public async Task CreateUserAsync_EmptyFields_ThrowsException()
    {
        var createDto = new CreateUserDTO(
            "",
            "Doe",
            "john.doe@example.com",
            "87654321",
            "Password123"
        );
        
        await Assert.ThrowsAsync<ArgumentException>(
            () => userService.CreateUserAsync(createDto)
        );
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        await CreateTestUser("user1@example.com");
        await CreateTestUser("user2@example.com");
        await CreateTestUser("user3@example.com");
        
        var result = await userService.GetAllUsersAsync();
        
        Assert.NotNull(result);
        Assert.True(result.Count >= 3);
        
        outputHelper.WriteLine($"Total users: {result.Count}");
    }

    [Fact]
    public async Task GetUserByIdAsync_ValidId_ReturnsUser()
    {
        var user = await CreateTestUser("findme@example.com");
        
        var result = await userService.GetUserByIdAsync(user.Id);
        
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_InvalidId_ReturnsNull()
    {
        var result = await userService.GetUserByIdAsync("non-existent-id");
        
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUserAsync_ValidInput_UpdatesUser()
    {
        var user = await CreateTestUser("update@example.com");
        var updateDto = new UpdateUserDTO(
            "Updated",
            "Name",
            "updated@example.com",
            "99999999",
            null
        );
        
        var result = await userService.UpdateUserAsync(user.Id, updateDto);
        
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Firstname);
        Assert.Equal("Name", result.Lastname);
        Assert.Equal("updated@example.com", result.Email);
        Assert.Equal("99999999", result.Phonenumber);
    }

    [Fact]
    public async Task UpdateUserAsync_WithPassword_UpdatesPassword()
    {
        var user = await CreateTestUser("updatepwd@example.com");
        var oldPassword = user.Password;
        
        var updateDto = new UpdateUserDTO(
            user.Firstname,
            user.Lastname,
            user.Email,
            user.Phonenumber,
            "NewPassword123"
        );
        
        var result = await userService.UpdateUserAsync(user.Id, updateDto);
        
        var updatedUser = await context.Users.FindAsync(user.Id);

        
        Assert.NotNull(result);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(oldPassword, updatedUser.Password);
    }

    [Fact]
    public async Task UpdateUserAsync_DuplicateEmail_ThrowsException()
    {
        await CreateTestUser("existing@example.com");
        var userToUpdate = await CreateTestUser("toupdate@example.com");
        
        var updateDto = new UpdateUserDTO(
            userToUpdate.Firstname,
            userToUpdate.Lastname,
            "existing@example.com",
            userToUpdate.Phonenumber,
            null
        );
        
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => userService.UpdateUserAsync(userToUpdate.Id, updateDto)
        );
    }

    [Fact]
    public async Task UpdateUserAsync_NonExistentUser_ThrowsException()
    {
        var updateDto = new UpdateUserDTO(
            "Test",
            "User",
            "test@example.com",
            "12345678",
            null
        );
        
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => userService.UpdateUserAsync("non-existent-id", updateDto)
        );
    }

    [Fact]
    public async Task ToggleUserActiveStatusAsync_TogglesStatus()
    {
        
        var user = await CreateTestUser("toggle@example.com", isActive: true);
        
        var result = await userService.ToggleUserActiveStatusAsync(user.Id);
        
        Assert.False(result.Isactive);
        
        result = await userService.ToggleUserActiveStatusAsync(user.Id);
        Assert.True(result.Isactive);
    }

    [Fact]
    public async Task SetUserActiveStatusAsync_SetsStatus()
    {
        var user = await CreateTestUser("setstatus@example.com", isActive: true);
        
        var result = await userService.SetUserActiveStatusAsync(user.Id, false);
        
        Assert.False(result.Isactive);
    }

    [Fact]
    public async Task SetUserAdminStatusAsync_SetsAdminStatus()
    {
        var user = await CreateTestUser("setadmin@example.com", isAdmin: false);
        
        var result = await userService.SetUserAdminStatusAsync(user.Id, true);
        
        Assert.True(result.Isadmin);
    }
    
    [Fact]
    public async Task GetUserDetailsAsync_ValidUser_ReturnsDetails()
    {
        var user = await CreateTestUser("details@example.com");
        
        var result = await userService.GetUserDetailsAsync(user.Id);
        
        Assert.NotNull(result);
        
        var resultType = result.GetType();
        var idProp = resultType.GetProperty("Id");
        var emailProp = resultType.GetProperty("Email");
        
        Assert.NotNull(idProp);
        Assert.NotNull(emailProp);
        Assert.Equal(user.Id, idProp.GetValue(result)?.ToString());
        Assert.Equal(user.Email, emailProp.GetValue(result)?.ToString());
    }

    [Fact]
    public async Task GetUserDetailsAsync_NonExistentUser_ThrowsException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => userService.GetUserDetailsAsync("non-existent-id")
        );
    }

    [Fact]
    public async Task FindUsersByPhoneNumber_ValidNumber_ReturnsUsers()
    {
        await CreateTestUser("phone1@example.com");
        var user2 = await CreateTestUser("phone2@example.com");
        user2.Phonenumber = "99887766";
        await context.SaveChangesAsync();
        
        var result = await userService.FindUsersByPhoneNumber("12345678");
        
        Assert.NotEmpty(result);
        Assert.Contains(result, u => u.Phonenumber.Contains("12345678"));
    }

    [Fact]
    public async Task FindUsersByPhoneNumber_EmptyString_ReturnsEmptyList()
    {
        var result = await userService.FindUsersByPhoneNumber("");
        
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindUsersByPhoneNumber_NoMatches_ReturnsEmptyList()
    {
        await CreateTestUser("nophone@example.com");
        
        var result = await userService.FindUsersByPhoneNumber("00000000");
        
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindUsersByPhoneNumber_PartialMatch_ReturnsUsers()
    {
        var user = await CreateTestUser("partial@example.com");
        user.Phonenumber = "12345678";
        await context.SaveChangesAsync();
        
        var result = await userService.FindUsersByPhoneNumber("1234");
        
        Assert.NotEmpty(result);
        Assert.Contains(result, u => u.Phonenumber.Contains("1234"));
    }
}