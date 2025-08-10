using BarberClub.DTOs;
using BarberClub.Models;

namespace BarbersClub.Services.Interfaces;

public interface IRatingService
{
    Task<Rating?> CreateRatingAsync(RatingRegisterRequest request);
    Task<IEnumerable<RatingViewResponse>> GetRatingsByBarberShopAsync(int barberShopId);
}