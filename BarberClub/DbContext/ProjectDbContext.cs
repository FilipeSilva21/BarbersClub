using BarberClub.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberClub.DbContext;

public class ProjectDbContext(DbContextOptions<ProjectDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<BarberShop> BarberShops { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.BarberShops)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>(); 
    }
}