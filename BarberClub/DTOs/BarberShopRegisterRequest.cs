namespace BarberClub.DTOs;

public record BarberShopRegisterRequest()
{
    public string Name { get; set; } = string.Empty;
    
    public string State { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
}