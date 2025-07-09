using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BarberClub.Controllers;

[Route("auth/[controller]")]
[ApiController]
public class AuthController(IAuthService authService): ControllerBase
{
    [HttpPost ("register")]
    public async Task<ActionResult<User>> Register(UserRegisterRequest request)
    {
        var user = await authService.RegisterAsync(request);

        if (user is null)
            return BadRequest("Email ja cadastrado");
        
        return Ok(user);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequest request)
    {
        var (token, user) = await authService.LoginAsync(request);

        if (token is null || user is null)
        {
            return Unauthorized(new { message = "Email ou senha inválidos." });
        }

        return Ok(new
        {
            token = token,
            user = new
            {
                name = user.FirstName,
                email = user.Email
            }
        });
    }


    [Authorize]
    [HttpGet ("home")]
    public IActionResult AuthenticationOnlyEndpoint()
    {
        return Ok("Bem-vindo ao BarberClub!");
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet ("admin")]
    public IActionResult AdminOnlyEndpoint()
    {
        return Ok("Painel de controle");
    }
}