using Microsoft.EntityFrameworkCore;
using BarberClub.DbContext;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services.Interfaces;

namespace BarberClub.Services;

public class ServiceService : IServiceService
{
    private readonly ProjectDbContext _context;

    public ServiceService(ProjectDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceViewResponse> CreateServiceAsync(ServiceRegisterRequest request)
    {
        var newService = new Service
        {
            Date = request.Date,
            Time = request.Time,
            Services = request.Services,
            Description = request.Description,
            BarberShopId = request.BarberShopId,
            UserId = request.UserId
        };

        _context.Services.Add(newService);
        await _context.SaveChangesAsync();

        await _context.Entry(newService).Reference(s => s.BarberShop).LoadAsync();
        await _context.Entry(newService).Reference(s => s.Client).LoadAsync();
        
        return new ServiceViewResponse()
        {
            ServiceId = newService.ServiceId,
            Date = newService.Date,
            Time = newService.Time,
            ServiceType = newService.Services.ToString(),
            BarberShopId = newService.BarberShopId,
            BarberShopName = newService.BarberShop.Name,
            ClientId = newService.UserId,
            ClientName = newService.Client.FirstName
        };
    }

    public async Task<IEnumerable<ServiceViewResponse>> GetServicesByBarberShopAsync(int barberShopId)
    {
        return await _context.Services
            .Where(s => s.BarberShopId == barberShopId)
            .Include(s => s.Client) 
            .OrderByDescending(s => s.Date)
            .Select(s => new ServiceViewResponse()
            {
                ServiceId = s.ServiceId,
                Date = s.Date,
                Time = s.Time,
                ServiceType = s.Services.ToString(),
                BarberShopId = s.BarberShopId,
                BarberShopName = s.BarberShop.Name,
                ClientId = s.UserId,
                ClientName = s.Client.FirstName
            })
            .ToListAsync();
    }
    
    

    public async Task<IEnumerable<Service?>> GetServicesAsync()
    {
        return await _context.Services.ToListAsync();
    }
    
    public async Task<IEnumerable<ServiceViewResponse>> GetServicesByUserAsync(int userId)
    {
         return await _context.Services
            .Where(s => s.UserId == userId)
            .Include(s => s.BarberShop) 
            .OrderByDescending(s => s.Date)
            .Select(s => new ServiceViewResponse())
            .ToListAsync();
    }
}