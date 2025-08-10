using BarberClub.DTOs;
using BarberClub.Models;
using BarbersClub.Services.Interfaces;
using BarbersClub.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BarberClub.Services;

public class UserService(ProjectDbContext context, IWebHostEnvironment webHostEnvironment) : IUserService
{
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await context.Users
            .Include(u => u.Services)
            .Include(u => u.Ratings)
            .SingleOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await context.Users.ToListAsync();
    }
    
    public async Task<User?> UpdateUserAsync(int userId, UserUpdateRequest request)
    {
        var user = await context.Users.FindAsync(userId);
        if (user is null)
        {
            return null;
        }

        if (await context.Users.AnyAsync(u => u.Email == request.Email && u.UserId != userId))
        {
            return null;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;

        if (request.ProfilePicFile != null && request.ProfilePicFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(user.ProfilePicUrl))
            {
                var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, user.ProfilePicUrl.TrimStart('/'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ProfilePicFile.FileName);
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "users");
            Directory.CreateDirectory(uploadsFolder);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.ProfilePicFile.CopyToAsync(fileStream);
            }

            user.ProfilePicUrl = $"/images/users/{uniqueFileName}";
        }

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return null;
            }

            var passwordHasher = new PasswordHasher<User>();
            var newHashedPassword = passwordHasher.HashPassword(user, request.NewPassword);
            user.PasswordHashed = newHashedPassword;
        }

        await context.SaveChangesAsync();

        return user;
    }
    
    public async Task<IEnumerable<User>> GetUserByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }
}