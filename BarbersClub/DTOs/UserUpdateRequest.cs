namespace BarberClub.DTOs;

public class UserUpdateRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    public string? NewPassword { get; set; }
    
    public string? ConfirmPassword { get; set; }
    
    public IFormFile? ProfilePicFile { get; set; }
    
    public string? CurrentProfilePicUrl { get; set; }
}