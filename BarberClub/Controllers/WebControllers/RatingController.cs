// using System.Security.Claims;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace BarberClub.Controllers.WebControllers
// {
//     [Route("postRatings")]  
//     public class RatingsController : Controller
//     {
//         [HttpGet]
//         public IActionResult CreateRatings(int barberShopId)
//         {
//             if (barberShopId <= 0)
//             {
//                 return BadRequest("O ID da barbearia é inválido.");
//             }
//
//             ViewBag.BarberShopId = barberShopId;
//             ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
//             
//             return View("~/Views/Service/Rating/CreateRating.cshtml");
//         }
//     }
// }