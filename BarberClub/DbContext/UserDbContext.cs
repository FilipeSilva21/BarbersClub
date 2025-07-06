using BarberClub.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberClub.DbContext;

public class UserDbContext(DbContextOptions<UserDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();
    }
}