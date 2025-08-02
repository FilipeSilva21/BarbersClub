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
                .WithMany(u => u.Services) // Lado oposto da relação
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Service -> BarberShop (N-1)
            serviceEntity.HasOne(s => s.BarberShop)
                .WithMany(bs => bs.Services) // Lado oposto da relação
                .HasForeignKey(s => s.BarberShopId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade é o padrão para chaves obrigatórias
        });

        // --- Configuração de Rating ---
        modelBuilder.Entity<Rating>(ratingEntity =>
        {
            // Rating -> Client (User) (N-1)
            ratingEntity.HasOne(r => r.Client)
                .WithMany(u => u.Ratings) // Lado oposto da relação
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rating -> BarberShop (N-1)
            ratingEntity.HasOne(r => r.BarberShop)
                .WithMany(bs => bs.Ratings) // Lado oposto da relação
                .HasForeignKey(r => r.BarberShopId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rating -> Service (N-1)
            ratingEntity.HasOne(r => r.Service)
                .WithMany(s => s.Ratings) // Lado oposto da relação
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Configuração de Image ---
        modelBuilder.Entity<Image>(imageEntity =>
        {
            // Image -> Service (N-1)
            imageEntity.HasOne(i => i.Service)
                .WithMany(s => s.Images) // Lado oposto da relação
                .HasForeignKey(i => i.ServiceId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade é mais comum aqui
        });

        // --- Configuração de Conversão de Enum em BarberShop ---
        modelBuilder.Entity<BarberShop>()
            .Property(e => e.OfferedServices)
            .HasConversion(
                v => string.Join(',', v.Select(e => (int)e)),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => (Models.Enums.Services)Enum.Parse(typeof(Models.Enums.Services), s))
                    .ToList()
            );
    }
}