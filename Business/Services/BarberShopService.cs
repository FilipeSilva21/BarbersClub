using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;
using BarbersClub.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Repository.DbContext;
using Repository.Models;
using Repository.Models.Enums;

// Garanta que este using está presente

namespace BarbersClub.Business.Services;

public class BarberShopService(ProjectDbContext context, IWebHostEnvironment webHostEnvironment) : IBarberShopService
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
            WorkingDays = request.WorkingDays?.Distinct().ToList() ?? new List<WorkingDays>(),
            OfferedServices = new List<OfferedService>() 
        };

        if (request.ProfilePicFile != null && request.ProfilePicFile.Length > 0)
        {
            string uniqueFileName = Guid.NewGuid() + Path.GetExtension(request.ProfilePicFile.FileName);
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "barbershops");
            Directory.CreateDirectory(uploadsFolder);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.ProfilePicFile.CopyToAsync(fileStream);
            }
            barberShop.ProfilePicUrl = $"/images/barbershops/{uniqueFileName}";
        }
        
        if (request.OfferedServices is not null)
        {
            foreach (var serviceDto in request.OfferedServices)
            {
                if (Enum.TryParse<ServiceTypes>(serviceDto.ServiceType, ignoreCase:true, out var serviceEnum))
                {
                    barberShop.OfferedServices.Add(new OfferedService
                    {
                        ServiceType = serviceEnum, // Corrigido para ServiceType
                        Price = serviceDto.Price
                    });
                }
            }
        }
        
        context.BarberShops.Add(barberShop);
        await context.SaveChangesAsync();
        await context.Entry(barberShop).Reference(b => b.Barber).LoadAsync();

        return barberShop;
    }
    
    public async Task<BarberShop?> UpdateBarberShopAsync(int barberShopId, int userId, BarberShopUpdateRequest request)
    {
        var barberShopToUpdate = await context.BarberShops
            .Include(bs => bs.OfferedServices)
            .FirstOrDefaultAsync(bs => bs.BarberShopId == barberShopId);

        if (barberShopToUpdate == null || barberShopToUpdate.UserId != userId) 
            return null;

        barberShopToUpdate.Name = request.Name;
        barberShopToUpdate.Address = request.Address;
        barberShopToUpdate.City = request.City;
        barberShopToUpdate.State = request.State;
        barberShopToUpdate.WhatsApp = request.WhatsApp;
        barberShopToUpdate.Instagram = request.Instagram;
        barberShopToUpdate.OpeningHours = request.OpeningHours;
        barberShopToUpdate.ClosingHours = request.ClosingHours;
        barberShopToUpdate.WorkingDays = request.WorkingDays?.Distinct().ToList() ?? new List<WorkingDays>();

        if (request.ProfilePicFile != null && request.ProfilePicFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(barberShopToUpdate.ProfilePicUrl))
            {
                var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, barberShopToUpdate.ProfilePicUrl.TrimStart('/'));
                if (File.Exists(oldImagePath)) File.Delete(oldImagePath);
            }

            string uniqueFileName = Guid.NewGuid() + Path.GetExtension(request.ProfilePicFile.FileName);
            
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "barbershops");
            
            Directory.CreateDirectory(uploadsFolder);
            
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.ProfilePicFile.CopyToAsync(fileStream);
            }
            barberShopToUpdate.ProfilePicUrl = $"/images/barbershops/{uniqueFileName}";
        }
        
        var servicesToRemove = barberShopToUpdate.OfferedServices
            .Where(os => request.OfferedServices.All(dto => dto.ServiceType != os.ServiceType.ToString()))
            .ToList();
        context.OfferedServices.RemoveRange(servicesToRemove);

        foreach (var serviceDto in request.OfferedServices)
        {
            if (Enum.TryParse<ServiceTypes>(serviceDto.ServiceType, true, out var serviceEnum))
            {
                var existingService = barberShopToUpdate.OfferedServices
                    .FirstOrDefault(os => os.ServiceType == serviceEnum);
                
                if (existingService != null)
                {
                    existingService.Price = serviceDto.Price;
                }
                else
                {
                    barberShopToUpdate.OfferedServices.Add(new OfferedService
                    {
                        ServiceType = serviceEnum,
                        Price = serviceDto.Price
                    });
                }
            }
        }

        await context.SaveChangesAsync();
        return barberShopToUpdate;
    }

    public async Task<bool> DeleteBarberShopAsync(int barberShopId, int userId)
    {
        var barberShop = await context.BarberShops.FindAsync(barberShopId);

        if (barberShop == null || barberShop.UserId != userId)
            return false;

        context.BarberShops.Remove(barberShop);
        await context.SaveChangesAsync();
        return true;
    }

    // --- MÉTODOS DE LEITURA (AGORA RETORNAM DTOs) ---

    public async Task<BarberShopViewResponse?> GetBarberShopByIdAsync(int barberShopId)
    {
        return await context.BarberShops
            .AsNoTracking()
            .Where(b => b.BarberShopId == barberShopId)
            .Include(bs => bs.Services)
                .ThenInclude(s => s.Ratings)
                .ThenInclude(s => s.Client)
            .Select(bs => new BarberShopViewResponse
            {
                BarberShopId = bs.BarberShopId,
                UserId = bs.UserId,
                Name = bs.Name,
                Address = bs.Address,
                City = bs.City,
                State = bs.State,
                ProfilePicUrl = bs.ProfilePicUrl,
                AverageRating = bs.Services.SelectMany(s => s.Ratings).Any() 
                                ? bs.Services.SelectMany(s => s.Ratings).Average(r => r.RatingValue) 
                                : 0,
                ReviewCount = bs.Services.SelectMany(s => s.Ratings).Count(),
                WhatsApp = bs.WhatsApp,
                Instagram = bs.Instagram,
                OpeningHours = bs.OpeningHours,
                ClosingHours = bs.ClosingHours,
                BarberName = bs.Barber.FirstName + " " + bs.Barber.LastName,
                WorkingDays = bs.WorkingDays.Select(d => d.ToString()).ToList(),
                OfferedServices = bs.OfferedServices.Select(os => new OfferedServiceResponse 
                {
                    ServiceType = os.ServiceType.ToString(),
                    Price = os.Price
                }).ToList(),
                Ratings = bs.Services.SelectMany(s => s.Ratings)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RatingViewResponse()
                {
                ClientName = r.Client.FirstName,
                RatingValue = r.RatingValue,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<BarberShopViewResponse?>> GetBarberShopsAsync()
    { 
        return await context.BarberShops
            .AsNoTracking()
            .Include(bs => bs.Services).ThenInclude(s => s.Ratings)
            .Select(bs => new BarberShopViewResponse
            {
                BarberShopId = bs.BarberShopId,
                UserId = bs.UserId,
                Name = bs.Name,
                Address = bs.Address,
                City = bs.City,
                State = bs.State,
                ProfilePicUrl = bs.ProfilePicUrl,
                AverageRating = bs.Services.SelectMany(s => s.Ratings).Any() 
                                ? bs.Services.SelectMany(s => s.Ratings).Average(r => r.RatingValue) 
                                : 0,
                ReviewCount = bs.Services.SelectMany(s => s.Ratings).Count()
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<BarberShopViewResponse?>> SearchBarberShopsAsync(string? barberShopName, string? state, string? city, string? barberName)
    {
        IQueryable<BarberShop> query = context.BarberShops
            .Include(bs => bs.Services).ThenInclude(s => s.Ratings)
            .Include(bs => bs.Barber);

        if (!string.IsNullOrEmpty(barberShopName))
            query = query.Where(b => b.Name.Contains(barberShopName));
        if (!string.IsNullOrEmpty(state))
            query = query.Where(b => b.State == state);
        if (!string.IsNullOrEmpty(city))
            query = query.Where(b => b.City == city);
        if (!string.IsNullOrEmpty(barberName))
            query = query.Where(b => b.Barber.FirstName.Contains(barberName));
    
        return await query
            .Select(bs => new BarberShopViewResponse
            {
                BarberShopId = bs.BarberShopId,
                UserId = bs.UserId,
                Name = bs.Name,
                Address = bs.Address,
                City = bs.City,
                State = bs.State,
                ProfilePicUrl = bs.ProfilePicUrl,
                AverageRating = bs.Services.SelectMany(s => s.Ratings).Any() 
                                ? bs.Services.SelectMany(s => s.Ratings).Average(r => r.RatingValue) 
                                : 0,
                ReviewCount = bs.Services.SelectMany(s => s.Ratings).Count()
            })
            .ToListAsync();
    }
    
    public async Task<BarberShop?> GetBarberShopForUpdateAsync(int barberShopId)
    {
        return await context.BarberShops
            .Include(bs => bs.OfferedServices)
            .FirstOrDefaultAsync(b => b.BarberShopId == barberShopId);
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
    
    public async Task<IEnumerable<BarberShop?>> GetBarberShopsByUserIdAsync(int userId)
    {
        return await context.BarberShops
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<TopRatedBarberShopResponse>> GetTopRatedBarberShopAsync(int count)
    {
        return await context.BarberShops
            .AsNoTracking()
            .Where(bs => bs.Services.Any(s => s.Ratings.Any()))
            .OrderByDescending(bs => bs.Services.SelectMany(s => s.Ratings).Average(r => r.RatingValue))
            .Take(count)
            .Select(bs => new TopRatedBarberShopResponse() 
            {
                Id = bs.BarberShopId,
                Name = bs.Name,
                ProfilePictureUrl = bs.ProfilePicUrl,
                City = bs.City,
                State = bs.State,
            
                AverageRating = bs.Services.SelectMany(s => s.Ratings).Average(r => r.RatingValue), 
                ReviewCount = bs.Services.SelectMany(s => s.Ratings).Count(),
            
                Description = "Especialistas em cortes modernos e barboterapia." 
            })
            .ToListAsync();
    }
}