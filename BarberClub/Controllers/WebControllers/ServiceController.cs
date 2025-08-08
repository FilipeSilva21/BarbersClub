using System.Security.Claims;
using System.Text.RegularExpressions;
using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.WebControllers;

public class ServiceController(IServiceService serviceContext, IBarberShopService barberShopService) : Controller
{
    [HttpGet("/services")]
    public IActionResult Services()
    {
        return View("~/Views/Service/ShowServices.cshtml");
    }
    
    [HttpGet("service/create/{barberShopId}")]
    public async Task<IActionResult> CreateServiceForBarberShop(int barberShopId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Sessão inválida.");
        }

        var barberShop = await barberShopService.GetBarberShopByIdAsync(barberShopId);
        if (barberShop == null)
        {
            return NotFound("Barbearia não encontrada.");
        }
    
        var availableServicesForView = barberShop.OfferedServices
            .Select(s => new 
            {
                Value = s.ServiceType.ToString(),
                Text = Regex.Replace(s.ServiceType.ToString(), "(\\B[A-Z])", " $1"),
            
                Price = s.Price
            })
            .ToList();

        ViewBag.PreselectedBarberShopId = barberShopId;
        ViewBag.CurrentUserId = userId;
        ViewBag.AvailableServices = availableServicesForView; 
    
        return View("~/Views/Service/RegisterService.cshtml");
    }
}