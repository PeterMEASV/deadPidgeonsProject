using System.Security.Claims;
using Api.Models;
using DataAccess;

namespace Api.Services.Interfaces;

public interface IAuthService
{
    Task<User?> LoginAsync(LoginDTO loginDto);
    User? GetUserInfo(ClaimsPrincipal principal);
}