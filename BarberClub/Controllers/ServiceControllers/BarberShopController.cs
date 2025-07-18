using System.Security.Claims;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.ServiceControllers;

[Route("barbearia/[controller]")]
[ApiController]
public class BarberShopApiController : ControllerBase
{
    private readonly IBarberShopService _service;

    public BarberShopApiController(IBarberShopService service)
    {
        this._service = service;
    }
    
    [HttpPost ("{userId}/register")]
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
        
        var barberShop = await _service.RegisterBarberShop(userId, request);

        if (barberShop == null)
        {
            return NotFound($"Autor com ID {userId} não encontrado no banco de dados.");
        }

        return CreatedAtAction(nameof(GetBarberShopById), new { id = barberShop.BarberShopId }, barberShop);
    }

    [HttpGet("barberShops")]
    public async Task<IActionResult> Index()
    {
        var barberShops = await _service.GetBarberShops();

        return Ok(barberShops);
    }
    
    [HttpGet("id/{barberShopId}")]
    public async Task<IActionResult> GetBarberShopById(int barberShopId)
    {
        var barberShop = await _service.GetBarberShopById(barberShopId);
        
        if (barberShop == null)
            return NotFound();
        
        return Ok(barberShop);
    }
    
    [HttpGet("/barberShops")]
    public async Task<IActionResult> GetBarberShops(
        [FromQuery] string? city,
        [FromQuery] string? state,
        [FromQuery] string? barberName,
        [FromQuery] string? barberShopName)
    {
        var barberShop = await _service.SearchBarberShops(barberShopName, city, state, barberName);
        
        return Ok(barberShop);
    }
    
    [HttpDelete("{barberShopId}")]
    public async Task<IActionResult> DeleteBarberShop(int barberShopId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);

        var success = await _service.DeleteBarberShop(barberShopId, userId);

        if (!success)
            return NotFound();

        return NoContent(); 
    }
}