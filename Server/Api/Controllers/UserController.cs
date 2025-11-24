using Api.Models;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    [Route(nameof(GetAllUsers))]
    [HttpGet]
    public Task<ActionResult<List<User>>> GetAllUsers()
    {
        
        throw new NotImplementedException();
    }
    
    [Route(nameof(GetById))]
    [HttpGet]
    public Task<ActionResult<User>> GetById([FromBody] string id)
    {
        throw new NotImplementedException();
    }

    [Route(nameof(CreateUser))]
    [HttpPost]
    public Task<ActionResult<User>> CreateUser([FromBody] CreateUserDTO userDto)
    {
        throw new NotImplementedException();
    }
    
    [Route(nameof(UpdateUser))]
    [HttpPatch]
    public Task<ActionResult<User>> UpdateUser(string id, User user)
    {
        throw new NotImplementedException();
    }
    
    [Route(nameof(DeleteUser))]
    [HttpDelete]
    public Task<ActionResult<User>> DeleteUser([FromBody] string id)
    {
        throw new NotImplementedException();
    }
}