namespace BarberClub.DTOs;

public record BarberShopRegisterRequest()
{
    public string Name { get; set; } = string.Empty;
    
    public string State { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string Instagram { get; set; } = string.Empty;
    
    public string WhatsApp { get; set; } = string.Empty;

    public string? WorkingDays { get; set; }

    public string? OpeningHours { get; set; } 
    
    public string? ClosingHours { get; set; }
    
    public List<Models.Enums.Services> OfferedServices { get; set; } = new();
}