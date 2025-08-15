using System.Security.Claims;
using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbersClub.Controllers.WebControllers;

public class BarberShopController(IBarberShopService barberShopContext) : Controller
{
    [HttpGet]
    [Route("/barbershops")]
    public IActionResult BarberShop()
    {
        return View("~/Views/BarberShop/ShowBarberShops.cshtml");
    }
    
    [HttpGet("/barbershops/register")]
    [Authorize]
    public IActionResult RegisterBarberShop()
    {
        return View("~/Views/BarberShop/RegisterBarberShop.cshtml");    
    }
    
    [HttpGet("barbershop/details/{barberShopId}")]
    public async Task<IActionResult> Details(int barberShopId) 
    {
        var barberShop = await barberShopContext.GetBarberShopByIdAsync(barberShopId);
        
        if (barberShop == null)
        {
            return NotFound();
        }
        return View("~/Views/BarberShop/BarberShopDetails.cshtml", barberShop);
    }
    
    [HttpGet("barbershop/edit/{id}")]
    [Authorize] 
    public async Task<IActionResult> UpdateBarberShop(int id)
    {
        var barberShop = await barberShopContext.GetBarberShopForUpdateAsync(id);
        if (barberShop is null)
        {
            return NotFound();
        }
        
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId) || barberShop.UserId != userId)
        {
            return Forbid(); 
        }

        var dto = new BarberShopUpdateRequest
        {
            BarberShopId = barberShop.BarberShopId,
            Name = barberShop.Name, 
            Address = barberShop.Address,
            City = barberShop.City,
            State = barberShop.State,
            WhatsApp = barberShop.WhatsApp,
            Instagram = barberShop.Instagram,
            OpeningHours = barberShop.OpeningHours,
            ClosingHours = barberShop.ClosingHours,
            CurrentProfilePicUrl = barberShop.ProfilePicUrl,
            WorkingDays = barberShop.WorkingDays,
            OfferedServices = barberShop.OfferedServices.Select(os => new OfferedServiceResponse
            {
                ServiceType = os.ServiceType.ToString(), 
                Price = os.Price
            }).ToList()
        };

        return View("~/Views/BarberShop/UpdateBarberShop.cshtml", dto);
    }
}