using System.Security.Claims;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.WebControllers;

public class BarberShopController : Controller
{
    private readonly IBarberShopService _service;
    
    public BarberShopController (IBarberShopService barberShopService)
    {
        _service = barberShopService;
    }
        
    [HttpGet]
    [Route("/barbershop/register")]
    public IActionResult RegisterBarberShop()
    {
        return View("~/Views/BarberShop/RegisterBarberShop.cshtml");    
    }

    [HttpPost]
    [Authorize]
    [Route("/barbershop/register")]
    public async Task<IActionResult> Create([FromForm] BarberShopRegisterRequest requestDto)
    {
        if (!ModelState.IsValid)
        {
            // Se houver um erro, é importante retornar a view com os dados que o usuário já preencheu.
            // Para isso, precisamos de um objeto BarberShop.
            var modelComErros = new BarberShop
            {
                Name = requestDto.Name,
                Address = requestDto.Address,
                City = requestDto.City,
                State = requestDto.State,
                PhoneNumber = requestDto.PhoneNumber
            };
            return View(modelComErros);
        }

        // Pega o ID do usuário logado a partir do Token JWT
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            return Unauthorized("ID de usuário inválido no token.");
        }

        var novaBarbearia = await _service.RegisterBarberShop(userId, requestDto);

        if (novaBarbearia == null)
        {
            ModelState.AddModelError(string.Empty, "Ocorreu um erro ao registrar a barbearia. Usuário não encontrado.");
            // Mapeia o DTO de volta para o modelo para retornar à view com os dados
            var modelComErros = new BarberShop
            {
                Name = requestDto.Name,
                Address = requestDto.Address,
                City = requestDto.City,
                State = requestDto.State,
                PhoneNumber = requestDto.PhoneNumber
            };
            return View(modelComErros);
        }

        // Redireciona para uma página de sucesso
        return RedirectToAction("Index", "Dashboard");
    }

}
