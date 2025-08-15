using BarbersClub.Business.DTOs;

namespace BarbersClub.Business.Services.Interfaces;

public interface IDashboardStatsService
{
    Task<DashboardStatsResponse> GetDashboardStatsAsync(int? barberShopId);
}