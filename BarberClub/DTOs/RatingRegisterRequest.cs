namespace BarberClub.DTOs;

public record RatingRegisterRequest
{
    public decimal RatingValue { get; set; }
    public string Comment { get; set; }
    public int ServiceId { get; set; }
    public int UserId { get; set; }
}