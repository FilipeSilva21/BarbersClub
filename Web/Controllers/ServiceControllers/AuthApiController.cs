using System.Security.Claims;
using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;
using Business.Error_Handling;
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
    public async Task<ActionResult<User>> RegisterUser([FromForm] UserRegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await authService.RegisterUserAsync(request);
        return Ok(new { Message = "Usuário registrado com sucesso!" });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]UserLoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (token, user) = await authService.LoginAsync(request);

        if (token is null || user is null)
            return Unauthorized(new { message = "Email ou senha inválidos." });

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.Email, user.Email),
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