using System.Security.Claims;
using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbersClub.Controllers.ServiceControllers;

[Route("api/users")]
[ApiController]
public class UserApiController (IUserService userService, IAuthService authService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await userService.GetUsers();

        return Ok(users);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var user = await userService.GetUserByIdAsync(userId);
     
        if(user == null)
            return NotFound();
        
        return Ok(user);
    }   
    
    [HttpPut("edit")] 
    [Authorize] 
    public async Task<IActionResult> UpdateProfile([FromForm] UserUpdateRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("ID de usuário inválido no token.");
        }

        var updatedUser = await userService.UpdateUserAsync(userId, request);

        if (updatedUser is null)
        {
            return BadRequest("Não foi possível atualizar o usuário. Verifique os dados fornecidos.");
        }
    
        return Ok(new 
        {
            updatedUser.UserId,
            updatedUser.FirstName,
            updatedUser.LastName,
            updatedUser.Email,
            updatedUser.ProfilePicUrl,
            updatedUser.Role
        });
    }
}