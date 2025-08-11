namespace BarberClub.DTOs;

public record TopRatedBarberShopResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public string? Description { get; set; }
};