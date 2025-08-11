using System.ComponentModel.DataAnnotations.Schema;
using BarberClub.Models.Enums;

namespace BarberClub.Models;

public class Service
{
    public int? ServiceId { get; set; }
    
    public DateTime Date { get; set; }
    
    public TimeSpan Time { get; set; }

    public ServiceTypes? ServiceType { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public Decimal Price { get; set; }
    
    public ServiceStatus Status { get; set; }
    
    public string? ServiceImageUrl { get; set; }
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    
    public int BarberShopId { get; set; }
    [ForeignKey("BarberShopId")]
    public virtual BarberShop BarberShop { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User Client { get; set; }
}