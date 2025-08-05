using System.ComponentModel.DataAnnotations.Schema;

namespace BarberClub.Models;

public class DashboardStats
{
    public int ServicesToday { get; set; }
    public decimal RevenueToday { get; set; }
    public int NewClientsMonth { get; set; }
    public decimal AverageRating { get; set; }
}   