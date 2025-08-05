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
        return View("~/Views/NavBar/Service/ShowServices.cshtml");
    }
    
    [HttpGet("service/create/{barberShopId}")]
    public async Task<IActionResult> CreateServiceForBarberShop(int barberShopId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Sessão inválida.");
        }

        // Este método precisa do IBarberShopService injetado no controller
        var barberShop = await barberShopService.GetBarberShopByIdAsync(barberShopId);
        if (barberShop == null)
        {
            return NotFound("Barbearia não encontrada.");
        }
    
        // --- INÍCIO DA CORREÇÃO ---
        var availableServicesForView = barberShop.OfferedServices
            .Select(s => new // 's' aqui é um objeto do tipo OfferedService
            {
                // CORREÇÃO: Acessar a propriedade 'ServiceType' do objeto 's'
                Value = s.ServiceType.ToString(),
                Text = Regex.Replace(s.ServiceType.ToString(), "(\\B[A-Z])", " $1"),
            
                // NOVO: Incluir o 'Price' para que o JavaScript possa usá-lo
                Price = s.Price
            })
            .ToList();
        // --- FIM DA CORREÇÃO ---

        ViewBag.PreselectedBarberShopId = barberShopId;
        ViewBag.CurrentUserId = userId;
        ViewBag.AvailableServices = availableServicesForView; 
    
        return View("~/Views/navbar/Service/RegisterService.cshtml");
    }
}