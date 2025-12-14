using System.Security.Claims;
using Api.Models;
using Api.Security;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Api.Services.Classes;

public class AuthService(
    MyDbContext context, 
    ILogger<AuthService> logger, 
    KonciousArgon2idPasswordHasher passwordHasher) : IAuthService
{
    public async Task<User?> LoginAsync(LoginDTO loginDto)
    {
        logger.LogInformation("Login attempt for email {Email}", loginDto.Email);

        // Check if user exists
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null)
        {
            logger.LogWarning("Login failed: User with email {Email} not found", loginDto.Email);
            return null;
        }

        // Verify password
        var result = passwordHasher.VerifyHashedPassword(null, user.Password, loginDto.Password);
        
        if (result != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
        {
            logger.LogWarning("Login failed: Invalid password");
            return null;
        }

        // TODO: Decied if we want to allow inactive users to login. For now we dont
        if (!user.Isactive && !user.Isadmin) 
        {
            logger.LogWarning("Login failed: User {Email} is inactive", loginDto.Email);
            return null;
        }

        logger.LogInformation("Login successful for user {UserId} - {Email}", user.Id, user.Email);
        return user;
    }
    
    public User? GetUserInfo(ClaimsPrincipal principal)
    {
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var userId = principal.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        return Queryable.SingleOrDefault(
            context.Users.AsNoTracking(),
            user => user.Id == userId
        );
    }
    
}