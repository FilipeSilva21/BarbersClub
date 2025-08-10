using BarberClub.DTOs;
using BarberClub.Models;

namespace BarbersClub.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(int userId);
    
    Task<IEnumerable<User>> GetUserByEmailAsync(string email);

    Task<IEnumerable<User>> GetUsers();
    
    Task<User?> UpdateUserAsync(int userId, UserUpdateRequest request); 
}