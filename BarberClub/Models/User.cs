using BarberClub.Models.Enums;
using BarberClub.Models.Enums;

namespace BarberClub.Models;

public class User
{
    public int Id { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PasswordHashed { get; set; } = string.Empty;
    
    public Roles Role { get; set; }
}