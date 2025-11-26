
using Api.Models;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Classes;

public class UserService : IUserService
{
    private readonly MyDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(MyDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        _logger.LogInformation("Getting all users");

        return await _context.Users
            .OrderBy(u => u.Lastname)
            .ThenBy(u => u.Firstname)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        _logger.LogInformation("Getting user by id");

        return await _context.Users.FindAsync(id);
    }

    public async Task<User> CreateUserAsync(CreateUserDTO userDto)
    {
        _logger.LogInformation("Creating user {Email}", userDto.email);

        //Validator
        if (string.IsNullOrWhiteSpace(userDto.firstname) ||
            string.IsNullOrWhiteSpace(userDto.lastname) ||
            string.IsNullOrWhiteSpace(userDto.email) ||
            string.IsNullOrWhiteSpace(userDto.phonenumber))
        {
            throw new ArgumentException("Fill out all fields");
        }

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == userDto.email);

        if (existingUser != null)
        {
            throw new InvalidOperationException("User already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Firstname = userDto.firstname,
            Lastname = userDto.lastname,
            Email = userDto.email,
            Phonenumber = userDto.phonenumber,
            Password = userDto.password, // TODO: Hash password
            Balance = 0,
            Timestamp = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created user {UserId} - {Email}", user.Id, user.Email);

        return user;
    }

    public async Task<User> UpdateUserAsync(string id, UpdateUserDTO updateDto)
    {
        _logger.LogInformation("Updating user {UserId}", id);

        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        // Validate input
        if (string.IsNullOrWhiteSpace(updateDto.firstname) ||
            string.IsNullOrWhiteSpace(updateDto.lastname) ||
            string.IsNullOrWhiteSpace(updateDto.email) ||
            string.IsNullOrWhiteSpace(updateDto.phonenumber))
        {
            throw new ArgumentException("Fill out all fields");
        }

        // Check if email is already an existing email
        if (user.Email != updateDto.email)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == updateDto.email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists");
            }
        }

        user.Firstname = updateDto.firstname;
        user.Lastname = updateDto.lastname;
        user.Email = updateDto.email;
        user.Phonenumber = updateDto.phonenumber;

        if (!string.IsNullOrWhiteSpace(updateDto.password))
        {
            user.Password = updateDto.password; // TODO: Hash password
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated user {UserId}", id);

        return user;
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        _logger.LogInformation("Deleting user {UserId}", id);

        var user = await _context.Users
            .Include(u => u.Boards)
            .Include(u => u.Balancelogs)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        if (user.Boards.Any())
        {
            throw new InvalidOperationException("Cannot delete user with existing boards");
        }

        if (user.Balance > 0)
        {
            throw new InvalidOperationException($"Cannot delete user with remaining balance ({user.Balance} DKK).");
        }

        if (user.Balancelogs.Any())
        {
            throw new InvalidOperationException("Cannot delete user with transaction history.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted user {UserId} - {Email}", id, user.Email);

        return true;
    }

    public async Task<object> GetUserDetailsAsync(string id)
    {
        _logger.LogInformation("Getting user details for user {UserId}", id);
        
        var user = await _context.Users
            .Include(u => u.Boards)
            .Include(u => u.Balancelogs)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }
        
        return new
        {
            user.Id,
            user.Firstname,
            user.Lastname,
            user.Email,
            user.Phonenumber,
            user.Balance,
            user.Timestamp,
            TotalBoards = user.Boards.Count,
            WinningBoards = user.Boards.Count(b => b.Winner),
            TotalTransactions = user.Balancelogs.Count,
            Boards = user.Boards.Select(b => new
            {
                b.Id,
                b.Selectednumbers,
                b.Timestamp,
                b.Winner
            }).OrderByDescending(b => b.Timestamp).ToList(),
            RecentTransactions = user.Balancelogs.Select(bl => new
            {
                bl.Id,
                bl.Amount,
                bl.Transactionnumber,
                bl.Timestamp
            }).OrderByDescending(bl => bl.Timestamp).Take(10).ToList()
        };
    }
}
