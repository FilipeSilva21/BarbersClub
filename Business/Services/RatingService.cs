using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;
using Business.Error_Handling;
using Microsoft.EntityFrameworkCore;
using Repository.DbContext;
using Repository.Models;

namespace Business.Services;

public class RatingService(ProjectDbContext context) : IRatingService
{
    public async Task<Rating?> CreateRatingAsync(RatingRegisterRequest request)
    {
        var service = await context.Services
            .AsNoTracking() 
            .Include(s => s.BarberShop)
            .FirstOrDefaultAsync(s => s.ServiceId == request.ServiceId);

        if (service is null)
            throw new ServiceNotFoundException(service.ServiceId); 

        var existingRating = await context.Ratings
            .AnyAsync(r => r.ServiceId == request.ServiceId);

        if (existingRating)
            throw new RatingAlreadyExistException(service.ServiceId); 

        var newRating = new Rating
        {
            RatingValue = request.RatingValue,
            Comment = request.Comment,
            ServiceId = request.ServiceId,
            UserId = service.UserId,
            BarberShopId = service.BarberShopId,
            CreatedAt = DateTime.UtcNow
        };

        context.Ratings.Add(newRating);
        await context.SaveChangesAsync();

        return newRating;
    }

    public async Task<IEnumerable<RatingViewResponse>> GetRatingsByBarberShopAsync(int barberShopId)
    {
        return await context.Ratings
            .Where(r => r.BarberShopId == barberShopId) 
            .Include(r => r.Client)
            .Include(r => r.BarberShop) 
            .Select(r => new RatingViewResponse()
            {
                RatingId = r.RatingId,
                RatingValue = r.RatingValue,
                Comment = r.Comment,
                ClientName = r.Client.FirstName,
                BarberShop = r.BarberShop,
            })
            .ToListAsync();
    }
}