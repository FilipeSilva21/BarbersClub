using BarberClub.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.WebControllers;

public class NavBarController : Controller
{
    private readonly IBarberShopService _service;
    
    public NavBarController (IBarberShopService barberShopService)
    {
        _service = barberShopService;
    }

    [HttpGet]
    [Route("/NavBar/BarberShop")]
    public IActionResult BarberShop()
    {
        return View("~/Views/NavBar/BarberShop.cshtml");
    }
    
    [HttpGet]
    [Route("/NavBar/Dashboard")]
    public IActionResult Dashboard()
    {
        return View("~/Views/NavBar/Dashboard.cshtml");
    }
}