using Repository.Models;

namespace BarbersClub.Business.DTOs;

public record RatingViewResponse
{
    public int RatingId { get; set; }
    public decimal RatingValue { get; set; }
    public string Comment { get; set; }
    public string ClientName { get; set; }
    public DateTime? CreatedAt { get; set; }
    
    public int UserId { get; set; }
    public BarberShop BarberShop { get; set; }
}