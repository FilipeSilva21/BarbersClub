using System.Security.Claims;
using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 

namespace BarberClub.Controllers.WebControllers;

public class NavBarController : Controller
{
    private readonly IBarberShopService _barberShopContext;
    private readonly IDashboardStatsService _dashboardContext;
    
    public NavBarController(
        IBarberShopService barberShopContext, 
        IDashboardStatsService dashboardContext)
    {
        _barberShopContext = barberShopContext;
        _dashboardContext = dashboardContext;
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
        var barberShop = await _barberShopContext.GetBarberShopByIdAsync(barberShopId);
        if (barberShop == null)
        {
            return NotFound();
        }
        return View("~/Views/NavBar/BarberShop/BarberShopDetails.cshtml", barberShop);
    }
    
    [HttpGet]
    [Route("/navbar/dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("ID de usuário inválido.");
        }

        var barberShops = await _barberShopContext.GetBarberShopsByUserIdAsync(userId);

        if (barberShops == null || !barberShops.Any())
        {
            ViewData["Message"] = "Você ainda não cadastrou sua barbearia.";
            return View("~/Views/NavBar/Dashboard.cshtml", null);
        }
        
        var firstBarberShop = barberShops.First();
        
        var statsDto = await _dashboardContext.GetDashboardStatsAsync(firstBarberShop.BarberShopId);


        return View("~/Views/NavBar/Dashboard.cshtml", statsDto);
    }

}