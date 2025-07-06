using BarberClub.Models.Enums;

namespace BarberClub.DTOs;

public record UserRegisterRequest()
{
    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public string ConfirmPassword { get; set; } = string.Empty;
    
    public Roles Role { get; set; }
}