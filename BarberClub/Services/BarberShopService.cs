using System.Collections;
using Microsoft.EntityFrameworkCore;
using BarberClub.DTOs;
using BarberClub.Models;

namespace BarberClub.Services;

public class BarberShopService : IBarberShopService
{
    private readonly DbContext.ProjectDbContext _context;

    public BarberShopService(DbContext.ProjectDbContext context)
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
        
        await _context.Entry(barberShop).Reference(b => b.Barber).LoadAsync();
 
        return barberShop;
    }

    public async Task<BarberShop?> GetBarberShopById(int barberShopId)
    {
        return await _context.BarberShops
            .Include(b => b.Barber)
            .FirstOrDefaultAsync(b => b.BarberShopId == barberShopId);
    }

    public async Task<IEnumerable<BarberShop?>> GetBarberShops()
    {
        return await _context.BarberShops.ToListAsync();
    }

    public async Task<IEnumerable<BarberShop?>> SearchBarberShops(string? barberShopName, string? state, string? city, string? barberName)
    {
        IQueryable<BarberShop> query = _context.BarberShops; 

        if (!string.IsNullOrEmpty(barberShopName))
        {
            query = query.Where(b => b.Name.Contains(barberShopName));
        }

        if (!string.IsNullOrEmpty(state))
        {
            query = query.Where(b => b.State == state);
        }

        if (!string.IsNullOrEmpty(city))
        {
            query = query.Where(b => b.City == city);
        }

        if (!string.IsNullOrEmpty(barberName))
        {
            query = query.Where(b => b.Barber.FirstName.Contains(barberName));
        }
    
        return await query.ToListAsync();
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