using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BarberClub.Controllers;

[Route("api/[controller]")]
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
    public async Task<ActionResult<string>> Login(UserLoginRequest request)
    {
        var result = await authService.LoginAsync(request);

        if (result is null)
            return BadRequest(new { message = "Email ou senha inválidos." });
        
        return Ok(new { token = result });
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