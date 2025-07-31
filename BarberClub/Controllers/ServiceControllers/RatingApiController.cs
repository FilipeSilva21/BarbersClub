using BarberClub.DTOs;
using BarberClub.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberClub.Controllers.ServiceControllers;


[Route("ratingsApi")]
[ApiController]
public class RatingsApiController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingsApiController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRating([FromBody] RatingRegisterRequest request)
    {
        var createdRating = await _ratingService.CreateRatingAsync(request);
        return Ok(createdRating);
    }

    [HttpGet("barbershop/{barberShopId}")]
    public async Task<IActionResult> GetRatingsByBarberShop(int barberShopId)
    {
        var ratings = await _ratingService.GetRatingsByBarberShopAsync(barberShopId);
        return Ok(ratings);
    }
}