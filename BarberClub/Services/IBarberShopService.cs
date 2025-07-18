using BarberClub.DTOs;
using BarberClub.Models;

namespace BarberClub.Services;

public interface IBarberShopService
{
    Task<BarberShop?> RegisterBarberShop(int userId, BarberShopRegisterRequest request);
    
    Task<BarberShop?> GetBarberShopById(int barberShopId);
    
    Task<IEnumerable<BarberShop?>> GetBarberShops();
    
    Task<IEnumerable<BarberShop?>> SearchBarberShops(string? barberShopName, string? state, string? city, string? barberName);
    
    Task<BarberShop?> UpdateBarberShop(int barberShopId, int userId, BarberShopUpdateRequest request);
    
    Task<bool> DeleteBarberShop(int barberShopId, int userId);
}