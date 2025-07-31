using BarberClub.DTOs;

namespace BarberClub.Services;

public interface IServiceService
{
    Task<ServiceViewResponse> CreateServiceAsync(ServiceRegisterRequest request);
    Task<IEnumerable<ServiceViewResponse>> GetServicesByBarberShopAsync(int barberShopId);
    Task<IEnumerable<ServiceViewResponse>> GetServicesByUserAsync(int userId);
}