using BarberClub.Models;

namespace BarberClub.DTOs;

public record ServiceViewResponse
{
    public int ServiceId { get; set; }
    public DateTime Date { get; set; }
    public DateTime Time { get; set; }
    public string ServiceType { get; set; }
    public int BarberShopId { get; set; }
    public string BarberShopName { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; }
    public Decimal Price { get; set; }
    public string Description { get; set; }
    public BarberShop BarberShop { get; set; }
}