using System.Security.Claims;
using BarbersClub.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.DbContext;

namespace Web.Controllers.ServiceControllers;

[ApiController]
[Route("api/dashboard")] 
public class DashboardStatsApiController(ProjectDbContext context, IDashboardStatsService dashboardService)
    : ControllerBase
{

    [HttpGet("stats")]
    [Authorize]
    public async Task<IActionResult> GetDashboardStats()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized("ID de usuário inválido no token.");
        
        var barberShop = await context.BarberShops
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.UserId == userId);
            
        if (barberShop == null)
        {
            return NotFound("Nenhuma barbearia encontrada para este usuário.");
        }

        var stats = await dashboardService.GetDashboardStatsAsync(barberShop.BarberShopId);

        if (stats is null)
            return Problem("Ocorreu um erro ao calcular as estatísticas.");

        return Ok(stats);
    }
}