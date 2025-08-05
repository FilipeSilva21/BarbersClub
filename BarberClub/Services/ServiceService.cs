using Microsoft.EntityFrameworkCore;
using BarberClub.DbContext;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services.Interfaces;

namespace BarberClub.Services;

public class ServiceService(ProjectDbContext context) : IServiceService
{
    public async Task<ServiceViewResponse> CreateServiceAsync(ServiceRegisterRequest request)
    {
        var barberShop = await context.BarberShops.FindAsync(request.BarberShopId);
        var client = await context.Users.FindAsync(request.UserId);
        var offeredService = await context.OfferedServices
            .FirstOrDefaultAsync(os => os.BarberShopId == request.BarberShopId && os.ServiceType == request.Services);

        if (offeredService == null)
        {
            throw new InvalidOperationException("Este serviço não é oferecido pela barbearia selecionada.");
        }

        if (barberShop == null)
        {
            throw new KeyNotFoundException($"Barbearia com ID {request.BarberShopId} não encontrada.");
        }
        if (client == null)
        {
            throw new KeyNotFoundException($"Usuário com ID {request.UserId} não encontrado.");
        }

        var newService = new Service
        {
            Date = request.Date,
            Time = request.Time,
            Services = request.Services,
            Price = offeredService.Price,
            Description = request.Description,  
            BarberShopId = request.BarberShopId,
            UserId = request.UserId
        };
        context.Services.Add(newService);
        await context.SaveChangesAsync();

        return new ServiceViewResponse
        {
            ServiceId = newService.ServiceId,
            Date = newService.Date,
            Time = newService.Time,
            ServiceType = newService.Services.ToString(),
            Description = newService.Description,
            Price = newService.Price,
            BarberShopId = newService.BarberShopId,
            BarberShopName = barberShop.Name, 
            ClientId = newService.UserId,
            ClientName = client.FirstName    
        };
    }

    public async Task<IEnumerable<ServiceViewResponse>> GetServicesByBarberShopAsync(int barberShopId)
    {
        return await context.Services
            .Where(s => s.BarberShopId == barberShopId)
            .Include(s => s.Client)
            .Include(s => s.BarberShop).ThenInclude(bs => bs.OfferedServices) 
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
                ClientName = s.Client.FirstName,
                OfferedServices = s.BarberShop.OfferedServices.Select(os => new OfferedServiceResponse()
                {
                    ServiceType = os.ServiceType.ToString(),
                    Price = os.Price
                }).ToList()
            })
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ServiceViewResponse>> GetServicesAsync(
        string? barberShopName, 
        string? clientName, 
        string? serviceType, 
        DateTime? startDate, 
        DateTime? endDate)
    {
        var query = context.Services.AsQueryable();

        if (!string.IsNullOrEmpty(barberShopName))
        {
            query = query.Where(s => s.BarberShop.Name.Contains(barberShopName));
        }

        if (!string.IsNullOrEmpty(clientName))
        {
            query = query.Where(s => s.Client.FirstName.Contains(clientName) || s.Client.LastName.Contains(clientName));
        }

        if (!string.IsNullOrEmpty(serviceType))
        {
            query = query.Where(s => s.Services.ToString() == serviceType);
        }

        if (startDate.HasValue)
        {
            query = query.Where(s => s.Date.Date >= startDate.Value.Date);
        }

        if (endDate.HasValue)
        {
            query = query.Where(s => s.Date.Date <= endDate.Value.Date);
        }

        var result = await query
            .Include(s => s.BarberShop) 
            .Include(s => s.Client)  
            .OrderByDescending(s => s.Date)
            .Select(s => new ServiceViewResponse()
            {
                ServiceId = s.ServiceId,
                Date = s.Date,
                ServiceType = s.Services.ToString(),
                Description = s.Description,
                BarberShopId = s.BarberShopId,
                BarberShopName = s.BarberShop.Name,
                ClientId = s.UserId,
                ClientName = s.Client.FirstName 
            })
            .AsNoTracking() 
            .ToListAsync();

        return result;
    }
    
    public async Task<List<ServiceViewResponse>> GetServicesByUserAsync(int userId)
    {
         return await context.Services
            .Where(s => s.UserId == userId)
            .Include(s => s.BarberShop) 
            .OrderByDescending(s => s.Date)
            .Select(s => new ServiceViewResponse
            {
                ServiceId = s.ServiceId,
                Date = s.Date,
                Time = s.Time,
                ServiceType = s.Services.ToString(),
                Description = s.Description,
                BarberShopId = s.BarberShopId,
                BarberShopName = s.BarberShop.Name,
                ClientId = s.UserId,
                ClientName = s.Client.FirstName
            })
            .ToListAsync();
    }
    
    public async Task<IEnumerable<string>> GetBookedTimesAsync(int barberShopId, DateTime date)
    {
        var bookedTimeSpans = await context.Services
            .Where(s => s.BarberShopId == barberShopId && s.Date.Date == date.Date)
            .Select(s => s.Time) 
            .ToListAsync();

        var bookedTimes = bookedTimeSpans
            .Select(ts => ts.ToString("hh\\:mm\\:ss")) 
            .ToList();

        return bookedTimes;
    }
}