namespace BarbersClub.Business.DTOs;

public record UserProfileResponse
{
    public int UserId { get; set; } 
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    
    public string? ProfilePicUrl { get; set; }
};
