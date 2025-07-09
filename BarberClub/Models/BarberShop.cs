using System.ComponentModel.DataAnnotations.Schema;

namespace BarberClub.Models;

public class BarberShop
{
    public int BarberShopId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string State { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}