using BarberClub.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers;

public class AccountController : Controller
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
}