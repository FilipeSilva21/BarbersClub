using BarbersClub.Business.DTOs;
using Repository.Models;

namespace BarbersClub.Business.Services.Interfaces;

public interface IBarberShopService
{
    Task<BarberShop?> RegisterBarberShopAsync(int userId, BarberShopRegisterRequest request);
    
    Task<BarberShopViewResponse?> GetBarberShopByIdAsync(int barberShopId);
    
    Task<IEnumerable<BarberShop?>> GetBarberShopsByUserIdAsync(int userId);
    
    Task<IEnumerable<BarberShopViewResponse?>> GetBarberShopsAsync();
    
    Task<IEnumerable<BarberShopViewResponse?>> SearchBarberShopsAsync(string? barberShopName, string? state, string? city, string? barberName);
    
    Task<BarberShop?> UpdateBarberShopAsync(int barberShopId, int userId, BarberShopUpdateRequest request);
    
    Task<bool> DeleteBarberShopAsync(int barberShopId, int userId);

    Task<BarberShop?> GetBarberShopForUpdateAsync(int barberShopId);

    Task<IEnumerable<User>> GetClientsByBarberShopAsync(int barberShopId);

    Task<IEnumerable<TopRatedBarberShopResponse>> GetTopRatedBarberShopAsync(int count);
}