using BarbersClub.Business.DTOs;

namespace Business.DTOs;

public record BarberShopViewResponse
{
    public int BarberShopId { get; init; }
    public int UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Address { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string? ProfilePicUrl { get; init; }

    public decimal AverageRating { get; init; }
    public int ReviewCount { get; init; }

    public string WhatsApp { get; init; } = string.Empty;
    public string Instagram { get; init; } = string.Empty;
    public string? OpeningHours { get; init; }
    public string? ClosingHours { get; init; }
    public string BarberName { get; init; } = string.Empty;
    public List<string> WorkingDays { get; init; } = new();
    public List<OfferedServiceResponse> OfferedServices { get; init; } = new();
    public List<RatingViewResponse> Ratings { get; init; } = new();
}