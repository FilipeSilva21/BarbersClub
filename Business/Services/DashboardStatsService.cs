using Microsoft.EntityFrameworkCore;
using BarberClub;
using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;
using BarbersClub.DbContext;
using Repository.DbContext;
using Repository.Models.Enums;

namespace BarbersClub.Business.Services;

public class DashboardStatsService(ProjectDbContext context) : IDashboardStatsService
{
    public async Task<DashboardStatsResponse> GetDashboardStatsAsync(int? barberShopId)
    {
        var barberShop = await context.BarberShops.FindAsync(barberShopId);
        
        if (barberShop is null)
            return null;

        var today = DateTime.UtcNow.Date;
        
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        var serviceIdsForShop = await context.Services
            .Where(s => s.BarberShopId == barberShopId)
            .Select(s => s.ServiceId)
            .ToListAsync();

        var ratingsQuery = context.Ratings
            .Where(r => serviceIdsForShop.Contains(r.ServiceId)); 
        
        var averageRating = await ratingsQuery.AnyAsync()
            ? await ratingsQuery.AverageAsync(r => r.RatingValue)
            : 0;

        var newClientsMonth = await ratingsQuery
            .Where(r => r.CreatedAt >= startOfMonth)
            .Select(r => r.UserId)
            .Distinct()
            .CountAsync();

        var todayServicesQuery = context.Services
            .Where(s => s.BarberShopId == barberShopId && s.Date == today);
            
        var servicesTodayCount = await todayServicesQuery.CountAsync();
        var revenueToday = await todayServicesQuery.Where(s => s.Status == ServiceStatus.Concluido).SumAsync(s => s.Price);

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