using System.Security.Claims;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.WebControllers;

[Route("barberShop")]
public class BarberShopController : Controller
{
    private readonly IBarberShopService _service;
    
    public BarberShopController (IBarberShopService barberShopService)
    {
        _service = barberShopService;
    }
        
    [HttpGet]
    [Route("register")]
    public IActionResult RegisterBarberShop()
    {
        return View("~/Views/BarberShop/RegisterBarberShop.cshtml");    
    }
}
