namespace BarbersClub.Business.DTOs;

public record ServiceViewResponse
{
    public int? ServiceId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public string? ServiceType { get; set; }
    
    public string Status { get; set; }
    public int BarberShopId { get; set; }
    public string BarberShopName { get; set; } 
    public int ClientId { get; set; }
    public string ClientName { get; set; }
    
    public string Barber { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    
    public string? ServiceImageUrl { get; set; }
    
    public bool HasRating { get; set; }
        
    public List<OfferedServiceResponse> OfferedServices { get; set; }
}