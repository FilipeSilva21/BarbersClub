using Microsoft.EntityFrameworkCore;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Models.Enums;
using BarbersClub.Services.Interfaces;
using BarbersClub.DbContext;
using Microsoft.AspNetCore.Http;

namespace BarberClub.Services;

public class ServiceService(ProjectDbContext context, IWebHostEnvironment webHostEnvironment) : IServiceService
{
   
    public async Task<ServiceViewResponse> CreateServiceAsync(ServiceRegisterRequest request)
    {
        var barberShop = await context.BarberShops.FindAsync(request.BarberShopId);
        var client = await context.Users.FindAsync(request.UserId);
        var offeredService = await context.OfferedServices
            .FirstOrDefaultAsync(os => os.BarberShopId == request.BarberShopId && os.ServiceType == request.Services);

        if (offeredService == null)
            throw new InvalidOperationException("Este serviço não é oferecido pela barbearia selecionada.");

        if (barberShop == null || client == null)
            throw new KeyNotFoundException("Barbearia ou cliente não encontrado.");

        var newService = new Service
        {
            Date = request.Date,
            Time = request.Time,
            Services = request.Services,
            Price = offeredService.Price,
            Description = request.Description,  
            BarberShopId = request.BarberShopId,
            UserId = request.UserId,
            Status = ServiceStatus.Confirmado
        };
    
        context.Services.Add(newService);
        await context.SaveChangesAsync();

        return new ServiceViewResponse();
    }
    
