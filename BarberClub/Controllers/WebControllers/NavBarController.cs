using System.Security.Claims;
using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.WebControllers;

public class NavBarController(IDashboardStatsService dashboardContext, IBarberShopService barberShopContext) : Controller
{
    [HttpGet]
    [Route("/navbar/dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("ID de usuário inválido.");
        }

        var barberShops = await barberShopContext.GetBarberShopsByUserIdAsync(userId);

        if (barberShops == null || !barberShops.Any())
        {
            ViewData["Message"] = "Você ainda não cadastrou sua barbearia.";
            return View("~/Views/NavBar/Dashboard.cshtml", null);
        }
        
        var firstBarberShop = barberShops.First();
        
        var statsDto = await dashboardContext.GetDashboardStatsAsync(firstBarberShop?.BarberShopId);


        return View("~/Views/NavBar/Dashboard.cshtml", statsDto);
    }
    
    [HttpGet("/navbar/profile")]
    public IActionResult Profile()
    {
        return View("~/Views/NavBar/Profile.cshtml");
    }

}