using System.Security.Claims;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services;
using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.ServiceControllers;

[Route("barberShopApi")]
[ApiController]
public class BarberShopApiController(IBarberShopService barberShopService): ControllerBase
{
    
    [HttpPost ("register")] 
    [Authorize]
    public async Task<IActionResult> RegisterBarberShop([FromBody] BarberShopRegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdString))
            return Unauthorized(); 

        if (!int.TryParse(userIdString, out int userId))
        {
            return BadRequest("ID de usuário inválido no token.");
        }
        
        var barberShop = await barberShopService.RegisterBarberShopAsync(userId, request);

        if (barberShop == null)
        {
            return NotFound($"Autor com ID {userId} não encontrado no banco de dados.");
        }
        Console.WriteLine(request + "2");
        return Ok(barberShop);
    }

    [HttpGet]
    public async Task<IActionResult> GetBarberShops()
    {
        var barberShops = await barberShopService.GetBarberShopsAsync();

        return Ok(barberShops);
    }
    
    [HttpGet("{barberShopId}")]
    public async Task<IActionResult> GetBarberShopById(int barberShopId)
    {
        var barberShop = await barberShopService.GetBarberShopByIdAsync(barberShopId);
        
        if (barberShop == null)
            return NotFound();
        
        return Ok(barberShop);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> SearchBarberShops(
        [FromQuery] string? city,
        [FromQuery] string? state,
        [FromQuery] string? barberName,
        [FromQuery] string? barberShopName)
    {
        var barberShop = await barberShopService.SearchBarberShopsAsync(barberShopName, state, city, barberName);
        
        return Ok(barberShop);
    }
    
    [HttpDelete("{barberShopId}")]
    public async Task<IActionResult> DeleteBarberShop(int barberShopId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);

        var success = await barberShopService.DeleteBarberShopAsync(barberShopId, userId);

        if (!success)
            return NotFound();

        return NoContent(); 
    }
}