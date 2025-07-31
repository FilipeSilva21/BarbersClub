using BarberClub.Models.Enums;
using BarberClub.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BarberClub.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public int UserId { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PasswordHashed { get; set; } = string.Empty;
    
    public Roles Role { get; set; }
    
    public virtual ICollection<BarberShop> BarberShops { get; set; }
    
    public virtual ICollection<Service> Services { get; set; }
    
    public virtual ICollection<Rating> Ratings { get; set; }
}