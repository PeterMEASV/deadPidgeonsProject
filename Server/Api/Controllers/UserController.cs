using Api.Models;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }
    
    [HttpGet("all")]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return StatusCode(500, "An error occurred while getting all users");
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(string id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound("User Not Found");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error When Getting User By Id");
            return StatusCode(500, "Error When Getting User By Id Check GetById");
        }
    }
    
    [HttpPost("create")]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDTO userDto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(userDto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, "An error occurred while creating the user");
        }
 
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<User>> UpdateUser(string id, [FromBody] UpdateUserDTO updateDto)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, updateDto);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {Id}", id);
            return StatusCode(500, "An error occurred while updating the user");
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new { Message = "User deleted successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Id}", id);
            return StatusCode(500, "An error occurred while deleting the user");
        }
    }
    
    [HttpGet("{id}/details")]
    public async Task<ActionResult<object>> GetUserDetails(string id)
    {
        try
        {
            var details = await _userService.GetUserDetailsAsync(id);
            return Ok(details);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user details for {Id}", id);
            return StatusCode(500, "An error occurred while retrieving user details");
        }
    }
    [HttpPatch("{id}/toggle-active")]
    public async Task<ActionResult<User>> ToggleUserActiveStatus(string id)
    {
        try
        {
            var user = await _userService.ToggleUserActiveStatusAsync(id);
            return Ok(new
            {
                user.Id,
                user.Firstname,
                user.Lastname,
                user.Email,
                user.Isactive,
                Status = user.Isactive ? "Active" : "Inactive",
                Message = $"User is now {(user.Isactive ? "ACTIVE" : "INACTIVE")}"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling user active status {Id}", id);
            return StatusCode(500, "An error occurred while toggling user status");
        }
    }
    
    [HttpPatch("{id}/set-active")]
    public async Task<ActionResult<User>> SetUserActiveStatus(string id, [FromBody] SetUserActiveDTO dto)
    {
        try
        {
            var user = await _userService.SetUserActiveStatusAsync(id, dto.IsActive);
            return Ok(new
            {
                user.Id,
                user.Firstname,
                user.Lastname,
                user.Email,
                user.Isactive,
                Status = user.Isactive ? "Active" : "Inactive",
                Message = $"User is now {(user.Isactive ? "ACTIVE" : "INACTIVE")}"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting user active status {Id}", id);
            return StatusCode(500, "An error occurred while setting user status");
        }
    }
    
    [HttpPatch("{id}/set-admin")]
    public async Task<ActionResult<User>> SetUserAdminStatus(string id, [FromBody] SetUserAdminDTO dto)
    {
        try
        {
            var user = await _userService.SetUserAdminStatusAsync(id, dto.isAdmin);
            return Ok(new
            {
                user.Id,
                user.Firstname,
                user.Lastname,
                user.Email,
                user.Isadmin,
                Status = user.Isadmin ? "Admin" : "User",
                Message = $"User is now {(user.Isadmin ? "ADMIN" : "NORMAL USER")}"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting user admin status {Id}", id);
            return StatusCode(500, "An error occurred while setting admin status");
        }
    }

    
}