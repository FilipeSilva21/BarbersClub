using System.Security.Claims;
using BarbersClub.Business.Services.Interfaces;
using Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.WebControllers;

public class NavBarController(IDashboardStatsService dashboardContext, IBarberShopService barberShopContext) : Controller
{
    [HttpGet]
    [Authorize]
    [Route("/navbar/dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Redirect("/Auth/Login");
        }

        var barberShops = await barberShopContext.GetBarberShopsByUserIdAsync(userId);

        if (barberShops is null || !barberShops.Any())
        {
            ViewData["Message"] = "Você ainda não cadastrou sua barbearia.";
            return View("~/Views/NavBar/Dashboard.cshtml", null);
        }
        
        var firstBarberShop = barberShops.First();
        
        var statsDto = await dashboardContext.GetDashboardStatsAsync(firstBarberShop?.BarberShopId);


        return View("~/Views/NavBar/Dashboard.cshtml", statsDto);
    }
}