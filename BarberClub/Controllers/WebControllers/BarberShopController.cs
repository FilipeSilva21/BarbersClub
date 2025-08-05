using System.Security.Claims;
using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.WebControllers;

public class BarberShopController(IBarberShopService barberShopContext) : Controller
{
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
        var barberShop = await barberShopContext.GetBarberShopByIdAsync(barberShopId);
        
        if (barberShop == null)
        {
            return NotFound();
        }
        return View("~/Views/NavBar/BarberShop/BarberShopDetails.cshtml", barberShop);
    }
}