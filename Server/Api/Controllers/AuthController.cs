using Api.Models;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginDTO loginDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Email and password are required");
            }

            var user = await _authService.LoginAsync(loginDto);

            if (user == null)
            {
                return Unauthorized("Invalid email or password, or account is inactive");
            }

            var response = new LoginResponseDTO(
                user.Id,
                user.Firstname,
                user.Lastname,
                user.Email,
                user.Phonenumber,
                user.Balance,
                user.Isactive,
                user.Isadmin,
                "Login successful"
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, "An error occurred during login");
        }
    }
}