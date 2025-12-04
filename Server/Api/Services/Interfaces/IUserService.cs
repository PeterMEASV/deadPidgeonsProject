using Api.Models;
using DataAccess;

namespace Api.Services.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(string id);
    Task<User> CreateUserAsync(CreateUserDTO userDto);
    Task<User> UpdateUserAsync(string id, UpdateUserDTO updateDto);
    Task<bool> DeleteUserAsync(string id);
    Task<object> GetUserDetailsAsync(string id);
    Task<User> ToggleUserActiveStatusAsync(string id);
    Task<User> SetUserActiveStatusAsync(string id, bool isActive);
    Task<User> SetUserAdminStatusAsync(string id, bool isAdmin);

}