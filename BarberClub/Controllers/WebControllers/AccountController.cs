using System.Security.Claims;
using BarberClub.DTOs;
using BarberClub.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.WebControllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    
    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }
        
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
        var (token, user) = await _authService.LoginAsync(request);

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
        return RedirectToAction("Index", "Home");
    }
    
    [Authorize]
    public IActionResult Dashboard()
    {
        return View();
    }
}