    public async Task<Service?> GetServiceByIdAsync(int serviceId)
    {
        return await context.Services  
            .Include(bs => bs.Ratings)
            .Include(bs => bs.BarberShop)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.ServiceId == serviceId);
    }

    public async Task<IEnumerable<ServiceViewResponse>> GetServicesByBarberShopAsync(
        int barberShopId, 
        string? clientName, 
        string? serviceType, 
        DateTime? startDate, 
        DateTime? endDate,
        TimeSpan? time)
    {
        var query = context.Services
            .Where(s => s.BarberShopId == barberShopId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(clientName))
            query = query.Where(s => s.Client.FirstName.Contains(clientName) || s.Client.LastName.Contains(clientName));
        
        if (!string.IsNullOrEmpty(serviceType))
            query = query.Where(s => s.Services.ToString().Contains(serviceType));
        
        if (startDate.HasValue)
            query = query.Where(s => s.Date.Date >= startDate.Value.Date);
        
        if (endDate.HasValue)
            query = query.Where(s => s.Date.Date <= endDate.Value.Date);
        
        if (time.HasValue)
            query = query.Where(s => s.Time == time.Value);

        return await query
            .Include(s => s.Client)
            .Include(s => s.BarberShop).ThenInclude(bs => bs.OfferedServices) 
            .OrderByDescending(s => s.Date).ThenBy(s => s.Time)
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
                Status = s.Status.ToString(),
                ServiceImageUrl = s.ServiceImageUrl,
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
        string? serviceType, 
        string? barber, 
        DateTime? startDate, 
        DateTime? endDate,
        bool? hasPhoto, 
        string? sortBy)
    {
        var query = context.Services.AsQueryable();

        if (!string.IsNullOrEmpty(barberShopName))
            query = query.Where(s => s.BarberShop.Name.Contains(barberShopName));
        
        if (!string.IsNullOrEmpty(serviceType))
            query = query.Where(s => s.Services.ToString() == serviceType);
        
        if (startDate.HasValue)
            query = query.Where(s => s.Date.Date >= startDate.Value.Date);
        
        if (endDate.HasValue)
            query = query.Where(s => s.Date.Date <= endDate.Value.Date);
        
        if (hasPhoto == true)
            query = query.Where(s => !string.IsNullOrEmpty(s.ServiceImageUrl));
        
        if (!string.IsNullOrEmpty(barber))
            query = query.Where(s => s.BarberShop.Barber != null &&
                                     (s.BarberShop.Barber.FirstName + " " + s.BarberShop.Barber.LastName).Contains(barber));
        
        switch (sortBy)
        {
            case "ratings_desc":
                query = query.Include(s => s.Ratings)
                    .OrderByDescending(s => s.Ratings.Any() ? s.Ratings.Average(r => r.RatingValue) : 0);
                break;
            case "date_desc":
            default:
                query = query.OrderByDescending(s => s.Date);
                break;
        }
        
        var result = await query
            .Include(s => s.BarberShop).ThenInclude(bs => bs.Barber) 
            .Include(s => s.Client)
            .Select(s => new ServiceViewResponse()
            {
                ServiceId = s.ServiceId,
                Date = s.Date,
                ServiceType = s.Services.ToString(),
                Description = s.Description,
                BarberShopId = s.BarberShopId,
                BarberShopName = s.BarberShop.Name,
                ClientId = s.UserId,
                ClientName = s.Client.FirstName,
                ServiceImageUrl = s.ServiceImageUrl,
                Barber = s.BarberShop.Barber != null 
                    ? s.BarberShop.Barber.FirstName + " " + s.BarberShop.Barber.LastName 
                    : "Não informado"
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
            .Include(s => s.Ratings)
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
                ClientName = s.Client.FirstName,
                HasRating = s.Ratings.Any()
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
    
    public async Task<IEnumerable<ServiceViewResponse>> GetServicesByUserAndStatusAsync(int userId, string status)
    {
        if (!Enum.TryParse<ServiceStatus>(status, true, out var statusEnum))
            return new List<ServiceViewResponse>(); 

        return await context.Services
            .Where(s => s.UserId == userId && s.Status == statusEnum)
            .Include(s => s.BarberShop)
            .Include(s => s.Client) 
            .OrderByDescending(s => s.Date)
            .Select(s => new ServiceViewResponse
            {
                ServiceId = s.ServiceId,
                Date = s.Date,
                Time = s.Time,
                ServiceType = s.Services.ToString(),
                Description = s.Description,
                Price = s.Price,
                BarberShopId = s.BarberShopId,
                BarberShopName = s.BarberShop.Name,
                ClientId = s.UserId,
                ClientName = s.Client.FirstName,
                Status = s.Status.ToString()
            })
            .ToListAsync();
    }

    public async Task<bool> CancelServiceAsync(int serviceId, int userId)
    {
        var service = await context.Services.FirstOrDefaultAsync(s => s.ServiceId == serviceId);

        if (service == null || service.UserId != userId)
            return false;
        
        if (service.Status != ServiceStatus.Confirmado)
            return false;

        service.Status = ServiceStatus.Cancelado;
        await context.SaveChangesAsync();

        return true;
    }
    
    public async Task<bool> ConcludeServiceAsync(int serviceId)
    {
        var service = await context.Services.FindAsync(serviceId);

        if (service == null || service.Status != ServiceStatus.Confirmado)
        {
            return false; 
        }

        service.Status = ServiceStatus.Concluido;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ConcludeServiceAsync(int serviceId, string photoPath)
    {
        var service = await context.Services.FindAsync(serviceId);
    
        if (service == null || service.Status != ServiceStatus.Confirmado)
        {
            return false;
        }

        if (string.IsNullOrEmpty(photoPath))
        {
            return false;
        }

        service.Status = ServiceStatus.Concluido;
        service.ServiceImageUrl = photoPath; 
        
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Service?> UpdateServiceAsync(int serviceId, ServiceUpdateRequest request)
    {
        var service = await context.Services.FindAsync(serviceId);
        if (service == null)
            return null;

        if (request.Description != null)
            service.Description = request.Description;

        if (request.Status.HasValue)
            service.Status = request.Status.Value;

        if (request.UploadedImage != null && request.UploadedImage.Length > 0)
        {
            if (!string.IsNullOrEmpty(service.ServiceImageUrl))
            {
                var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, service.ServiceImageUrl.TrimStart('/'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.UploadedImage.FileName);
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "services");
            Directory.CreateDirectory(uploadsFolder); 
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.UploadedImage.CopyToAsync(fileStream);
            }

            service.ServiceImageUrl = $"/images/services/{uniqueFileName}";
        }

        await context.SaveChangesAsync();
        return service;
    }
}