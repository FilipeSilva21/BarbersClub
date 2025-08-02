using System.ComponentModel.DataAnnotations.Schema;
using BarberClub.Models.Enums;

namespace BarberClub.Models;

public class Service
{
    public int ServiceId { get; set; }
    
    public DateTime Date { get; set; }
    
    public DateTime Time { get; set; }

    public Enums.Services Services { get; set; }
    
    public String Description { get; set; } = string.Empty;
    
    public Decimal Price { get; set; }
    
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    
    public int BarberShopId { get; set; }
    [ForeignKey("BarberShopId")]
    public virtual BarberShop BarberShop { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User Client { get; set; }
    public virtual ICollection<Image> Images { get; set; } = new List<Image>(); 
}