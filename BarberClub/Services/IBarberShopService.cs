using BarberClub.DTOs;
using BarberClub.Models;

namespace BarberClub.Services;

public interface IBarberShopService
{
    Task<BarberShop?> RegisterBarberShop(int userId, BarberShopRegisterRequest request);
    
    Task<BarberShop?> GetBarberShopById(int barberShopId);
    
    Task<IEnumerable<BarberShop?>> GetBarberShopsByState(string state);
    
    Task<IEnumerable<BarberShop?>> GetBarberShopsByCity(string city);
    
    Task<IEnumerable<BarberShop?>> GetAllBarberShops();
    
    Task<IEnumerable<BarberShop?>> GetAllBarberShopsByUserId(int userId);
    
    Task<BarberShop?> UpdateBarberShop(int barberShopId, int userId, BarberShopUpdateRequest request);
    
    Task<bool> DeleteBarberShop(int barberShopId, int userId);
}