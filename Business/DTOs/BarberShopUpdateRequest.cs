using Microsoft.AspNetCore.Http;
using Repository.Models.Enums;

namespace BarbersClub.Business.DTOs;

public record BarberShopUpdateRequest
{
    public int BarberShopId { get; set; } 
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string State { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string Instagram { get; set; } = string.Empty;
    
    public string WhatsApp { get; set; } = string.Empty;
    
    public string? OpeningHours { get; set; }
    
    public string? ClosingHours { get; set; }

    public IFormFile? ProfilePicFile { get; set; }

    public string? CurrentProfilePicUrl { get; set; }

    public List<WorkingDays> WorkingDays { get; set; } = new();
    
    public List<OfferedServiceResponse> OfferedServices { get; set; } = new();
}