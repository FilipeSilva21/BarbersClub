namespace BarberClub.DTOs;

public record ServiceRegisterRequest
{
    public DateTime Date { get; set; }
    public DateTime Time { get; set; }
    public Models.Enums.Services Services { get; set; }
    
    public string Description { get; set; } = string.Empty;
    public int BarberShopId { get; set; }
    public int UserId { get; set; }
}