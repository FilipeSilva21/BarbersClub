using Microsoft.AspNetCore.Http;
using Repository.Models.Enums;

namespace BarbersClub.Business.DTOs;

public record UserRegisterRequest()
{
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public string ConfirmPassword { get; set; } = string.Empty;
    
    public Roles Role { get; set; }
    
    public IFormFile? ProfilePic { get; set; }
}