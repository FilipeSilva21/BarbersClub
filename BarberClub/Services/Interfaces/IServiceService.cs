using BarberClub.DTOs;
using BarberClub.Models;

namespace BarberClub.Services.Interfaces;

public interface IServiceService
{
    Task<ServiceViewResponse> CreateServiceAsync(ServiceRegisterRequest request);
    Task<IEnumerable<ServiceViewResponse>> GetServicesByBarberShopAsync(int barberShopId);
    Task<List<ServiceViewResponse>> GetServicesByUserAsync(int userId);

    Task<IEnumerable<ServiceViewResponse>> GetServicesAsync(
        string? barberShopName,
        string? clientName,
        string? serviceType,
        DateTime? startDate,
        DateTime? endDate
    );

    Task<IEnumerable<string>> GetBookedTimesAsync(int barberShopId, DateTime date);
    
    Task<bool> CancelServiceAsync(int serviceId, int userId);

    Task<IEnumerable<ServiceViewResponse>> GetServicesByUserAndStatusAsync(int userId, string status);

    Task<Service?> UpdateServiceAsync(int serviceId, ServiceUpdateRequest request);
}