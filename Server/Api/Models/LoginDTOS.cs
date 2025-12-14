namespace Api.Models;

public record LoginDTO(
    string Email,
    string Password
);

public record LoginResponseDTO(
    string id,
    string firstname,
    string lastname,
    string email,
    string phonenumber,
    decimal balance,
    bool Isactive,
    bool Isadmin, 
    string token,
	string message 
);