namespace BarberClub.DTOs;

public record OfferedServiceResponse
{
    public string ServiceType { get; set; }
    public decimal Price { get; set; }
};