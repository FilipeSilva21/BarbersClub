using System.ComponentModel.DataAnnotations.Schema;
using BarberClub.Models.Enums;

namespace BarberClub.Models;

public class Service
{
    public int ServiceId { get; set; }
    
    public DateTime Date { get; set; }
    
    public DateTime Time { get; set; }

    public Enums.Services Services { get; set; }
    
    public String Desciption { get; set; } = string.Empty;
    
    public Decimal Price { get; set; }
    
    public int? RatingId { get; set; }
    [ForeignKey("RatingId")]
    public virtual Rating? Rating { get; set; }
    
    public int BarberShopId { get; set; }
    [ForeignKey("BarberShopId")]
    public virtual BarberShop BarberShop { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User Client { get; set; }
}