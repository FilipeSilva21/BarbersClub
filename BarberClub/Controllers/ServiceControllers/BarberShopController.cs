using System.Security.Claims;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.ServiceControllers;

[Route("barbershop/[controller]")]
[ApiController]
public class BarberShopController(IBarberShopService barberShopService): ControllerBase
{
    
    [HttpPost ("/barberShopApi/register")]
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
        
        var barberShop = await barberShopService.RegisterBarberShop(userId, request);

        if (barberShop == null)
        {
            return NotFound($"Autor com ID {userId} não encontrado no banco de dados.");
        }

        return Ok(barberShop);
    }

    [HttpGet("/barberShop/getAll")]
    public async Task<IActionResult> GetBarberShops()
    {
        var barberShops = await barberShopService.GetBarberShops();

        return Ok(barberShops);
    }
    
    [HttpGet("/barberShop/{barberShopId}")]
    public async Task<IActionResult> GetBarberShopById(int barberShopId)
    {
        var barberShop = await barberShopService.GetBarberShopById(barberShopId);
        
        if (barberShop == null)
            return NotFound();
        
        return Ok(barberShop);
    }
    
    [HttpGet("/barberShop/search")]
    public async Task<IActionResult> SearchBarberShops(
        [FromQuery] string? city,
        [FromQuery] string? state,
        [FromQuery] string? barberName,
        [FromQuery] string? barberShopName)
    {
        var barberShop = await barberShopService.SearchBarberShops(barberShopName, city, state, barberName);
        
        return Ok(barberShop);
    }
    
    [HttpDelete("/barberShop/{barberShopId}")]
    public async Task<IActionResult> DeleteBarberShop(int barberShopId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);

        var success = await barberShopService.DeleteBarberShop(barberShopId, userId);

        if (!success)
            return NotFound();

        return NoContent(); 
    }
}