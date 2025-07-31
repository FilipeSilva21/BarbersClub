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
    [Route("/barbershops")]
    public IActionResult BarberShop()
    {
        return View("~/Views/NavBar/BarberShop/ShowBarberShops.cshtml");
    }
    
    [HttpGet]
    [Route("/barbershops/register")]
    public IActionResult RegisterBarberShop()
    {
        return View("~/Views/NavBar/BarberShop/RegisterBarberShop.cshtml");    
    }
    
    [HttpGet]
    [Route("/navbar/dashboard")]
    public IActionResult Dashboard()
    {
        return View("~/Views/NavBar/Dashboard.cshtml");
    }
}