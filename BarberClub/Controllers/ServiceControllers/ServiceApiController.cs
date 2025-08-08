using BarberClub.DTOs;
using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization; // Adicionado
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // Adicionado

namespace BarberClub.Controllers.ServiceControllers;

[Route("api/services")]
[ApiController]
[Authorize] 
public class ServiceApiController(IServiceService serviceService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] ServiceRegisterRequest request)
    {
        var createdService = await serviceService.CreateServiceAsync(request);
        
        return CreatedAtAction(nameof(GetServicesByBarberShop), new { barberShopId = createdService.BarberShopId }, createdService);
    }

    [HttpGet("myServices")]
    public async Task<IActionResult> GetMyServices([FromQuery] string status)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            return Unauthorized("Sessão inválida.");
        
        var services = await serviceService.GetServicesByUserAndStatusAsync(userId, status); 
        
        return Ok(services);
    }

    [HttpPost("{serviceId}/cancel")]
    public async Task<IActionResult> CancelService(int serviceId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var result = await serviceService.CancelServiceAsync(serviceId, userId);
        
        if (!result)
        {
            return BadRequest("Não foi possível cancelar o agendamento. Verifique se ele ainda está confirmado.");
        }

        return Ok(new { message = "Agendamento cancelado com sucesso." });
    }
    
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> GetServicesByUser(int userId)
    {
        var services = await serviceService.GetServicesByUserAsync(userId);
        return Ok(services);
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
    
    [HttpGet("bookedTimes")]

    public async Task<IActionResult> GetBookedTimes([FromQuery] int barberShopId, [FromQuery] DateTime date)
    {
        if (barberShopId <= 0)
        {
            return BadRequest("O ID da barbearia é inválido.");
        }
        try
        {
            var bookedTimes = await serviceService.GetBookedTimesAsync(barberShopId, date);
            
            return Ok(bookedTimes);
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro interno ao buscar os horários.");
        }
    }
}