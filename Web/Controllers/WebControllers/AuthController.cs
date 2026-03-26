using BarbersClub.Business.DTOs;
using Business.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.WebControllers;

public class AuthController(IAuthService authService) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        var model = new UserRegisterRequest();
        
        return View(model); 
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        var (token, user) = await authService.LoginAsync(request);

        if (token is null || user is null)
        {
            return Unauthorized(new { message = "Email ou senha inválidos." });
        }

        return Ok(new { token, user = new { user.FirstName, user.Email } });
    }
    
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    
        return Ok(new { message = "Logout bem-sucedido." });
    }
}
