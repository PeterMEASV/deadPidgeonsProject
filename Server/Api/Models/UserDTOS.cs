namespace Api.Models;

public record CreateUserDTO(string firstname, string lastname, string email, string phonenumber, string password);

public record DeleteUserDTO(string id);

public record UpdateUserDTO(string firstname, string lastname, string email, string phonenumber, string password, decimal balance);

