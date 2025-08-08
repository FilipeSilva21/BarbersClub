using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarberClub.DbContext;
using BarberClub.DTOs;
using BarberClub.Models;
using BarberClub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BarberClub.Services;

public class AuthService(ProjectDbContext context, IConfiguration config, IWebHostEnvironment webHostEnvironment): IAuthService
{
    public async Task<User?> RegisterAsync(UserRegisterRequest request)
    {
        if (await context.Users.AnyAsync(u => u.Email == request.Email))
            return null;
        
        if(request.Password != request.ConfirmPassword)
            return null;
        
        var user = new User();
        
        var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.PasswordHashed = hashedPassword;
        user.Role = request.Role;

        if (request.ProfilePic != null && request.ProfilePic.Length > 0)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ProfilePic.FileName);
        
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "users");
            Directory.CreateDirectory(uploadsFolder); 
            
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.ProfilePic.CopyToAsync(fileStream);
            }

            user.ProfilePicUrl = $"/images/users/{uniqueFileName}";
        }
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        return user;
    }

    public async Task<(string? token, User? user)> LoginAsync(UserLoginRequest request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null)
            return (null, null);

        if (new PasswordHasher<User>().VerifyHashedPassword(
                user,
                user.PasswordHashed,
                request.Password) == PasswordVerificationResult.Failed)
            return (null, null);
    
        var token = GenerateToken(user);
        
        Console.WriteLine("Entrou nessa desgraca" + token);
        
        return (await token, user); 
    }

    public async Task<BarberShop?> GetAllBarberShopsByUser(int userId)
    {
        return await context.BarberShops
            .Include(u => u.Barber)
            .FirstOrDefaultAsync(b => b.UserId == userId);
    }

    private async Task<string> GenerateToken(User user)
    {
        var barberShopIds = await context.BarberShops
            .Where(b => b.UserId == user.UserId)
            .Select(b => b.BarberShopId)
            .ToListAsync();

        var claims = new List<Claim>
        {
            new Claim("firstName", user.FirstName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("hasBarberShops", barberShopIds.Any().ToString())
        };

        foreach (var id in barberShopIds)
        {
            claims.Add(new Claim("barberShopId", id.ToString()));
        }

        var tokenKeyString = config.GetValue<string>("Jwt:SecretKey");
        if (string.IsNullOrEmpty(tokenKeyString))
        {
            throw new InvalidOperationException("A chave do token ('Jwt:SecretKey') não está configurada.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: config.GetValue<string>("Jwt:Issuer"),
            audience: config.GetValue<string>("Jwt:Audience"),
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}