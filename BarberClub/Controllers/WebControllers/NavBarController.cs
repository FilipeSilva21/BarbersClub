using BarberClub.Services;
using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.WebControllers;

public class NavBarController : Controller
{
    private readonly IBarberShopService _context;
    
    public NavBarController (IBarberShopService barberShopService)
    {
        _context = barberShopService;
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
    
    [HttpGet("barbershop/details/{barberShopId}")]
    public async Task<IActionResult> Details(int barberShopId) 
    {
        var barberShop = await _context.GetBarberShopByIdAsync(barberShopId);

        if (barberShop == null)
        {
            return NotFound();
        }

        return View("~/Views/NavBar/BarberShop/BarberShopDetails.cshtml", barberShop);
    }
    
    [HttpGet]
    [Route("/navbar/dashboard")]
    public IActionResult Dashboard()
    {
        return View("~/Views/NavBar/Dashboard.cshtml");
    }
}