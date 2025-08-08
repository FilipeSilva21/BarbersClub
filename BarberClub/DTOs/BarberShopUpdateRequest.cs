namespace BarberClub.DTOs;

public record BarberShopUpdateRequest()
{
    public string Name { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public string Instagram { get; set; } = string.Empty;
    
    public string? OpeningHours { get; set; } 
    
    public string? ClosingHours { get; set; }
    
    public IFormFile? ProfilePictureFile { get; set; }
}