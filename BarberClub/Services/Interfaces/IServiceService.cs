using BarberClub.DTOs;
using BarberClub.Models;

namespace BarberClub.Services.Interfaces;

public interface IServiceService
{
    Task<ServiceViewResponse> CreateServiceAsync(ServiceRegisterRequest request);
    Task<IEnumerable<Service?>> GetServicesAsync();
    Task<IEnumerable<ServiceViewResponse>> GetServicesByBarberShopAsync(int barberShopId);
    Task<IEnumerable<ServiceViewResponse>> GetServicesByUserAsync(int userId);
}