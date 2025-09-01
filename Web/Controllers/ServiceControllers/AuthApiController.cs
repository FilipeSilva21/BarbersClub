using System.Security.Claims;
using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;
using Business.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;

namespace Web.Controllers.ServiceControllers;

[Route("api/auth")]
[ApiController]
public class AuthApiController(IAuthService authService): ControllerBase
{
    [HttpPost ("register")]
    public async Task<ActionResult<User>> Register([FromForm] UserRegisterRequest request)
    {
        var user = await authService.RegisterUserAsync(request);

        if (user is null)
            return BadRequest(new { message = "Email já cadastrado" });
        
        return Ok(user);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        var (token, user) = await authService.LoginAsync(request);

        if (token is null || user is null)
        {
            return Unauthorized(new { message = "Email ou senha inválidos." });
        }

        var cookieClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        bool hasShops = user.BarberShops is not null && user.BarberShops.Any();
        cookieClaims.Add(new Claim("hasBarbershops", hasShops.ToString().ToLower()));

        if (hasShops)
        {
            foreach (var barbershop in user.BarberShops)
            {
                cookieClaims.Add(new Claim("barberShopId", barbershop.BarberShopId.ToString()));
            }
        }
        
        var claimsIdentity = new ClaimsIdentity(
            cookieClaims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity), 
            new AuthenticationProperties 
            { 
                IsPersistent = true, 
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            }); 
    
        return Ok(new { token });
    }
}