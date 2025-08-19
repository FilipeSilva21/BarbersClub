using Microsoft.AspNetCore.Mvc;

namespace BarbersClub.Controllers.WebControllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;

    public IActionResult Index()
    {
        return View();
    }
}