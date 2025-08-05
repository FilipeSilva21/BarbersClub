using BarberClub.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberClub.DbContext;

public class ProjectDbContext(DbContextOptions<ProjectDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<BarberShop> BarberShops { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Service> Services { get; set; }
    
    public DbSet<Image> Images { get; set; }
    public DbSet<OfferedService> OfferedServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    
        // --- Configuração de User ---
        modelBuilder.Entity<User>(userEntity =>
        {
            userEntity.HasIndex(u => u.Email).IsUnique();
            userEntity.Property(u => u.Role).HasConversion<string>();

            // User (Barber) -> BarberShops (1-N)
            userEntity.HasMany(u => u.BarberShops)
                .WithOne(b => b.Barber)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Configuração de Service ---
        modelBuilder.Entity<Service>(serviceEntity =>
        {
            // Service -> Client (User) (N-1)
            serviceEntity.HasOne(s => s.Client)
                .WithMany(u => u.Services) 
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Service -> BarberShop (N-1)
            serviceEntity.HasOne(s => s.BarberShop)
                .WithMany(bs => bs.Services) 
                .HasForeignKey(s => s.BarberShopId)
                .OnDelete(DeleteBehavior.Cascade); 
        });

        // --- Configuração de Rating ---
        modelBuilder.Entity<Rating>(ratingEntity =>
        {
            ratingEntity.HasOne(r => r.Client)
                .WithMany(u => u.Ratings) 
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rating -> BarberShop (N-1)
            ratingEntity.HasOne(r => r.BarberShop)
                .WithMany(bs => bs.Ratings) 
                .HasForeignKey(r => r.BarberShopId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rating -> Service (N-1)
            ratingEntity.HasOne(r => r.Service)
                .WithMany(s => s.Ratings) 
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Configuração de Image ---
        modelBuilder.Entity<Image>(imageEntity =>
        {
            // Image -> Service (N-1)
            imageEntity.HasOne(i => i.Service)
                .WithMany(s => s.Images) 
                .HasForeignKey(i => i.ServiceId)
                .OnDelete(DeleteBehavior.Cascade); 
        });
        
        modelBuilder.Entity<OfferedService>(offeredServiceEntity =>
        {
            offeredServiceEntity.HasOne(os => os.BarberShop)
                .WithMany(bs => bs.OfferedServices) 
                .HasForeignKey(os => os.BarberShopId) 
                .OnDelete(DeleteBehavior.Cascade); 
        });
    }
}