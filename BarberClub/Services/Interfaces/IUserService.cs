using BarberClub.Models;

namespace BarberClub.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetUserByIdAsync(int userId);
    
    Task<IEnumerable<User>> GetUserByEmailAsync(string email);

    Task<IEnumerable<User>> GetUsers();
}