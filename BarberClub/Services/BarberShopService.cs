using Microsoft.EntityFrameworkCore;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Models.Enums;
using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace BarberClub.Services;

public class BarberShopService(DbContext.ProjectDbContext context, IWebHostEnvironment webHostEnvironment) : IBarberShopService
{
    public async Task<BarberShop?> RegisterBarberShopAsync(int userId, BarberShopRegisterRequest request)
    {
        var userExists = await context.Users.AnyAsync(u => u.UserId == userId);
    
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
            OpeningHours = request.OpeningHours,
            ClosingHours = request.ClosingHours,
            WorkingDays = request.WorkingDays?.Distinct().ToList() ?? new List<WorkingDays>()
            
        };

        if (request.ProfilePicFile != null && request.ProfilePicFile.Length > 0)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ProfilePicFile.FileName);
        
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "barbershops");
            Directory.CreateDirectory(uploadsFolder);
        
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.ProfilePicFile.CopyToAsync(fileStream);
            }

            barberShop.ProfilePicUrl = $"/images/barbershops/{uniqueFileName}";
        }
        
        if (request.OfferedServices != null)
        {
            foreach (var serviceDto in request.OfferedServices)
            {
                if (Enum.TryParse<Models.Enums.Services>(serviceDto.ServiceType, ignoreCase:true, out var serviceEnum))
                {
                    barberShop.OfferedServices.Add(new OfferedService
                    {
                        ServiceType = serviceEnum,
                        Price = serviceDto.Price,
                        BarberShop = barberShop
                    });
                }
            }
        }
        
        context.BarberShops.Add(barberShop);
        await context.SaveChangesAsync();
        await context.Entry(barberShop).Reference(b => b.Barber).LoadAsync();

        return barberShop;
    }

    public async Task<BarberShop?> GetBarberShopByIdAsync(int barberShopId)
    {
        return await context.BarberShops
                .Include(bs => bs.Barber)    
                .Include(bs => bs.Services)  
                .Include(bs => bs.Ratings)
                .Include(bs => bs.OfferedServices)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BarberShopId == barberShopId);
    }

    public async Task<IEnumerable<BarberShop?>> GetBarberShopsAsync()
    {
        return await context.BarberShops.ToListAsync();
    }

    public async Task<IEnumerable<BarberShop?>> SearchBarberShopsAsync(string? barberShopName, string? state, string? city, string? barberName)
    {
        IQueryable<BarberShop> query = context.BarberShops; 

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
        var clients = await context.Services
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
        var barberShop = await context.BarberShops.FindAsync(barberShopId);

        if (barberShop == null)
            return false;

        if (barberShop.UserId != userId)
            return false;

        context.BarberShops.Remove(barberShop);
        await context.SaveChangesAsync();

        return true;
    }    
    
    public async Task<IEnumerable<BarberShop?>> GetBarberShopsByUserIdAsync(int userId)
    {
        return await context.BarberShops
            .Where(b => b.UserId == userId)
            .Include(b => b.Ratings)
            .ToListAsync();
    }
}