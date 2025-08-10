using System.Security.Claims;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services;
using BarbersClub.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbersClub.Controllers.ServiceControllers;

[Route("api/auth")]
[ApiController]
public class AuthApiController(IAuthService authService): ControllerBase
{
    [HttpPost ("register")]
    public async Task<ActionResult<User>> Register([FromForm] UserRegisterRequest request)
    {
        var user = await authService.RegisterAsync(request);

        if (user is null)
            return BadRequest(new { message = "Email já cadastrado" });
        
        return Ok(user);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]UserLoginRequest request)
    {
        var (token, user) = await authService.LoginAsync(request);

        if (token == null || user == null)
        {
            return Unauthorized(new { message = "Email ou senha inválidos." });
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity), 
            new AuthenticationProperties { IsPersistent = true }); 
        
        return Ok(new { token });
    }
}