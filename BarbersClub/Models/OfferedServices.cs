using System.ComponentModel.DataAnnotations.Schema;

namespace BarberClub.Models;

public class OfferedService
{
    public int OfferedServiceId { get; set; }
    public Enums.ServiceTypes ServiceTypeType { get; set; }
    public decimal Price { get; set; }

    public int BarberShopId { get; set; }
    [ForeignKey("BarberShopId")]
    public virtual BarberShop BarberShop { get; set; }
}