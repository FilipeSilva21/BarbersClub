using System.ComponentModel.DataAnnotations.Schema;

namespace BarberClub.Models;

public class Image
{
    public int ImageId { get; set; }

    public string Url { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public int ServiceId { get; set; } 
    [ForeignKey("ServiceId")]
    public virtual Service Service { get; set; }
}