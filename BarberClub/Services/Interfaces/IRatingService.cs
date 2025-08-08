using BarberClub.DTOs;
using BarberClub.Models;

namespace BarberClub.Services;

public interface IRatingService
{
    Task<Rating?> CreateRatingAsync(RatingRegisterRequest request);
    Task<IEnumerable<RatingViewResponse>> GetRatingsByBarberShopAsync(int barberShopId);
}