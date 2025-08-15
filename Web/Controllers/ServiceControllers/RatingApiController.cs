using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbersClub.Controllers.ServiceControllers;


[Route("api/ratings")]
[ApiController]
public class RatingsApiController(IRatingService ratingService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateRating([FromForm] RatingRegisterRequest request)
    {
        var createdRating = await ratingService.CreateRatingAsync(request);
        
        return Ok(createdRating);
    }
    
    [HttpGet("barbershop/{barberShopId}")]
    public async Task<IActionResult> GetRatingsByBarberShop(int barberShopId)
    {
        var ratings = await ratingService.GetRatingsByBarberShopAsync(barberShopId);
        
        return Ok(ratings);
    }
}