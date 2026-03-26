using System.Security.Claims;
using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;
using Business.DTOs;
using Business.Error_Handling;
using Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.ServiceControllers;

[Route("api/barberShop")]
[ApiController]
public class BarberShopApiController(IBarberShopService barberShopService, IAuthService authService): ControllerBase
{
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RegisterBarberShop([FromForm] BarberShopRegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString))
            return Unauthorized(); 

        if (!int.TryParse(userIdString, out int userId))
            return BadRequest("ID de usuário inválido no token.");

        var barberShop = await barberShopService.RegisterBarberShopAsync(userId, request);

        if (barberShop is null)
            return NotFound($"Autor com ID {userId} não encontrado no banco de dados.");
        
        var claims = User.Claims.ToList();

        var existingClaim = claims.FirstOrDefault(c => c.Type == "hasBarbershops");
        if (existingClaim is not null)
            claims.Remove(existingClaim);
        
        claims.Add(new Claim("hasBarbershops", "true"));

        var updatedToken = authService.GenerateToken(claims);

        return Ok(new { barberShop, token = updatedToken });
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
        
        if (barberShop is null)
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
    
    [HttpDelete("delete/{barberShopId}")]
    [Authorize]
    public async Task<IActionResult> DeleteBarberShop(int barberShopId)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId))
            return Unauthorized("ID de usuário inválido.");

        var success = await barberShopService.DeleteBarberShopAsync(barberShopId, userId);

        if (!success)
            throw new BarberShopNotFoundException(barberShopId);
        
        var claims = User.Claims.ToList();

        claims.RemoveAll(c => c.Type == "hasBarbershops" || c.Type == "barberShopId");

        claims.Add(new Claim("hasBarbershops", "false"));
    
        var updatedToken = authService.GenerateToken(claims);

        return Ok(new { token = updatedToken });
    }
    
    [HttpPost("edit")]
    [Authorize]
    public async Task<IActionResult> Edit([FromForm] BarberShopUpdateRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
            return Unauthorized("Token de usuário inválido.");

        var updatedBarberShop = await barberShopService.UpdateBarberShopAsync(request.BarberShopId, userId, request);

        if (updatedBarberShop is null)
            return BadRequest(new { message = "Não foi possível atualizar a barbearia. Verifique os dados ou a permissão." });
        
        return Ok(updatedBarberShop);
    }
    
    [HttpGet("top-rated")]
    public async Task<IActionResult> GetTopRated([FromQuery] int count = 6)
    {
        var topRatedShops = await barberShopService.GetTopRatedBarberShopAsync(count);
        return Ok(topRatedShops);
    }
}