using BarberClub.DbContext;
using BarberClub.Models;
using BarberClub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarberClub.Services;

public class UserService(ProjectDbContext context) : IUserService
{
    public async Task<IEnumerable<User>> GetUserByIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<User>> GetUserByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await context.Users.ToListAsync();
    }
}