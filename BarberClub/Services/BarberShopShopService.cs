using Microsoft.EntityFrameworkCore;
using BarberClub.DTOs;
using BarberClub.Models;

namespace BarberClub.Services;

public class BarberShopShopService : IBarberShopService
{
    private readonly DbContext.ProjectDbContext _context;

    public BarberShopShopService(DbContext.ProjectDbContext context)
    {
        _context = context;
    }
    
    public async Task<BarberShop?> RegisterBarberShop(int userId, BarberShopRegisterRequest request)
    {
        var userExists= await _context.Users.AnyAsync(u => u.UserId == userId);
        
        if (!userExists)
            return null;

        var barberShop = new BarberShop()
        {
            UserId = userId,
            Name = request.Name,
            State = request.State,
            City = request.City,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber
        };

        _context.BarberShops.Add(barberShop);
        await _context.SaveChangesAsync();

        return barberShop;
    }

    public async Task<BarberShop?> GetBarberShopById(int barberShopId)
    {
        return await _context.BarberShops
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.BarberShopId == barberShopId);
    }

    public async Task<IEnumerable<BarberShop?>> GetBarberShopsByState(string state)
    {
        return await _context.BarberShops
            .Where(b => b.State == state)
            .ToListAsync();
    }

    public async Task<IEnumerable<BarberShop?>> GetBarberShopsByCity(string city)
    {
        return await _context.BarberShops
            .Where(b => b.City == city)
            .ToListAsync();
    }

    public async Task<IEnumerable<BarberShop?>> GetAllBarberShops()
    {
        return await _context.BarberShops
            .AsNoTracking()
            .ToListAsync();
            
    }

   public async Task<IEnumerable<BarberShop?>> GetAllBarberShopsByUserId(int userId)
   {
       return await _context.BarberShops
           .Include(b => b.User) 
           .Where(b => b.UserId == userId)
           .ToListAsync(); 
   }

    public Task<BarberShop?> UpdateBarberShop(int barberShopId, int userId, BarberShopUpdateRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteBarberShop(int barberShopId, int userId)
    {
        var barberShop = await _context.BarberShops.FindAsync(barberShopId);

        if (barberShop == null)
            return false; 
        
        if (barberShop.UserId != userId)
            return false;

        _context.BarberShops.Remove(barberShop);
        await _context.SaveChangesAsync();
        
        return true;
    }
}