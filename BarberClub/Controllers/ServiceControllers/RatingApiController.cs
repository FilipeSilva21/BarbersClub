using BarberClub.DTOs;
using BarberClub.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.ServiceControllers;


[Route("api/ratings")]
[ApiController]
public class RatingsApiController(IRatingService ratingService) : ControllerBase
{
    // [HttpPost]
    // public async Task<IActionResult> CreateRating([FromBody] RatingRegisterRequest request)
    // {
    //     var createdRating = await ratingService.CreateRatingAsync(request);
    //     
    //     return Ok(createdRating);
    // }
    //
    // [HttpGet("barbershop/{barberShopId}")]
    // public async Task<IActionResult> GetRatingsByBarberShop(int barberShopId)
    // {
    //     var ratings = await ratingService.GetRatingsByBarberShopAsync(barberShopId);
    //     
    //     return Ok(ratings);
    // }
}