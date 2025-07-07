using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarberClub.DbContext;
using BarberClub.DTOs;
using BarberClub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BarberClub.Services;

public class AuthService(UserDbContext context, IConfiguration config): IAuthService
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
        return (token, user); 
    }
    
    private string GenerateToken(User users)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, users.FirstName), 
            new Claim(ClaimTypes.Email, users.Email),
            new Claim(ClaimTypes.NameIdentifier, users.Id.ToString()),
            new Claim(ClaimTypes.Role, users.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config.GetValue<string>("AppSettings:Token")!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        
        var tokenDescriptor = new JwtSecurityToken(
            issuer: config.GetValue<string>("AppSettings:Issuer"),
            audience: config.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}