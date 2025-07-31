using BarberClub.DTOs;

namespace BarberClub.Services;

public interface IRatingService
{
    Task<RatingViewResponse> CreateRatingAsync(RatingRegisterRequest request);
    Task<IEnumerable<RatingViewResponse>> GetRatingsByBarberShopAsync(int barberShopId);
}