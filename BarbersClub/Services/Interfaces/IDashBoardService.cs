using BarberClub.DTOs;
using BarberClub.Models;

namespace BarbersClub.Services.Interfaces;

public interface IDashboardStatsService
{
    Task<DashboardStatsResponse> GetDashboardStatsAsync(int? barberShopId);
}