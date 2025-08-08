using Microsoft.EntityFrameworkCore;
using BarberClub.DbContext;
using BarberClub.DTOs;
using BarberClub.Models;

namespace BarberClub.Services;

public class RatingService(ProjectDbContext context) : IRatingService
{
    public async Task<Rating?> CreateRatingAsync(RatingRegisterRequest request)
    {
        var newRating = new Rating
        {
            RatingValue = request.RatingValue,
            Comment = request.Comment,
            ServiceId = request.ServiceId,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

        context.Ratings.Add(newRating);
        await context.SaveChangesAsync();

        await context.Entry(newRating).Reference(r => r.Client).LoadAsync();

        return newRating;
    }

    public async Task<IEnumerable<RatingViewResponse>> GetRatingsByBarberShopAsync(int barberShopId)
    {
        return await context.Ratings
            .Where(r => r.Service.BarberShopId == barberShopId)
            .Include(r => r.Client)
            .Select(r => new RatingViewResponse()
            {
                RatingId = r.RatingId,
                RatingValue = r.RatingValue,
                Comment = r.Comment,
                ClientName = r.Client.FirstName
            })
            .ToListAsync();
    }
}