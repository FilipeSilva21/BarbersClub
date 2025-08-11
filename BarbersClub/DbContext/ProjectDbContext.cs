using BarberClub.Models;
using BarberClub.Models.Enums; // Garanta que este using está correto
using Microsoft.EntityFrameworkCore;

namespace BarbersClub.DbContext;

public class ProjectDbContext(DbContextOptions<ProjectDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<BarberShop> BarberShops { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<OfferedService> OfferedServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Configuração de User ---
        modelBuilder.Entity<User>(userEntity =>
        {
            userEntity.HasKey(u => u.UserId);
            userEntity.HasIndex(u => u.Email).IsUnique();
            userEntity.Property(u => u.Role).HasConversion<string>();

            userEntity.HasMany(u => u.BarberShops)
                .WithOne(b => b.Barber)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Configuração de BarberShop ---
        modelBuilder.Entity<BarberShop>(barberShopEntity =>
        {
            barberShopEntity.HasKey(bs => bs.BarberShopId);

            barberShopEntity.HasMany(bs => bs.Services)
                .WithOne(s => s.BarberShop)
                .HasForeignKey(s => s.BarberShopId)
                .OnDelete(DeleteBehavior.Cascade);

            barberShopEntity.HasMany(bs => bs.OfferedServices)
                .WithOne(os => os.BarberShop)
                .HasForeignKey(os => os.BarberShopId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Configuração de Service (Agendamento) ---
        modelBuilder.Entity<Service>(serviceEntity =>
        {
            serviceEntity.HasKey(s => s.ServiceId);

            serviceEntity.Property(s => s.ServiceType)
                .HasConversion<string>();

            serviceEntity.Property(s => s.Status)
                .HasConversion<string>();

            // Relacionamento com User (Client)
            serviceEntity.HasOne(s => s.Client)
                .WithMany(u => u.Services)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // REMOVIDO: A relação com a entidade Image não existe mais.
            // serviceEntity.HasMany(s => s.Images)...
        });

        // --- Configuração de Rating ---
        modelBuilder.Entity<Rating>(ratingEntity =>
        {
            ratingEntity.HasKey(r => r.RatingId);

            ratingEntity.HasOne(r => r.Client)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            ratingEntity.HasOne(r => r.Service)
                .WithMany(s => s.Ratings)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Configuração de OfferedService ---
        modelBuilder.Entity<OfferedService>(offeredServiceEntity =>
        {
            offeredServiceEntity.HasKey(os => os.OfferedServiceId);
            offeredServiceEntity.HasIndex(os => new { os.BarberShopId, ServiceType = os.ServiceTypeType }).IsUnique();
            offeredServiceEntity.Property(os => os.ServiceTypeType).HasConversion<string>();
        });
    }
}