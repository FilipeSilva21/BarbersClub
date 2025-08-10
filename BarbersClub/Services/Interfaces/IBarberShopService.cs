using BarberClub.DTOs;
using BarberClub.Models;

namespace BarbersClub.Services.Interfaces;

public interface IBarberShopService
{
    Task<BarberShop?> RegisterBarberShopAsync(int userId, BarberShopRegisterRequest request);
    
    Task<BarberShop?> GetBarberShopByIdAsync(int barberShopId);
    
    Task<IEnumerable<BarberShop?>> GetBarberShopsByUserIdAsync(int userId);
    
    Task<IEnumerable<BarberShop?>> GetBarberShopsAsync();
    Task<IEnumerable<User>> GetClientsByBarberShopAsync(int barberShopId);
    
    Task<IEnumerable<BarberShop?>> SearchBarberShopsAsync(string? barberShopName, string? state, string? city, string? barberName);
    
    Task<BarberShop?> UpdateBarberShopAsync(int barberShopId, int userId, BarberShopUpdateRequest request);
    
    Task<bool> DeleteBarberShopAsync(int barberShopId, int userId);
}