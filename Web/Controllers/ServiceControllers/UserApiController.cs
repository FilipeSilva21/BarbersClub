using System.Security.Claims;
using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;
using Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.ServiceControllers;

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
      
        if(user is null)
            return NotFound();
        
        return Ok(user);
    }   
    
    [HttpPut("edit")] 
    [Authorize] 
    public async Task<IActionResult> UpdateProfile([FromForm] UserUpdateRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized("ID de usuário inválido no token.");

        var updatedUser = await userService.UpdateUserAsync(userId, request);

        if (updatedUser is null)
            return BadRequest("Não foi possível atualizar o usuário. Verifique os dados fornecidos.");

        var claims = User.Claims
            .Where(c => c.Type != ClaimTypes.Name && 
                        c.Type != ClaimTypes.Email &&
                        c.Type != "hasBarbershops" &&
                        c.Type != "barberShopId") // Também remove o ID antigo para evitar duplicatas
            .ToList();
        
        claims.Add(new Claim(ClaimTypes.Name, updatedUser.FirstName));
        claims.Add(new Claim(ClaimTypes.Email, updatedUser.Email));

        var userBarberShops = updatedUser.BarberShops;
        bool hasShops = userBarberShops != null && userBarberShops.Any();

        claims.Add(new Claim("hasBarbershops", hasShops.ToString().ToLower()));
        
        if (hasShops)
        {
            var barberShopId = userBarberShops.First().BarberShopId.ToString();
            claims.Add(new Claim("barberShopId", barberShopId));
        }

        var updatedToken = authService.GenerateToken(claims);

        return Ok(new 
        {
            updatedUser.UserId,
            updatedUser.FirstName,
            updatedUser.LastName,
            updatedUser.Email,
            updatedUser.ProfilePicUrl,
            updatedUser.Role, 
            token = updatedToken 
        });
    }
}