using BarbersClub.Business.DTOs;
using BarbersClub.Business.Services.Interfaces;
using Business.Error_Handling;
using Business.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.DbContext;
using Repository.Models;

namespace Business.Services;

public class UserService(ProjectDbContext context, IWebHostEnvironment webHostEnvironment) : IUserService
{
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
        if (user is null)
            throw new UserIdNotFoundException(userId);
        
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
            throw new UserIdNotFoundException(userId);

        if (await context.Users.AnyAsync(u => u.Email == request.Email && u.UserId != userId))
            throw new EmailNotFoundException(request.Email);
        
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

        if (!string.IsNullOrWhiteSpace(request.NewPassword) || !string.IsNullOrWhiteSpace(request.ConfirmPassword))
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
                throw new ArgumentException("Para alterar a senha, informe Nova Senha e Confirmar Senha.");
            if (!string.Equals(request.NewPassword, request.ConfirmPassword))
                throw new ArgumentException("As senhas não conferem.");

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHashed = passwordHasher.HashPassword(user, request.NewPassword!);
        }

        await context.SaveChangesAsync();

        return user;
    }
    
    
}