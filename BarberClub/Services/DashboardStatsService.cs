using BarberClub.DTOs;
using BarberClub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BarberClub.DbContext;
using BarberClub.Models;

namespace BarberClub.Services;

public class DashboardStatsService : IDashboardStatsService
{
    private readonly ProjectDbContext _context;

    public DashboardStatsService(ProjectDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsResponse> GetDashboardStatsAsync(int? barberShopId)
    {
        var barberShop = await _context.BarberShops.FindAsync(barberShopId);
        
        if (barberShop == null)
        {
            return null;
        }

        var today = DateTime.UtcNow.Date;
        
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        var serviceIdsForShop = await _context.Services
            .Where(s => s.BarberShopId == barberShopId)
            .Select(s => s.ServiceId)
            .ToListAsync();

        var ratingsQuery = _context.Ratings
            .Where(r => serviceIdsForShop.Contains(r.ServiceId)); 
        
        var averageRating = await ratingsQuery.AnyAsync()
            ? await ratingsQuery.AverageAsync(r => r.RatingValue)
            : 0;

        var newClientsMonth = await ratingsQuery
            .Where(r => r.CreatedAt >= startOfMonth)
            .Select(r => r.UserId)
            .Distinct()
            .CountAsync();

        var todayServicesQuery = _context.Services
            .Where(s => s.BarberShopId == barberShopId && s.Date == today);
            
        var servicesTodayCount = await todayServicesQuery.CountAsync();
        var revenueToday = await todayServicesQuery.SumAsync(s => s.Price);

        var statsDto = new DashboardStatsResponse
        {
            BarberShop = barberShop,
            Stats = new DashboardStats
            {
                AverageRating = Math.Round(averageRating, 1),
                NewClientsMonth = newClientsMonth,
                ServicesToday = servicesTodayCount,
                RevenueToday = revenueToday
            }
        };

        return statsDto;
    }
}