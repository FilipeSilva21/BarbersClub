using System.Data.Entity;
using BarberClub.DbContext;
using BarberClub.DTOs;
using BarberClub.Models;

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
            BarberShopId = request.BarberShopId,
            UserId = request.UserId
        };

        _context.Services.Add(newService);
        await _context.SaveChangesAsync();

        // Carrega os dados relacionados para retornar no DTO
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
            .Include(s => s.Client) // Inclui dados do cliente
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
    
    // Implementação similar para GetServicesByUserAsync
    public async Task<IEnumerable<ServiceViewResponse>> GetServicesByUserAsync(int userId)
    {
         return await _context.Services
            .Where(s => s.UserId == userId)
            .Include(s => s.BarberShop) // Inclui dados da barbearia
            .OrderByDescending(s => s.Date)
            .Select(s => new ServiceViewResponse()
            {
                 // Mapeamento similar ao anterior
            })
            .ToListAsync();
    }
}