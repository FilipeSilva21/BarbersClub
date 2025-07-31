namespace BarberClub.DTOs;

public record RatingViewResponse
{
    public int RatingId { get; set; }
    public decimal RatingValue { get; set; }
    public string Comment { get; set; }
    public string ClientName { get; set; }
}