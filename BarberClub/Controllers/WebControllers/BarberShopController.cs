using System.Security.Claims;
using BarberClub.DTOs;
using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.WebControllers;

public class BarberShopController(IBarberShopService barberShopContext) : Controller
{
    [HttpGet]
    [Route("/barbershops")]
    public IActionResult BarberShop()
    {
        return View("~/Views/BarberShop/ShowBarberShops.cshtml");
    }
    
    [HttpGet]
    
    [Route("/barbershops/register")]
    public IActionResult RegisterBarberShop()
    {
        return View("~/Views/BarberShop/RegisterBarberShop.cshtml");    
    }
    
    [HttpGet("barbershop/details/{barberShopId}")]
    public async Task<IActionResult> Details(int barberShopId) 
    {
        var barberShop = await barberShopContext.GetBarberShopByIdAsync(barberShopId);
        
        if (barberShop == null)
        {
            return NotFound();
        }
        return View("~/Views/BarberShop/BarberShopDetails.cshtml", barberShop);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromForm] BarberShopRegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            // Se houver erro de validação, retorna para a mesma tela com os dados
            return View(request);
        }
        
        // Pega o ID do usuário logado (exemplo)
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return Unauthorized("Sessão inválida.");
        }

        var newBarberShop = await barberShopContext.RegisterBarberShopAsync(userId, request);

        if (newBarberShop == null)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível cadastrar a barbearia.");
            return View(request);
        }

        TempData["SuccessMessage"] = "Barbearia cadastrada com sucesso!";
        return RedirectToAction("Dashboard", "NavBar"); // Redireciona para o dashboard
    }
}