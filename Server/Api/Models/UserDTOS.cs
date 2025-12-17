using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public record CreateUserDTO([Required] string firstname, [Required] string lastname, [Required] [EmailAddress] string email, [Required] [Length(8,8)] [Phone] string phonenumber, [Required] [MinLength(3)] string password);

public record DeleteUserDTO(string id);

public record UpdateUserDTO([Required] string firstname, [Required] string lastname, [Required] [EmailAddress] string email, [Required] [Length(8,8)] [Phone] string phonenumber, [MinLength(3)] string? password);

public record SetUserActiveDTO(bool IsActive);

public record SetUserAdminDTO(bool isAdmin);
