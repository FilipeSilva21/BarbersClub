namespace BarberClub.DTOs;

public record RatingRegisterRequest
{
    public decimal RatingValue { get; set; }
    public string Comment { get; set; }
    public int BarberShopId { get; set; }
    public int UserId { get; set; }
}