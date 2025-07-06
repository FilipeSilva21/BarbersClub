using BarberClub.DTOs;
using BarberClub.Models;

namespace BarberClub.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserRegisterRequest request);
    
    Task<string?> LoginAsync(UserLoginRequest request);
}