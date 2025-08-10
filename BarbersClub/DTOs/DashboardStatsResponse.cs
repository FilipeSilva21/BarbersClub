using BarberClub.Models;

namespace BarberClub.DTOs;

public record DashboardStatsResponse
{
    public BarberShop BarberShop { get; set; }
    public DashboardStats Stats { get; set; } = new();

    public List<ServiceRegisterRequest> UpcomingService { get; set; } = new();
    public List<decimal> WeeklyRevenue { get; set; } = new();
}