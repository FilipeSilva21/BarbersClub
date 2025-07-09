namespace BarberClub.DTOs;

public record BarberShopUpdateRequest()
{
    public string Name { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
}