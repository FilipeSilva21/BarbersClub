using System.Collections;
using Microsoft.EntityFrameworkCore;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services.Interfaces;

namespace BarberClub.Services;

public class BarberShopService : IBarberShopService
{
    private readonly DbContext.ProjectDbContext _context;

    public BarberShopService(DbContext.ProjectDbContext context)
    {
        _context = context;
    }
    
    public async Task<BarberShop?> RegisterBarberShopAsync(int userId, DTOs.BarberShopRegisterRequest request)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
    
        if (!userExists)
            return null;

        var barberShop = new BarberShop()
        {
            UserId = userId,
            Name = request.Name,
            State = request.State,
            City = request.City,
            Address = request.Address,
            Instagram = request.Instagram,
            WhatsApp = request.WhatsApp,
            WorkingDays = request.WorkingDays,
            OpeningHours = request.OpeningHours,
            ClosingHours = request.ClosingHours,
            OfferedServices = request.OfferedServices?.Distinct().ToList() ?? new List<Models.Enums.Services>()
        };

        _context.BarberShops.Add(barberShop);
        await _context.SaveChangesAsync();
    
        await _context.Entry(barberShop).Reference(b => b.Barber).LoadAsync();

        return barberShop;
    }

    public async Task<BarberShop?> GetBarberShopByIdAsync(int barberShopId)
    {
        return await _context.BarberShops
                .Include(bs => bs.Barber)    
                .Include(bs => bs.Services)  
                .Include(bs => bs.Ratings)   
                .FirstOrDefaultAsync(b => b.BarberShopId == barberShopId);
    }

    public async Task<IEnumerable<BarberShop?>> GetBarberShopsAsync()
    {
        return await _context.BarberShops.ToListAsync();
    }

    public async Task<IEnumerable<BarberShop?>> SearchBarberShopsAsync(string? barberShopName, string? state, string? city, string? barberName)
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

    public async Task<IEnumerable<User>> GetClientsByBarberShopAsync(int barberShopId)
    {
        var clients = await _context.Services
            .Where(s => s.BarberShopId == barberShopId)
            .Select(s => s.Client)
            .Distinct()
            .ToListAsync();

        return clients;
    }
    
    public Task<BarberShop?> UpdateBarberShopAsync(int barberShopId, int userId, BarberShopUpdateRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteBarberShopAsync(int barberShopId, int userId)
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
    
    public async Task<IEnumerable<BarberShop?>> GetBarberShopsByUserIdAsync(int userId)
    {
        return await _context.BarberShops
            .Where(b => b.UserId == userId)
            .Include(b => b.Ratings)
            .ToListAsync();
    }
}