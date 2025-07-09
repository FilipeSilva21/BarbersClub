using System.Security.Claims;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers;

[Route("barbearia/[controller]")]
[ApiController]
public class BarberShopController : ControllerBase
{
    private readonly IBarberShopService _service;

    public BarberShopController(IBarberShopService service)
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
    
    [HttpGet("id/{barberShopId}")]
    public async Task<IActionResult> GetBarberShopById(int barberShopId)
    {
        var barberShop = await _service.GetBarberShopById(barberShopId);
        
        if (barberShop == null)
            return NotFound();
        
        return Ok(barberShop);
    }
    
    [HttpGet("state/{state}")]
    public async Task<IActionResult> GetBarberShopsByState(string state)
    {
        var barberShop = await _service.GetBarberShopsByState(state);
        
        if (barberShop == null)
            return NotFound();
        
        return Ok(barberShop);
    }
    
    [HttpGet("city/{city}")]
    public async Task<IActionResult> GetBarberShopsByCity(string city)
    {
        var barberShop = await _service.GetBarberShopsByState(city);
        
        if (barberShop == null)
            return NotFound();
        
        return Ok(barberShop);
    }
    
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetBarberShopsByUser(int userId)
    {
        var barberShop = await _service.GetAllBarberShopsByUserId(userId);

        return Ok(barberShop);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllBarberShops()
    {
        var barberShops = await _service.GetAllBarberShops();
        
        return Ok(barberShops);
    }
    
    [HttpDelete("{barberShopId}")]
    public async Task<IActionResult> DeleteBarberShop(int barberShopId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var success = await _service.DeleteBarberShop(barberShopId, userId);

        if (!success)
            return NotFound();

        return NoContent(); 
    }
}