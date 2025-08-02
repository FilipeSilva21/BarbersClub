using BarberClub.DTOs;
using BarberClub.Models;

namespace BarberClub.Services.Interfaces;

public interface IDashboardStatsService
{
    Task<DashboardStatsResponse> GetDashboardStatsAsync(int? barberShopId);
}