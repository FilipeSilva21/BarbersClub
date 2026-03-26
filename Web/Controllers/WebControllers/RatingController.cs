using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbersClub.Controllers.WebControllers
{
    [Route("/rate/{serviceId}")]  
    public class RatingsController : Controller
    {
        [HttpGet]
        [Authorize]
        public IActionResult CreateRatings(int serviceId)
        {
            if (serviceId <= 0)
            {
                return BadRequest("O ID do servico é inválido.");
            }

            ViewBag.ServiceId = serviceId;
            ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            
            return View("~/Views/Service/Rating/CreateRating.cshtml");
        }
    }
}