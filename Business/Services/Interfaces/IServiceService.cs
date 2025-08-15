using BarbersClub.Business.DTOs;
using Repository.Models;

namespace BarbersClub.Business.Services.Interfaces;

public interface IServiceService
{
    Task<ServiceViewResponse> CreateServiceAsync(ServiceRegisterRequest request);
    Task<IEnumerable<ServiceViewResponse>> GetServicesByBarberShopAsync(
        int barberShopId,
        string? clientName,
        string? serviceType,
        DateTime? startDate,
        DateTime? endDate,
        TimeSpan? time);
    Task<List<ServiceViewResponse>> GetServicesByUserAsync(int userId);

    Task<IEnumerable<ServiceViewResponse>> GetServicesAsync(
        string? barberShopName,
        string? serviceType,
        string? barber,
        DateTime? startDate,
        DateTime? endDate,
        bool? hasPhoto,
        string? sortBy
    );

    Task<Service?> GetServiceByIdAsync(int serviceId);

    Task<IEnumerable<string>> GetBookedTimesAsync(int barberShopId, DateTime date);
    
    Task<bool> CancelServiceAsync(int serviceId, int userId);

    Task<IEnumerable<ServiceViewResponse>> GetServicesByUserAndStatusAsync(int userId, string status);

    Task<Service?> UpdateServiceAsync(int serviceId, ServiceUpdateRequest request);
    
    Task<bool> ConcludeServiceAsync(int serviceId);
    Task<bool> ConcludeServiceAsync(int serviceId, string photoPath);
}