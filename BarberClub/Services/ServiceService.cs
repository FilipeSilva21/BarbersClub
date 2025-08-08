using Microsoft.EntityFrameworkCore;
using BarberClub.DbContext;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Models.Enums;
using BarberClub.Services.Interfaces;
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
            query = query.Where(s => s.BarberShop.Name.Contains(barberShopName));
        

        if (!string.IsNullOrEmpty(clientName))
            query = query.Where(s => s.Client.FirstName.Contains(clientName) || s.Client.LastName.Contains(clientName));
        

        if (!string.IsNullOrEmpty(serviceType))
            query = query.Where(s => s.Services.ToString() == serviceType);
        

        if (startDate.HasValue)
            query = query.Where(s => s.Date.Date >= startDate.Value.Date);
        

        if (endDate.HasValue)
            query = query.Where(s => s.Date.Date <= endDate.Value.Date);
        

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
    
    public async Task<bool> ConcludeServiceAsync(int serviceId, int userId)
    {
        var service = await context.Services.FirstOrDefaultAsync(s => s.ServiceId == serviceId);

        if (service == null || service.UserId != userId)
            return false;
        
        if (service.Status != ServiceStatus.Confirmado)
            return false;

        service.Status = ServiceStatus.Concluido;
        await context.SaveChangesAsync();

        return true;
    }
    
    public async Task<Service?> UpdateServiceAsync(int serviceId, ServiceUpdateRequest request)
    {
        // 1. Encontra o serviço existente no banco
        var service = await context.Services.FindAsync(serviceId);
        if (service == null)
        {
            return null; // ou throw new KeyNotFoundException(...);
        }

        // 2. Atualiza outras propriedades, se fornecidas
        if (request.Description != null)
        {
            service.Description = request.Description;
        }
        if (request.Status.HasValue)
        {
            service.Status = request.Status.Value;
        }

        // 3. Processa a nova imagem, se ela foi enviada
        if (request.UploadedImage != null && request.UploadedImage.Length > 0)
        {
            // Se já existe uma imagem, apaga a antiga para não acumular lixo
            if (!string.IsNullOrEmpty(service.ServiceImageUrl))
            {
                var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, service.ServiceImageUrl.TrimStart('/'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            // Salva a nova imagem com nome único
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.UploadedImage.FileName);
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "services");
            Directory.CreateDirectory(uploadsFolder);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.UploadedImage.CopyToAsync(fileStream);
            }

            // Atualiza a URL da imagem no banco de dados
            service.ServiceImageUrl = $"/images/services/{uniqueFileName}";
        }

        // 4. Salva todas as alterações no banco
        await context.SaveChangesAsync();

        return service;
    }
}