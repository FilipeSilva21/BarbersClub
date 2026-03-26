using BarbersClub.Business.DTOs;
using Repository.Models;

namespace BarbersClub.Business.Services.Interfaces;

public interface IRatingService
{
    Task<Rating?> CreateRatingAsync(RatingRegisterRequest request);
    Task<IEnumerable<RatingViewResponse>> GetRatingsByBarberShopAsync(int barberShopId);
}