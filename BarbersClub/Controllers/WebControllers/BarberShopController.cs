using System.Security.Claims;
using BarberClub.DTOs;
using BarbersClub.Services.Interfaces;
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
        // Ensure your BarberShopDetails.cshtml view uses @model BarberShopResponse
        return View("~/Views/BarberShop/BarberShopDetails.cshtml", barberShop);
    }
    
    // --- MÉTODO CORRIGIDO ---
    [HttpGet("barbershop/edit/{id}")]
    [Authorize] // Added authorization for security
    public async Task<IActionResult> UpdateBarberShop(int id)
    {
        // 1. Call the new method to get the FULL model for editing
        var barberShop = await barberShopContext.GetBarberShopForUpdateAsync(id);
        if (barberShop == null)
        {
            return NotFound();
        }

        // 2. Safe permission check
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId) || barberShop.UserId != userId)
        {
            return Forbid(); // Use Forbid() for authorization failures
        }

        // 3. Map the full BarberShop MODEL to the BarberShopUpdateRequest DTO
        var dto = new BarberShopUpdateRequest
        {
            BarberShopId = barberShop.BarberShopId,
            Name = barberShop.Name, // Corrected property name
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
                ServiceType = os.ServiceType.ToString(), // Corrected property name
                Price = os.Price
            }).ToList()
        };

        // 4. Pass the DTO to the View
        // Ensure your UpdateBarberShop.cshtml view uses @model BarberShopUpdateRequest
        return View("~/Views/BarberShop/UpdateBarberShop.cshtml", dto);
    }
}