using BarberClub.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberClub.DbContext;

public class ProjectDbContext(DbContextOptions<ProjectDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<BarberShop> BarberShops { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Service> Services { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var userEntity = modelBuilder.Entity<User>();

        userEntity.HasIndex(u => u.Email).IsUnique();

        userEntity
            .Property(u => u.Role)
            .HasConversion<string>();

        userEntity
            .HasMany(u => u.BarberShops)
            .WithOne(b => b.Barber)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade); 
        
        
        var serviceEntity = modelBuilder.Entity<Service>();

        serviceEntity
            .HasOne(s => s.Client)
            .WithMany(u => u.Services) 
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict); 

        serviceEntity
            .HasOne(s => s.BarberShop)
            .WithMany(bs => bs.Services) 
            .HasForeignKey(s => s.BarberShopId); 

        var ratingEntity = modelBuilder.Entity<Rating>();

        ratingEntity
            .HasOne(r => r.Client)
            .WithMany(u => u.Ratings)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        ratingEntity
            .HasOne(r => r.BarberShop)
            .WithMany(bs => bs.Ratings) 
            .HasForeignKey(r => r.BarberShopId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}