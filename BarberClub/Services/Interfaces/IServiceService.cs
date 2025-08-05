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
        DateTime? endDate);
}