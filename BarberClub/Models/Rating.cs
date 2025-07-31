using System.ComponentModel.DataAnnotations.Schema;

namespace BarberClub.Models;

public class Rating
{
    public int RatingId { get; set; }
    
    public Decimal RatingValue { get; set; }
    
    public string Comment { get; set; } = string.Empty;
    
    public int BarberShopId { get; set; }
    [ForeignKey("BarberShopId")]
    public virtual BarberShop BarberShop { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User Client { get; set; }
}