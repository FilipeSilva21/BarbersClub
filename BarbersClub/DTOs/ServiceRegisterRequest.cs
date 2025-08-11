using BarberClub.Models.Enums;

namespace BarberClub.DTOs;

public record ServiceRegisterRequest
{
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public ServiceTypes ServiceTypes { get; set; }
    public string Description { get; set; } = string.Empty;
    public int BarberShopId { get; set; }
    public int UserId { get; set; }
}