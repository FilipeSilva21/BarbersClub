using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System.Security.Claims;
using System.Threading.Tasks;
using BarbersClub;
using BarbersClub.Business.Services.Interfaces;
using BarbersClub.DbContext;
using Repository.DbContext;

namespace BarbersClub.Controllers.ServiceControllers;

[ApiController]
[Route("api/dashboard")] // Rota padronizada para APIs
public class DashboardStatsApiController : ControllerBase
{
    private readonly ProjectDbContext _context; // Apenas para encontrar a barbearia do usuário
    private readonly IDashboardStatsService _dashboardService; 

    public DashboardStatsApiController(ProjectDbContext context, IDashboardStatsService dashboardService)
    {
        _context = context;
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    [Authorize]
    public async Task<IActionResult> GetDashboardStats()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("ID de usuário inválido no token.");
        }
        
        var barberShop = await _context.BarberShops
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.UserId == userId);
            
        if (barberShop == null)
        {
            return NotFound("Nenhuma barbearia encontrada para este usuário.");
        }

        var stats = await _dashboardService.GetDashboardStatsAsync(barberShop.BarberShopId);

        if (stats == null)
        {
            return Problem("Ocorreu um erro ao calcular as estatísticas.");
        }

        return Ok(stats);
    }
}