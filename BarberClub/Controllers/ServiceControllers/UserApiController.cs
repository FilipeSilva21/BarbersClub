using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.ServiceControllers;

[Route("api/users")]
[ApiController]
public class UserApiController (IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await userService.GetUsers();

        return Ok(users);
    }
}