using Microsoft.EntityFrameworkCore;
using BarberClub.DbContext;
using BarberClub.DTOs;
using BarberClub.Models;

namespace BarberClub.Services;

public class RatingService : IRatingService
{
    private readonly ProjectDbContext _context;

    public RatingService(ProjectDbContext context)
    {
        _context = context;
    }

    public async Task<RatingViewResponse> CreateRatingAsync(RatingRegisterRequest request)
    {
        var newRating = new Rating
        {
            RatingValue = request.RatingValue,
            Comment = request.Comment,
            BarberShopId = request.BarberShopId,
            UserId = request.UserId
        };

        _context.Ratings.Add(newRating);
        await _context.SaveChangesAsync();

        await _context.Entry(newRating).Reference(r => r.Client).LoadAsync();
        
        return new RatingViewResponse()
        {
            RatingId = newRating.RatingId,
            RatingValue = newRating.RatingValue,
            Comment = newRating.Comment,
            ClientName = newRating.Client.FirstName
        };
    }

    public async Task<IEnumerable<RatingViewResponse>> GetRatingsByBarberShopAsync(int barberShopId)
    {
        return await _context.Ratings
            .Where(r => r.BarberShopId == barberShopId)
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