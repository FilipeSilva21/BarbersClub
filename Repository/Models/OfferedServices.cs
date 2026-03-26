using System.ComponentModel.DataAnnotations.Schema;
using Repository.Models.Enums;

namespace Repository.Models;

public class OfferedService
{
    public int OfferedServiceId { get; set; }
    public ServiceTypes ServiceType { get; set; }
    public decimal Price { get; set; }

    public int BarberShopId { get; set; }
    [ForeignKey("BarberShopId")]
    public virtual BarberShop BarberShop { get; set; }
}