using System.ComponentModel.DataAnnotations.Schema;
using Repository.Models.Enums;

namespace Repository.Models;

public class BarberShop
{
    public int BarberShopId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string State { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public string WhatsApp { get; set; } = string.Empty;
    
    public string Instagram { get; set; } = string.Empty;
    
    public string? OpeningHours { get; set; } 
    
    public string? ClosingHours { get; set; }
    
    public string? ProfilePicUrl { get; set; }
    
    public virtual ICollection<Service> Services { get; set; }
    
    public virtual ICollection<Rating> Ratings { get; set; }
    
    public List<WorkingDays> WorkingDays { get; set; } = new();
    
    public virtual ICollection<OfferedService> OfferedServices { get; set; } = new List<OfferedService>();
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User Barber { get; set; }
}