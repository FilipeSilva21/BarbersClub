using BarberClub.DTOs;
using BarberClub.Services;
using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.ServiceControllers;

[Route("servicesApi")]
[ApiController]
public class ServicesApiController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServicesApiController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] ServiceRegisterRequest request)
    {
        var createdService = await _serviceService.CreateServiceAsync(request);
        return CreatedAtAction(nameof(GetServicesByBarberShop), new { barberShopId = createdService.BarberShopId }, createdService);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBarberShops()
    {
        var barberShops = await _serviceService.GetServicesAsync();

        return Ok(barberShops);
    }

    [HttpGet("barbershop/{barberShopId}")]
    public async Task<IActionResult> GetServicesByBarberShop(int barberShopId)
    {
        var services = await _serviceService.GetServicesByBarberShopAsync(barberShopId);
        return Ok(services);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetServicesByUser(int userId)
    {
        var services = await _serviceService.GetServicesByUserAsync(userId);
        return Ok(services);
    }
}