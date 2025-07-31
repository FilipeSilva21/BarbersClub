using BarberClub.DTOs;
using BarberClub.Models;

namespace BarberClub.Services.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserRegisterRequest request);
    
    Task<(string? token, User? user)> LoginAsync(UserLoginRequest request);
    
    Task<BarberShop?> GetAllBarberShopsByUser(int userId);
}