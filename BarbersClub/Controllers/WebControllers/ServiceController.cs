using System.Security.Claims;
using System.Text.RegularExpressions;
using BarberClub.DTOs;
using BarbersClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BarbersClub.Controllers.WebControllers;

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
    
    [HttpGet("/services/barbershop/{id}")]
    public async Task<IActionResult> BarberShopServices(int id)
    {
        var barberShop = await barberShopService.GetBarberShopByIdAsync(id);
        if (barberShop == null)
        {
            return NotFound("Barbearia não encontrada.");
        }

        ViewBag.BarberShopId = barberShop.BarberShopId;
        ViewBag.BarberShopName = barberShop.Name;
        if (TimeSpan.TryParse(barberShop.OpeningHours, out TimeSpan openingTime))
        {
            ViewBag.OpeningTime = openingTime;
        }

        if (TimeSpan.TryParse(barberShop.ClosingHours, out TimeSpan closingTime))
        {
            ViewBag.ClosingTime = closingTime;
        }

        var serviceOptions = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
        {
            new() { Value = "", Text = "Todos os Serviços" }
        };

        if (barberShop.OfferedServices != null)
        {
            foreach (var service in barberShop.OfferedServices)
            {
                serviceOptions.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = service.ServiceType.ToString(),
                    Text = System.Text.RegularExpressions.Regex.Replace(service.ServiceType.ToString(), "(\\B[A-Z])", " $1")
                });
            }
        }

        ViewBag.ServiceTypeOptions = serviceOptions;
    
        return View("BarberShopServices");
    }
    
    [HttpGet("service/edit/{serviceId}")]
    public async Task<IActionResult> EditService(int serviceId)
    {
        var service = await serviceContext.GetServiceByIdAsync(serviceId);
        if (service == null)
            return NotFound("Agendamento não encontrado.");

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId) || service.BarberShop.UserId != userId)
            return Forbid(); 

        var availableServicesForView = service.BarberShop.OfferedServices
            .Select(s => new 
            {
                Value = s.ServiceType.ToString(),
                Text = Regex.Replace(s.ServiceType.ToString(), "(\\B[A-Z])", " $1"),
                Price = s.Price
            })
            .ToList();
    
        ViewBag.AvailableServices = availableServicesForView;

        return View("~/Views/Service/EditServices.cshtml", service);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateService(int serviceId, IFormFile photoFile)
    {
        if (photoFile == null || photoFile.Length == 0)
        {
            return View("EditServices" );
        }
    
        var updateRequest = new ServiceUpdateRequest
        {
            UploadedImage = photoFile
        };

        var updatedService = await serviceContext.UpdateServiceAsync(serviceId, updateRequest);

        if (updatedService == null)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Serviço atualizado com sucesso!";
        return RedirectToAction("BarberShopServices", new { id = updatedService.BarberShopId });
    }
}