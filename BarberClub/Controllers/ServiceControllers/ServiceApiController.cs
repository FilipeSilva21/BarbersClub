using BarberClub.DTOs;
using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.ServiceControllers;

[Route("api/services")]
[ApiController]
public class ServiceApiController(IServiceService serviceService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] ServiceRegisterRequest request)
    {
        var createdService = await serviceService.CreateServiceAsync(request);
        
        return CreatedAtAction(nameof(GetServicesByBarberShop), new
        {
            barberShopId = createdService.BarberShopId
        }, createdService);
    }
    
    [HttpGet] 
    public async Task<IActionResult> GetServices(
        [FromQuery] string? barberShopName,
        [FromQuery] string? clientName,
        [FromQuery] string? serviceType,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var services = await serviceService.GetServicesAsync(barberShopName, clientName, serviceType, startDate, endDate);
        return Ok(services);
    }

    [HttpGet("barbershop/{barberShopId}")]
    public async Task<IActionResult> GetServicesByBarberShop(int barberShopId)
    {
        var services = await serviceService.GetServicesByBarberShopAsync(barberShopId);
        return Ok(services);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetServicesByUser(int userId)
    {
        var services = await serviceService.GetServicesByUserAsync(userId);
        return Ok(services);
    }
}